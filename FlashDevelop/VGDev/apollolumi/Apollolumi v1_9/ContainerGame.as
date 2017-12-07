/*
	ContainerGame.as

	Alexander Huynh
	11-3-13
	VGDev - Apollolumi
	
	Main game engine and control object.
*/

package 
{
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.ui.Keyboard;
	import flash.ui.Mouse;
	import flash.utils.Timer;
	import flash.events.TimerEvent;
	import flash.utils.getTimer;
	import flash.display.MovieClip;
	import flash.display.BitmapData;
	import flash.geom.Point;
	import props.*;
	import utils.*;
	import managers.*;
	import scenarios.*;
	import flash.geom.ColorTransform;
	//import flash.utils.getQualifiedClassName;

	public class ContainerGame extends Container
	{
		// -- addEventListener was overridden to make removing them easier; this holds the listeners
		private var listeners:Array = [];
		
		// -- scenario
		public var scen:Scenario;
		public var stageInd:int;			// number of stage, starting at 1
		
		// -- managers
		public var guiMan:GUIManager;
		public var astMan:AsteroidManager;
		public var minMan:MineralManager;
		public var strMan:StructureManager;
		public var eneMan:EnemyManager;
		public var projMan:ProjectileManager;
		public var managerV:Vector.<Manager> = new Vector.<Manager>();		// all managers
		public var managerC:Vector.<Manager> = new Vector.<Manager>();		// managers with collidable objects
		public var objArr:Vector.<MovieClip> = new Vector.<MovieClip>();	// for generic un-interactive things like DebrisFX
		public var sndMan:SoundManager;
		public var str_CC:Structure;	// command center
		public var minimap:MovieClip;	// minimap in guiMan.guiLeft
		private var mmX:Number;			// starting location
		private var mmY:Number;

		// -- controls
		public var leftDown:Boolean, rightDown:Boolean;		// if keys are down
		public var wDown:Boolean, aDown:Boolean, sDown:Boolean, dDown:Boolean, shiftDown:Boolean;
		public var dX:Number = 0, dY:Number = 0;			// viewpoint speed
		public var LIM_X:int, LIM_Y:int;					// bounds of map
		public var isDeploying:Boolean;						// T if about to deploy a new structure

		// -- tractor
		public var tractTgt:MovieClip;						// target of tractor
		private var grabX:Number = 0, grabY:Number = 0;		// location on object grabbed
		public var tPt:Point;
		public var tractInterrupt:Boolean;					// if broken by collision

		// -- timing
		public var limitTimer:Timer;						// main timer (used only as time limit)
		public var timeLeft:Number;							// time left in milliseconds
		public var TICK:Number = 33, tick:Number = TICK;	// interval to fire (and update time textfield)
		public var timeEndless:int = 1;						// set to negative to count up
		public var timeMin:int, timeSec:int, timeMSec:int;	// define here to avoid defining new vars every 33ms
		public var timeWarn:int = 30000;					// time threshold to trigger a low time warning in ms
		public var timeLowFlag:Boolean, isPaused:Boolean;
		private var fpsCount:int;							// used in temp FPS counter
		private var fpsPrev:int;
		public var stageActive:Boolean;						// false when time is 0 or next stage is requested by player
		public var enWave:int;								// time until next enemy wave
		public var isFinalStage:Boolean;					// TRUE on final stage
		
		// -- gameplay
		public var winState:String = "quit";
		public var score:int;
		public var scoreMode:String = "NORM";
		public var mineralVec:Vector.<int> = new Vector.<int>(4, true);		// how much of each mineral player has (R, Y, G, B)
		public var objectives:Vector.<int> = new Vector.<int>(4, true);		// minerals needed to repair vital part
		public var laserCharge:Number = 100;		// charge of laser (%)
		public var laserCool:Boolean;				// true if laser is fully depleted and recharging
		public var tractorCharge:Number = 100;		// charge of tractor (%)
		public var tractorCool:Boolean;				// true if tractor is fully depleted and recharging
		
		public var dMineral:Vector.<int> = new Vector.<int>(4, true);
		public var dScore:int;
		public var minBonus:Number = 1
		
		// -- stats
		// laser
		public var laserDmg:Number = 2.5;
		public var laserDrain:Number = .65;			// amount to drain laser charge per frame (%)
		public var laserRecharge:Number = 1;		// amount to recharge laser charge per frame (%)
		public var laserDepleteCharge:Number = 1.5;	// amount to recharge laser charge if fully depleted per frame (%)
		// tractor
		public var tractDist:Number = 400;			// minimum tractor beam length for 100% drag speed multiplier
		public var tractorDrain:Number = 1;			// amount to drain tractor charge per frame (%)
		public var tractorRecharge:Number = 1;		// amount to recharge tractor charge per frame (%)
		public var tractorDepleteCharge:Number = 1.5;	// amount to recharge tractor charge if fully depleted per frame (%)
		public var tractorRotRed:Number = 1;			// amount to multiply rotation speed of asteroids by (%)
		
		// Mineral Synthesizer
		public var synth:Boolean;					// if you have it
		public var synthCnt:int = 1;				// timer
		public var synthTime:int = 40;				// time to reset by
		
		// viewpoint
		public var moveSpd:Number = .75;			// viewpoint acceleration speed
		public var minSpd:Number = .3;				// highest speed before speed is set to 0
		public var maxSpd:Number = 3;
		public var brakeSpd:Number = .9;
		public var friction:Number = .98;
		
		// -- parallax
		public var parallaxArr:Array = [];			// objects such as distant planets
		public var parallaxAmt:Array = [];			// distance factor to multiply by
		
		// -- other
		public var mPt:Point;			// reuse for mouse's global position
		public var uiMC:MovieClip;		// closest item for UI display
		public var uiDist:Number = 0;	// used to get closest item for UI display
		
		private var colTRed:ColorTransform = new ColorTransform();		// for low warnings
		private var colTBlue:ColorTransform = new ColorTransform();
		private var tractLastUsed:Boolean;	// F for laser, T for tractor
		
		public function ContainerGame(s:String)
		{
			super();
			loadScenario(s);
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{			
			removeEventListener(Event.ADDED_TO_STAGE, init);
			// add listeners for control
			stage.addEventListener(MouseEvent.MOUSE_DOWN, onMLdown);
			stage.addEventListener(MouseEvent.MOUSE_UP, onMLup);
			stage.addEventListener(MouseEvent.RIGHT_MOUSE_DOWN, onMRdown);
			stage.addEventListener(MouseEvent.RIGHT_MOUSE_UP, onMRup);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyboardDn);
			stage.addEventListener(KeyboardEvent.KEY_UP, onKeyboardUp);
			
			// setup managers
			guiMan = new GUIManager(this, GUI_left, GUI_mid, GUI_right, GUI_warning, GUI_enemy, GUI_deploy);
														managerV.push(guiMan);
			strMan = new StructureManager(this);		managerV.push(strMan);	managerC.push(strMan);
			astMan = new AsteroidManager(this);			managerV.push(astMan);	managerC.push(astMan);	
			minMan = new MineralManager(this);			managerV.push(minMan);
			eneMan = new EnemyManager(this, strMan, astMan);
			projMan = new ProjectileManager(this);		managerV.push(projMan);	managerC.push(projMan);	
														managerV.push(eneMan);	managerC.push(eneMan);	
			sndMan = new SoundManager(this);
			minimap = guiMan.guiLeft.minimap;
			mmX = minimap.x;
			mmY = minimap.y;

			// let mouse see through cursor MovieClip
			cursor.buttonMode = false;
			cursor.mouseEnabled = false;
			cursor.base.buttonMode = false;
			cursor.base.mouseEnabled = false;

			setupStage();

			// create Command Center
			str_CC = new CommandCenter(strMan, this, 0, 0, 0, 0, 100, 3);
			strMan.addStruct(str_CC);
			guiMan.addArrow(str_CC, 0x3F9359);		// add arrow for HQ
			
			// other init
			score = 0;
			stageInd = 1;
			guiMan.guiLeft.tf_sector.text = "SECTOR 1";
			colTRed.color = 0xFF0000;
			colTBlue.color = 0x00FFFF;
		}
		
		public function setupStage():void
		{			
			// setup GUI
			GUI_pause.visible = GUI_tech.visible = GUI_build.visible = false;
			GUI_laserWarn.visible = guiMan.guiMid.btn_next.visible = false;
			buyWarning.gotoAndStop(1);

			trace("Set up stage.");
			
			dMineral = new Vector.<int>(4, true);
			dScore = 0;
			
			// clear managers other than GUI and structures
			for each (var m:Manager in managerC)
				if (!(m is GUIManager) && !(m is StructureManager))
					m.destroy();
			guiMan.clearArrows();
			minMan.destroy();
			for each (var mc:MovieClip in objArr)
				if (game.cont.contains(mc))
					game.cont.removeChild(mc);
			for each (mc in strMan.getVec())		// reset turrets
				mc.resetTurret();
			if (parallaxArr && parallaxArr.length > 0)
				for each (mc in parallaxArr)		// remove parallax objects
					if (game.back.contains(mc))
						game.back.removeChild(mc);
			parallaxArr = [];
			parallaxAmt = [];
					
			// setup part cost
			GUI_build.tier = scen.diffMod;
			
			// setup time limit
			timeLowFlag = false;
			timeLeft = scen.stageTime;		// set time left in stage
			limitTimer = new Timer(tick);
			limitTimer.addEventListener(TimerEvent.TIMER, limitTimerTick);
			limitTimer.start();
			isFinalStage = scen.stageNext == "END";
			if (isFinalStage && scoreMode == "NORM")
			{
				guiMan.guiMid.surviveLabel.visible = true;
				guiMan.guiRight.tf_timeType.text = "WARPDRIVE CHARGE";
			}

			// setup enemy waves
			enWave = scen.eneWave;
			
			// create background and limits
			var bg:BitmapData;
			switch (scen.stageBG)
			{
				case 1:
					bg = new BG_Space0(0, 0);
				break;
				case 2:
				default: bg = new BG_Space1(0, 0);
			}
			
			game.back.graphics.clear();
			var HSIZE:int = scen.stageLen;
			var VSIZE:int = scen.stageHig;
			game.back.graphics.beginBitmapFill(bg);
			game.back.graphics.drawRect(-HSIZE * .5, -VSIZE * .5, HSIZE, VSIZE);
			game.back.graphics.endFill();
			
			LIM_X = game.back.width * .5 - 400;
			LIM_Y = game.back.height * .5 - 300;

			// populate space based on scenario
			scen.spawnStructures();
			
			// update GUI
			guiMan.guiLeft.tf_sector.text = "SECTOR " + stageInd.toString();
			
			// auto-buy if endless, other set up
			if (scoreMode != "NORM")
			{
				trace("AUTO-BUY");
				GUI_build.motherBought.visible = true;
				GUI_build.bTime.visible = false;
				GUI_build.tf_time.visible = false;
				timeEndless = -1;
				guiMan.guiMid.endlessLabel.visible = true;
				guiMan.guiRight.tf_timeType.text = "TIME ELAPSED";
				timeLowFlag = true;
				objective.visible = false;
			}
			else
			{
				objective.gotoAndPlay(1);
				trace("Playing objective...");
			}
			scoreOverlay.visible = false;
			outro.visible = false;
			//ui_redFlash.visible = false;
			
			//trace(GUI_build.motherBought.visible);
			
			stageActive = true;
		}
		
		public function loadScenario(s:String):void
		{
			switch (s)
			{
				case "tutorial":
					scen = new ScTutorial(this);
				break;
				case "vanilla1":
					scen = new ScVanilla1(this);
					minBonus = 1.5;
				break;
				case "vanilla2":
					scen = new ScVanilla2(this);
				break;
				case "vanilla3":
					scen = new ScVanilla3(this);
				break;
				case "easy1":
					scen = new ScEasy1(this);
					minBonus = 2;
				break;
				case "easy2":
					scen = new ScEasy2(this);
				break;
				case "easy3":
					scen = new ScEasy3(this);
				break;
				case "hard1":
					scen = new ScHard1(this);
				break;
				case "hard2":
					scen = new ScHard2(this);
				break;
				case "hard3":
					scen = new ScHard3(this);
				break;
				case "mineral":
					scen = new ScMineral(this);
				break;
				default:
					scen = new ScVanilla1(this);
				break;
			}
		}

		// creates n asteroids at a random position (not in the 'safe zone' near the spawn)
		// set t to define a mineral type; else will be random
		public function spawnAsteroid(n:int, t:int = -1, spawnOutside:Boolean = true):void
		{
			trace("I'm still alive");
			var xP:Number, yP:Number;
			for (var i:int = 0; i < n; i++)
			{
				if (!spawnOutside)
					do
					{
						xP = getRand(-1000, 1000);
						yP = getRand(-800, 800);
					} while (abs(xP) < 400 || abs(yP) < 300);
				else
				{					
					if (Math.random() > .5)
					{
						xP = (LIM_X + 50) * -1.5;
						xP += getRand(0, (LIM_X + 50) * 1.5);
						yP = (LIM_Y + 50) * -1.5 * (Math.random() > .5 ? 1 : -1);
					}
					else
					{
						yP = (LIM_Y + 50) * -1.5;
						yP += getRand(0, (LIM_Y + 50) * 1.5);
						xP = (LIM_X + 50) * -1.5 * (Math.random() > .5 ? 1 : -1);
					}
				}
				
				// spawn asteroid with mineral type you have the least of
				if (t == -1 && Math.random() > .4)
					if (!(mineralVec[0] == mineralVec[1] == mineralVec[2] == mineralVec[3]))
						t = getLowestMin();
				
				astMan.spawnAsteroid(xP, yP, scen.astVelMin, scen.astVelMax, scen.astRotMin, scen.astRotMax,
									 scen.astSize, (t == -1 ? getRand(-1, 3) : t), spawnOutside);
				//tf_mid.text = "Asteroid " + (t == -1 ? "" : "("+t+") ") + "spawned @ " + Math.round(xP) + "," + Math.round(yP) + "!";
			}
		}

		// places asteroid with specified parameters
		public function placeAsteroid(_xP:Number, _yP:Number, _velocity:Number, _rotSpeed:Number, _size:int, _mineral:int)
		{
			//trace("CG: Placed asteroid at " + _xP + " " + _yP);
			astMan.spawnAsteroid(_xP, _yP, _velocity, _velocity, _rotSpeed, _rotSpeed, _size, _mineral, false);
		}

		// creates n Kamikaze enemies
		public function spawnKami(n:int):int
		{
			var xP:Number = (LIM_X + 50) * 2 * (Math.random() > .5 ? 1 : -1);
			var yP:Number = (LIM_Y + 50) * 2 * (Math.random() > .5 ? 1 : -1);
			var e:Enemy;
			for (var i:int = 0; i < n; i++)
			{
				e = new EnemyKami(eneMan, this, xP + getRand(-50, 50), yP + getRand(-50, 50), 25 * scen.multHP, 7);
				eneMan.addEnemy(e);
				if (i == 0)
					guiMan.addArrow(e, 0xFA3030);		//0xB247D9
			}
			return n;
		}

		// creates n Interferers
		public function spawnInterf(n:int):int
		{
			var xP:Number, yP:Number;
			var e:Enemy;
			for (var i:int = 0; i < n; i++)
			{
				xP = (LIM_X + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				yP = (LIM_Y + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				e = new EnemyInterf(eneMan, this, xP + getRand(-50, 50), yP + getRand(-50, 50), 100 * scen.multHP, 7);
				eneMan.addEnemy(e);
				guiMan.addArrow(e, 0xFA3030);
			}
			return n;
		}

		// creates n Draggers
		public function spawnDragger(n:int):int
		{
			var xP:Number, yP:Number;
			var e:Enemy;
			for (var i:int = 0; i < n; i++)
			{
				xP = (LIM_X + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				yP = (LIM_Y + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				e = new EnemyDrag(eneMan, this, xP + getRand(-50, 50), yP + getRand(-50, 50), 200 * scen.multHP, 7);
				eneMan.addEnemy(e);
				guiMan.addArrow(e, 0xFA3030);
			}
			return n;
		}

		// creates n Shooters
		public function spawnShooter(n:int):int
		{
			var xP:Number, yP:Number;
			var e:Enemy;
			for (var i:int = 0; i < n; i++)
			{
				xP = (LIM_X + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				yP = (LIM_Y + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				e = new EnemyShooter(eneMan, this, xP + getRand(-50, 50), yP + getRand(-50, 50), 70 * scen.multHP, 7);
				eneMan.addEnemy(e);
				guiMan.addArrow(e, 0xFA3030);
			}
			return n;
		}
		
		// creates n Bombers
		public function spawnBomber(n:int):int
		{
			var xP:Number, yP:Number;
			var e:Enemy;
			for (var i:int = 0; i < n; i++)
			{
				xP = (LIM_X + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				yP = (LIM_Y + 50) * 2 * (Math.random() > .5 ? 1 : -1);
				e = new EnemyBomb(eneMan, this, xP + getRand(-50, 50), yP + getRand(-50, 50), 100 * scen.multHP, 5);
				eneMan.addEnemy(e);
				guiMan.addArrow(e, 0xFA3030);
			}
			return n;
		}
		
		public function spawnStructure(str:String):void
		{
			var sd:StructureDeploy = new StructureDeploy(this, -game.x, -game.y, str)
			objArr.push(sd);
			game.cont.addChild(sd);
		}
		
		public function addProjectile(xP:int, yP:int, spd:Number, rot:Number, type:String, life:int,
									  dmg:Number, isFriendly:Boolean, tgt:Floater = null):void
		{
			projMan.addProjectile(xP, yP, spd, rot, type, life, dmg, isFriendly, tgt);
		}

		// --		--		--		--		--		--		--		--		--		CONTROL
		private function onKeyboardDn(e:KeyboardEvent):void
		{
			//trace("KEYBOARD!");
			if (!stageActive)
				return;
			if (e.shiftKey) shiftDown = true;
			switch (e.keyCode)
			{
				case Keyboard.W: wDown = true; break;
				case Keyboard.A: aDown = true; break;
				case Keyboard.S: sDown = true; break;
				case Keyboard.D: dDown = true; break;
				//case Keyboard.Q: onMLdown(null); break;
				case Keyboard.E: onMRdown(null); break;
				case Keyboard.P:
					togglePause(0);
				break;
				//case Keyboard.I: spawnAsteroid(1, 0); break;			// temphack
				//case Keyboard.O: spawnAsteroid(1, 3); break;			// temphack
				//case Keyboard.L: spawnKami(5 + getRand(0, 7)); break;	// temphack
				//case Keyboard.U: spawnKami(1); break;					// temphack
				//case Keyboard.J: spawnInterf(3); break;	// temphack
				//case Keyboard.K: spawnInterf(1); break;	// temphack
				//case Keyboard.T: spawnDragger(3); break;	// temphack
				//case Keyboard.Y: spawnDragger(1); break;	// temphack
				case Keyboard.Z: spawnBomber(1); break;	// temphack
				case Keyboard.X: spawnShooter(1); break;	// temphack
				/*case Keyboard.R:
					completed = true;
					outro.gotoAndStop(outro.totalFrames)
				break;	*/			// temphack
				case Keyboard.M:
					changeMineral(0, 10);
					changeMineral(1, 10);
					changeMineral(2, 10);
					changeMineral(3, 10);
				break;				// temphack
				//case Keyboard.Q:
					//guiMan.updateScore(1500);
				break;
				//case Keyboard.N: minMan.spawnMineral(mouseX, mouseY, getRand(0,3)); break;				// temphack
				case Keyboard.B:
					if (GUI_pause.visible) return;
					togglePause(1);
					guiMan.onBuild(null);
					/*var pro:Projectile = new Projectile(game, this, 0, 0, 13, getRand(0, 359), "", 30, 3);
					objArr.push(pro);
					game.addChild(pro);*/
				break;
				case Keyboard.V:
					if (GUI_pause.visible) return;
					togglePause(1);
					guiMan.onTech(null);
				break;
			}
		}
		
		public function togglePause(code:int):void
		{
			if (!stageActive)	// don't let pause unless game is going
				return;
			switch (code)
			{
				case 0:		// P key
					isPaused = !isPaused;
					GUI_pause.visible = isPaused;
					if (!GUI_pause.visible)
						GUI_pause.onNo(null);
					if (!GUI_pause.visible && (GUI_build.visible || GUI_tech.visible))
						isPaused = true;
				break;
				case 1:		// enter menu
					isPaused = true;
					GUI_pause.visible = false;
				break;
				case 2:		// exit menu
					isPaused = false;
				break;
			}
			trace("code: " + code);
			trace("togglePause" + stageActive);
			trace("Is paused? " + isPaused);
			if (limitTimer)
				isPaused ? limitTimer.stop() : limitTimer.start();
		}
		
		private function onKeyboardUp(e:KeyboardEvent):void
		{
			if (e.shiftKey) shiftDown = false;
			switch (e.keyCode)
			{
				case Keyboard.W: wDown = false; break;
				case Keyboard.A: aDown = false; break;
				case Keyboard.S: sDown = false; break;
				case Keyboard.D: dDown = false; break;
				//case Keyboard.Q: onMLup(null); break;
				case Keyboard.E: onMRup(null); break;
			}
		}
		
		private function doNothing(e:MouseEvent):void
		{
			// -- TEMPORARY
		}
		
		// left mouse
		private function onMLdown(e:MouseEvent):void
		{
			leftDown = true;
			
			//trace("Mouse: " + getQualifiedClassName(e.target));
		}
		
		private function onMLup(e:MouseEvent):void
		{
			leftDown = false;
			high.graphics.clear();
		}

		// right mouse
		private function onMRdown(e:MouseEvent):void
		{
			rightDown = true;
			Mouse.hide();
		}

		public function onMRup(e:MouseEvent):void
		{
			shutTractor();
			tractInterrupt = false;
			Mouse.hide();
		}

		// also called if an object being tractored collides with something
		public function shutTractor():void
		{
			rightDown = false;			// force mouse right button to be "up"
			tractInterrupt = true;		// let game know tractor beam was interrupted
			if (tractTgt)				// let object being tractored know it's free
				tractTgt.isTractorTgt = false;
			tractTgt = null;			// no tractor beam target anymore
			high.graphics.clear();		// clear tractor beam graphics
		}
		
		// handles mouse (left and right buttons)
		protected function onMouse():void
		{
			// clear graphics to make way for updated graphics
			high.graphics.clear();
			// don't continue if in a menu or tractor beam was interrupted
			if (guiMan.isOverUI(mPt) || tractInterrupt) return;				
			// handle left button
			if (!laserCool && leftDown && laserCharge > 0)
			{
				high.graphics.lineStyle(1, 0xFF0000, .8);					// primary beam
				high.graphics.moveTo(400, -300);
				high.graphics.lineTo(mouseX, mouseY);
				
				high.graphics.lineStyle(3, 0xFF0000, .5);
				high.graphics.lineTo(400 + Math.random() * 10 - 5, -300);	// random FX
				
				if (stageActive)
					sndMan.playSound("playerLaser");
			}
			// handle right button
			else if (!tractorCool && rightDown && tractorCharge > 0)
			{
				high.graphics.lineStyle(1, 0x3DD7D2, tractTgt ? .8 : .5);	// primary beam
				high.graphics.moveTo(400, -300);
				high.graphics.lineTo(mouseX, mouseY);
				
				high.graphics.lineStyle(3, 0x3DD7D2, tractTgt ? .5 : .2);	// random FX
				high.graphics.lineTo(400 + Math.random() * 10 - 5, -300);
				
				if (stageActive)
					sndMan.playSound("playerTractor");
			}
			// if tractor beam is towing something
			if (tractTgt)
			{
				// draw a line from the mouse location to the target location
				tPt = tractTgt.localToGlobal(new Point());
				// mPt set at function start
				
				tPt = high.globalToLocal(tPt);
				mPt = high.globalToLocal(mPt);
				
				high.graphics.lineStyle(2, 0x7BBDBB, .5);
				high.graphics.moveTo(tPt.x + grabX, tPt.y + grabY);
				high.graphics.lineTo(mPt.x, mPt.y);

				if (!(tractTgt is Floater))
				{
					tractTgt.x = mouseX;
					tractTgt.y = mouseY;
				}
				else
				{
					// tow the object
					var towX:Number = mPt.x - (tPt.x + grabX);
					if (abs(towX) > tractDist)
						towX = tractDist * (towX > 0 ? 1 : -1);
					var towY:Number = mPt.y - (tPt.y + grabY);
					if (abs(towY) > tractDist)
						towY = tractDist * (towY > 0 ? 1 : -1);
					var tX = (towX / tractDist) * tractTgt.tractSpd;
					var tY = (towY / tractDist) * tractTgt.tractSpd;
					tractTgt.x += tX; tractTgt.dX = tX;
					tractTgt.y += tY; tractTgt.dY = tY;					
				}
			}
		}
		
		// --		--		--		--		--		--		--		--		--		LOOPS
		override public function step():Boolean
		{
			// update the cursor's position
			cursor.x = mouseX;
			cursor.y = mouseY;
			mPt = localToGlobal(new Point(mouseX, mouseY));
			
			// quit if game is paused
			if (isPaused) return (completed && scoreOverlay.currentFrame == scoreOverlay.totalFrames);
			
			// nothing down if both are down
			if (leftDown && rightDown)
				leftDown = rightDown = false;
			
			// clear UI
			uiMC = null;
			uiDist = 2500;
			
			handleMovement();
			handleLaser();
			handleTractor();
			
			// check if deploying should be handled
			if (isDeploying)
			{
				if (leftDown)
				{
					var valid:Boolean = true;
					var mc:MovieClip;
					var allVec:Vector.<MovieClip> = astMan.getVec().concat(strMan.getVec(), eneMan.getVec());
					for each (mc in allVec)
						if (mc.getDistN(mc.x, mc.y, -game.x, -game.y) < 50)
						{
							valid = false
							break;
						}
					
					if (valid)
					{
						GUI_build.spawnStructure();
						isDeploying = guiMan.guiDeploy.visible = false;
					}
					else
						guiMan.guiDeploy.deployErr.gotoAndPlay(2);
				}
				else if (rightDown)
				{
					guiMan.onBuild(null);
					isDeploying = guiMan.guiDeploy.visible = false;
				}
			}
			else
			{
				if (!(leftDown && rightDown) && (leftDown || rightDown) || (tractTgt && !tractInterrupt)) 
					onMouse();
				else if (leftDown && rightDown && !tractTgt)
					high.graphics.clear();
			}

			// force left mouse to false if not charged
			var leftReady:Boolean = laserCharge > 0 ? (!laserCool && leftDown) : false;
			
			var i:int;
			
			if (stageActive)
			{
				//trace("step")
				Mouse.hide();
				// -- spawn more asteroids
				if (astMan.getVec().length <= scen.astSaturationMin ||
				   (astMan.getVec().length <= scen.astSaturationMax && Math.random() > .999))
				   {
					spawnAsteroid(getRand(1, scen.astSize), -1);
					//trace("Herp!");
				   }
					
				// step all managers
				for each (var man:Manager in managerV)
					man.step(leftReady, rightDown, managerC, mPt);
				// update enemy waves
				if (enWave % 30 == 0)
					tf_mid.text = "Enemies in " + enWave / 30;
				
				if (--enWave <= 0)
				{
					var num:int = scen.wave();
					enWave = scen.eneWaveSpace;				// reset wave time
					guiMan.enemyWarn(num + " enem" + (num == 1 ? "y" : "ies") + " inbound!");
				}

				scen.scenarioStep(timeLeft);		// do any scripted events
				
				if (synth && --synthCnt == 0)		// handle mineral synthesizer
				{
					synthCnt = synthTime;
					changeMineral(getLowestMin(), 1);
				}
				
				// step all uninteractive objects
				for (i = objArr.length - 1; i >= 0; i--)
					if (objArr[i].step(managerC, mPt))
						objArr.splice(i, 1);
						
				// update target UI
				if (uiMC)
				{
					GUI_left.infobar.visible = true;
					GUI_left.infobar.tf_tgt.text = uiMC.TITLE;
					GUI_left.infobar.hpBar.width = 100 * (uiMC.hp / uiMC.hpMax);
					if (GUI_left.infobar.hpBar.width < 0)
						GUI_left.infobar.hpBar.width = 0;
					GUI_left.infobar.tf_tgtHP.text = int(GUI_left.infobar.hpBar.width) + "%";
					
					if (uiMC is Structure)
						if (uiMC.range != 0)
							uiMC.drawRange();
				}
				else
				GUI_left.infobar.visible = false;
			}

			// cleanup
			if (completed) destroy();
			
			return (completed && scoreOverlay.currentFrame == scoreOverlay.totalFrames);
		}
		
		public function endStage():void
		{
			//trace("--- Stage end ---");
			
			stageActive = false;
			
			stopTime();
			
			if (scen.isEndless)
				outro.endless = true;
			outro.gotoAndPlay(2);
			outro.visible = true;
			outro.minerals = dMineral;
			changeScore(scen.diffMod * 10000);
			outro.score = score;
			outro.dScore = dScore;
			if (!GUI_build.motherBought.visible)		// try to auto-buy part
			{
				if (GUI_build.build_mother.costs[0] <= mineralVec[0] &&
				    GUI_build.build_mother.costs[1] <= mineralVec[1] &&
				    GUI_build.build_mother.costs[2] <= mineralVec[2] &&
				    GUI_build.build_mother.costs[3] <= mineralVec[3])
					{
						GUI_build.motherBought.visible = true;
						for (var i:int = 0; i < 4; i++)
							changeMineral(i, -GUI_build.build_mother.costs[i]);
					}
			}
			
			outro.success = GUI_build.motherBought.visible;
			stageInd++;
			
			if (!outro.success) return;
			
			// see MovieClip on layer "Outro Overlay" in ContainerGame
		}

		// handle time limit
		private function limitTimerTick(e:TimerEvent):void
		{
			timeLeft -= tick * timeEndless;
			
			// give score
			if (timeLeft % 4 == 0 && scoreMode != "MINE")
				changeScore(int(scen.diffMod * 3));

			if (!timeLowFlag && timeLeft < timeWarn && scoreMode == "NORM")
			{
				timeLowFlag = true;
				if (isFinalStage)
					guiMan.warningWarn("Warp almost charged!", 0xFFFFFF, true);
				else
				{
					guiMan.warningWarn("Time's almost up!", 0xFF0000, true);
					if (!GUI_build.motherBought.visible)
						buyWarning.gotoAndPlay(2);
				}
			}
			if (timeLeft < 0 && limitTimer)
			{
				timeLeft = 0;
				endStage();
			}
			guiMan.updateTime(getTime());
			
			// fps counter
			var currentTime:Number = getTimer();
			fpsCount++;
			if (currentTime >= fpsPrev + 1000)
			{
				fpsPrev = currentTime;
				tf_fps.text = "FPS: " + fpsCount;
				fpsCount = 0;
			}
		}

		// --		--		--		--		--		--		--		--		--		HELPERS
		private function getLowestMin():int
		{
			var minA:int = mineralVec[0];
			var minM:int = 0;
			for (var j:int = 1; j < 4; j++)
				if (mineralVec[j] < minA ||
					(mineralVec[j] == minA && Math.random() > .5))
				{
					minA = mineralVec[j];
					minM = j;
				}
			return minM;
		}
		
		public function changeScore(s:int):void
		{
			score += s;
			dScore += s;
			guiMan.updateScore(score);
		}
		
		public function setCompleted(c:Boolean):void
		{
			completed = c;
			resetStage();
			stageActive = false;		
			leftDown = rightDown = wDown = aDown = sDown = dDown = shiftDown = false;
			scoreOverlay.score = score;
			scoreOverlay.gotoAndStop(2);
			MovieClip(parent).ws = winState;
		}
		
		private function handleMovement():void
		{
			// update viewpoint position, speed
			game.x += dX; game.y += dY;
			if (abs(game.x) > LIM_X)
			{
				game.x = LIM_X * (game.x < 0 ? -1 : 1);
				dX *= -.5;
			}
			if (abs(game.y) > LIM_Y)
			{
				game.y = LIM_Y * (game.y < 0 ? -1 : 1);
				dY *= -.5;
			}
			
			minimap.x = mmX + game.x * minimap.scaleX;
			minimap.y = mmY + game.y * minimap.scaleY;
			
			/*if (shiftDown)
			{
				trace("BRAKES");
				dX *= brakeSpd;
				dY *= brakeSpd;
			}*/
				
			// -- left/right
			if (aDown && dDown)
			{
				dX *= brakeSpd;
				dY *= brakeSpd;
			}
			else
			{
				if (aDown) dX += moveSpd;
				if (dDown) dX -= moveSpd;
			}
			// -- up/down
			if (wDown && sDown)
			{
				dX *= brakeSpd;
				dY *= brakeSpd;
			}
			else
			{
				if (wDown) dY += moveSpd;
				if (sDown) dY -= moveSpd;
			}
			
			// apply friction
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			// handle parallax
			var j:int = parallaxArr.length;
			var mc:MovieClip;
			if (j == 0)
				return;
			for (var i:int = 0; i < j; i++)
			{
				parallaxArr[i].x += dX * parallaxAmt[i]; 
				parallaxArr[i].y += dY * parallaxAmt[i]; 
			}
		}
		
		private function handleLaser():void
		{
			cursor.maskL.height = .385 * laserCharge;
			
			if (leftDown)
				tractLastUsed = false;
			if (!tractLastUsed)
				GUI_laserWarn.visible = !tractLastUsed && (laserCool || laserCharge < 33);
			// if laser was depleted
			if (laserCool)
			{
				laserCharge += laserRecharge * laserDepleteCharge;
				if (laserCharge >= 100)
				{
					laserCharge = 100;
					if (!tractLastUsed)
						GUI_laserWarn.visible = false;
					laserCool = false;
				}
				if (!tractLastUsed)
					guiMan.updateLaser(laserCharge, colTRed);
				return;
			}
			// if laser is on
			if (leftDown && !guiMan.isOverUI(mPt))
			{
				laserCharge -= laserDrain;
				if (laserCharge < 0)
				{
					laserCharge = 0;
					laserCool = true;
					GUI_laserWarn.base.transform.colorTransform = colTRed;
					GUI_laserWarn.tf_title.text = "LASER RECHARGING";
				}
				else if (laserCharge < 33)
				{
					GUI_laserWarn.base.transform.colorTransform = colTRed;
					GUI_laserWarn.tf_title.text = "LOW LASER";
				}
				if (!tractLastUsed)
					guiMan.updateLaser(laserCharge, colTRed);
			}
			// if laser is not on
			else if (!leftDown && laserCharge < 100)
			{
				laserCharge += laserRecharge;
				if (laserCharge >= 100)
					laserCharge = 100;
				if (!tractLastUsed)
					guiMan.updateLaser(laserCharge, colTRed);
			}
		}
		
		// seek tractor targets and handle tractor beam
		private function handleTractor():void
		{			
			cursor.maskT.height = .385 * tractorCharge;
			
			if (rightDown)
				tractLastUsed = true;		// remember tractor was last used for low warnings
			if (tractLastUsed)
				GUI_laserWarn.visible = tractLastUsed && (tractorCool || tractorCharge < 33);
			// if tractor was depleted
			if (tractorCool)
			{
				tractorCharge += tractorRecharge * tractorDepleteCharge;
				if (tractorCharge >= 100)
				{
					tractorCharge = 100;
					if (tractLastUsed)
						GUI_laserWarn.visible = false;
					tractorCool = false;
				}
				if (tractLastUsed)
					guiMan.updateLaser(tractorCharge, colTBlue);
				return;
			}
			// if tractor is on
			if (rightDown && !guiMan.isOverUI(mPt))
			{
				var towMod:Number = 1;
				if (tractTgt)
				{
					switch (tractTgt.ID)
					{
						case "asteroid": 
							tractTgt.rotSpd *= tractorRotRed;
						break;
						case "structure": towMod = .5; break;
						case "enemy": towMod = 2; break;
					}
					tractorCharge -= tractorDrain * towMod * (towMod == 1 ? (1 / (tractorRotRed == 1 ? 1 : (tractorRotRed - .2))) : 1);
				}
				if (tractorCharge < 0)
				{
					tractorCharge = 0;
					tractorCool = true;
					if (tractLastUsed)
					{
						GUI_laserWarn.base.transform.colorTransform = colTBlue;
						GUI_laserWarn.tf_title.text = "TRACTOR RECHARGING";
					}
					shutTractor();
				}
				else if (tractorCharge < 33)
				{
					GUI_laserWarn.base.transform.colorTransform = colTBlue;
					GUI_laserWarn.tf_title.text = "LOW TRACTOR";
				}
				if (tractLastUsed)
					guiMan.updateLaser(tractorCharge, colTBlue);
			}
			// if tractor is not on
			else if (!rightDown && tractorCharge < 100)
			{
				tractorCharge += tractorRecharge;
				if (tractorCharge >= 100)
					tractorCharge = 100;
				if (tractLastUsed)
					guiMan.updateLaser(tractorCharge, colTBlue);
			}		
		
			// only continue if valid state
			if (!rightDown || tractTgt || tractInterrupt || tractorCool) return;
			var t:Point;
			
			for each (var man:Manager in managerC)
				for each (var f:Floater in man.getVec())
					if (f.collidable && f.hitTestPoint(mPt.x, mPt.y, true))
					{
						tractTgt = f;

						t = tractTgt.localToGlobal(new Point());
						grabX = mPt.x - t.x;
						grabY = mPt.y - t.y;
						
						tractTgt.isTractorTgt = true;
						return;
					}
		}
		
		public function changeHP(n:Number, f:Floater):void
		{
			f.changeHP(n);
			if (f.SPID == "hq")
			{
				guiMan.warningWarn("Home taking damage!", 0xFF0000, true);
				guiMan.updateHealth(str_CC.hp);
				ui_redFlash.gotoAndPlay(2);

				ccDestroyed();
			}
			if (f.ID == "structure")
			{
				if (f.SPID != "hq")
					guiMan.warningWarn("Base under attack!", 0xFF0000, true);
				var s:Structure = f as Structure;
				s.playAlert();
			}
		}
		
		public function ccDestroyed():void
		{
			//if (!failure.visible && str_CC.hp <= 0)
			if (!outro.visible && str_CC.hp <= 0)
			{
				if (limitTimer)
				{
					limitTimer.removeEventListener(TimerEvent.TIMER, limitTimerTick);
					limitTimer.stop();
				}

				addExplosion(str_CC.x, str_CC.y, 8);
				str_CC.visible = false;
				
				if (scen.isEndless)
					outro.endless = true;
				outro.gotoAndPlay(2);
				outro.visible = true;
				outro.score = score;
				outro.dScore = dScore;
				outro.minerals = dMineral;
				outro.success = false;
			}
		}
		
		public function getTime():String
		{
			timeMin = int(timeLeft / 60000);
			timeSec = int((timeLeft - timeMin * 60000) * .001);
			timeMSec = int((timeLeft - timeMin * 60000 - timeSec * 1000) * .1);
			return timeMin + ":" +
				   (timeSec < 10 ? "0" : "" ) + timeSec + "." +
				   (timeMSec < 10 ? "0" : "" ) + timeMSec;
		}
		
		public function stopTime():void
		{
			limitTimer.removeEventListener(TimerEvent.TIMER, limitTimerTick);
			limitTimer.stop();
		}
		
		// changes a mineral (ind) by an amount (amt)
		public function changeMineral(ind:int, amt:int):void
		{
			if (amt > 0)
				amt = int(amt * minBonus);
	
			mineralVec[ind] += amt;
			if (mineralVec[ind] < 0) mineralVec[ind] = 0;
			else if (mineralVec[ind] > 100) mineralVec[ind] = 100;
			guiMan.updateMineral(ind, mineralVec[ind]);
			
			if (amt > 0 && scoreMode != "TIME")
			{
				changeScore(amt * 100);
				dMineral[ind] += amt;
			}
		}
		
		// adds a DebrisFX (non-interactive)
		// amt: number		xP/yP: location		type: graphic		randX/randY: random location
		public function addDebrisFX(amt:int, _xP:int, _yP:int, _type:int, randX:int = 50, randY:int = 50):void
		{
			var dfx:DebrisFX;
			for (var i:int = 0; i < amt; i++)
			{
				dfx = new DebrisFX(this, _xP + getRand(-randX, randX, false), _yP + getRand(-randY, randY, false),
								   getRand(-5, 5, false), getRand(-5, 5, false), _type);
				game.cont.addChild(dfx);
				objArr.push(dfx);
			}
		}
		
		// adds an FXExplosion at the given point
		public function addExplosion(_xP:int, _yP:int, scaleAmt:Number = 1):void
		{
			var expl:FXExplosion = new FXExplosion(this, _xP, _yP);
			expl.scaleX = expl.scaleY = .2 * scaleAmt;
			game.cont.addChild(expl);
			objArr.push(expl);
		}
		
		// adds a MC to parallax, with factor
		public function addParallax(mc:MovieClip, moveFactor:Number, _xP:Number, _yP:Number):void
		{
			parallaxArr.push(mc);
			parallaxAmt.push(moveFactor);
			game.back.addChild(mc);
			mc.x = _xP;
			mc.y = _yP;
			//trace(mc.x + " " + mc.y);
		}
		
		public function setTip(s:String):void
		{
			GUI_tip.base.tf_text.text = s;
			GUI_tip.gotoAndPlay(2);
		}

		private function destroy():void
		{
			Mouse.show();
			cursor.visible = false;
			
			var i:int;
			for (i = 0; i < listeners.length; i++)
				if (hasEventListener(listeners[i].type))
					removeEventListener(listeners[i].type, listeners[i].listener);
			listeners = [];
			limitTimer = null;
			resetStage();
		}
		
		public function resetStage():void
		{
			var mc:MovieClip;
			for each (mc in objArr)				// remove general objects
				if (game.cont.contains(mc))
					game.cont.removeChild(mc);
			for each (mc in parallaxArr)		// remove parallax objects
				if (game.back.contains(mc))
					game.back.removeChild(mc);
			parallaxArr = [];
			parallaxAmt = [];
			objArr = new Vector.<MovieClip>();
			for each (var m:Manager in managerV)
				m.destroy();
			//sndMan = null;
			isDeploying = false;
		}
		
		override public function addEventListener(type:String, listener:Function, useCapture:Boolean = false, priority:int = 0, useWeakReference:Boolean = false):void
		{
       		super.addEventListener(type, listener, useCapture, priority, useWeakReference);
       		listeners.push({type:type, listener:listener});
		}
		
		// much faster than abs
		private function abs(n:Number):Number
		{
			return n < 0 ? -n : n;
		}
 
		private function getRand(min:Number, max:Number, useInt:Boolean = true):Number   
		{  
			if (useInt)
				return (int(Math.random() * (max - min + 1)) + min);
			return (Math.random() * (max - min + 1)) + min;  
		} 
	}
}