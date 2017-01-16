package cobaltric
{	
	import deltabattery.Armory;
	import deltabattery.SoundPlayer;
	import deltabattery.managers.ABST_Manager;
	import deltabattery.managers.AutoPlayer;
	import deltabattery.managers.ManagerArtillery;
	import deltabattery.managers.ManagerBullet;
	import deltabattery.managers.ManagerMissile;
	import deltabattery.managers.ManagerExplosion;
	import deltabattery.managers.ManagerParticle;
	import deltabattery.managers.ManagerWave;
	import deltabattery.projectiles.ABST_Bullet;
	import deltabattery.projectiles.ABST_Missile;
	import deltabattery.Turret;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	import flash.ui.Mouse;
	
	/**
	 * Primary game container and controller
	 * @author Alexander Huynh
	 */
	public class ContainerGame extends ABST_Container
	{		
		public var engine:Engine;
		public var game:Game;				// the Game SWC, containing all the base assets
		public var turret:Turret;			// the player's turret
		public var turretGrace:int;			// if not 0, don't let things fire (i.e. right after game starts)
		
		public var gameActive:Boolean;		// TRUE if game is active (ex: unpaused)
		public var prevActive:Boolean;		// helper to remember gameActive while paused
		public var paused:Boolean;			// TRUE if pause screen is up
		
		public var tutorialFlag:Boolean;	// TRUE if day 1 chosen and tutorial didn't show
		public var infoFlag:Boolean;		// TRUE if info box at new wave needs to show
		
		private var managers:Array;			// array of all managers
		private var manLen:int;				// length of the manager array
		public var manPart:ManagerParticle;	// managers for specific ABSTract classes
		public var manWave:ManagerWave;
		public var manMiss:ManagerMissile;
		public var manArty:ManagerArtillery;
		public var manBull:ManagerBullet;
		public var manExpl:ManagerExplosion;
		
		public var money:int = 0;			// actual money
		private var moneyTotal:int = 0;		// total earned money - for scoring
		private var moneyDisplay:int;		// displayed money (for 'increasing' slack effect)
		private const MONEY_DELTA:int = 11;	// rate to change displayed money
		
		public var cityHP:Number;			// HP of city, 0 is destroyed (uses primary HP bar)
		public var cityHPMax:Number;		// max HP of city
		
		private var cityHPSlack:Number;		// displayed HP of city (uses secondary 'ghost' HP bar)
		private var cityHPCounter:int;		// delay before slack decreases
		private var cityExplode:int = 0;	// explode city when it dies
		
		private var intermission:int;		// counter for in-between waves
		
		//private var ai:AutoPlayer;		// TEMP AutoPlayer
		public var mx:Number = 0;
		public var my:Number = 0;
		
		private var shop:MovieClip;			// reference to the shop MovieClip
		public var armory:Armory;
		public var newWepFlag:int = 0;		// 0 not shown; 1 to be shown; 2 shown
		
		private var startWave:int = 1;
		private var headStart:int;
		
		public var cursor:MovieClip;
		private var win:Boolean = false;
		private var dayOffset:Boolean = true;
	
		public function ContainerGame(eng:Engine, _startWave:int = 1)
		{
			super();
			engine = eng;
			startWave = _startWave;
			addEventListener(Event.ADDED_TO_STAGE, init);
			
			headStart = startWave == 1 ? 0 : 1;
		}
		
		/*	Sets up the game.
		 * 	Called after this Container is added to the stage.
		 */
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			addEventListener(Event.REMOVED_FROM_STAGE, destroy);
			//addEventListener(MouseEvent.MOUSE_MOVE, updateMouse);
			
			// disable right click menu
			stage.showDefaultContextMenu = false;
	
			// setup the Game SWC
			game = new Game();
			addChild(game);
			game.x -= 129;
			game.y -= 446;
			game.mc_gui.mc_statusCenter.visible = false;
			game.mc_gui.mc_statusHuge.visible = false;
			game.bg.cacheAsBitmap = true;
			
			// pause menu
			game.mc_pause.visible = false;
			game.mc_pause.btn_resume.addEventListener(MouseEvent.CLICK, onPause);
			game.mc_pause.btn_quit.addEventListener(MouseEvent.CLICK, onQuit);
			game.mc_gui.btn_pause.addEventListener(MouseEvent.CLICK, onPause);
			game.mc_gui.btn_help.addEventListener(MouseEvent.CLICK, onHelp);
			
			game.mc_pause.btn_resume.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_pause.btn_quit.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_gui.btn_pause.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_gui.btn_help.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// game over menus
			game.mc_lose.visible = false;
			game.mc_lose.btn_quit.addEventListener(MouseEvent.CLICK, onQuit);
			game.mc_lose.btn_quit.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_win.visible = false;
			game.mc_win.btn_quit.addEventListener(MouseEvent.CLICK, onQuit);
			game.mc_win.btn_quit.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// text
			game.mc_gui.tf_wave.text = startWave;
			
			// initialize managers
			manPart = new ManagerParticle(this);
			manMiss = new ManagerMissile(this);
			manArty = new ManagerArtillery(this);
			manBull = new ManagerBullet(this);
			manExpl = new ManagerExplosion(this);
			manWave = new ManagerWave(this, startWave);
			managers = [manPart, manWave, manArty, manMiss, manBull, manExpl];
			manLen = managers.length - 1;
			
			// setup the Armory
			armory = new Armory(this);
			game.mc_gui.shop.mc_tutorial.btn_skip.addEventListener(MouseEvent.CLICK, armAck);
			game.mc_gui.shop.mc_tutorial.btn_skip.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_gui.shop.mc_tutorial.btn_next.addEventListener(MouseEvent.CLICK, onButton);
			game.mc_gui.shop.mc_tutorial.btn_next.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_gui.shop.mc_tutorial.btn_resume.addEventListener(MouseEvent.CLICK, armAck);
			game.mc_gui.shop.mc_tutorial.btn_resume.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// setup the Turret
			turret = new Turret(this, game.mc_turret);
			turret.turret.mc_cannon.deltaStrike.visible = false;
			turret.upgradeAll()
			
			// setup city
			cityHP = cityHPMax = cityHPSlack = 100;
			game.city.cacheAsBitmap = true;
			
			// setup autoplayer
			//ai = new AutoPlayer(this, manMiss);
			
			// setup shop
			shop = game.mc_gui.shop;
			shop.visible = false;
			shop.btn_nextDay.addEventListener(MouseEvent.CLICK, onShopDone);
			shop.btn_nextDay.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// setup tutorial
			tutorialFlag = true;
			gameActive = false;
			game.mc_gui.tutorial.gotoAndPlay("in");
			game.mc_gui.newWeapon.visible = false;
			game.mc_gui.newWeapon.stop();
			game.mc_gui.newWeapon.mc.btn_resume.addEventListener(MouseEvent.CLICK, wepAck);
			game.mc_gui.newWeapon.mc.btn_resume.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// (attach sound events)
			game.mc_gui.tutorial.mc.btn_start.addEventListener(MouseEvent.CLICK, onButton);
			game.mc_gui.tutorial.mc.btn_start.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_gui.tutorial.mc.btn_skip.addEventListener(MouseEvent.CLICK, onButton);
			game.mc_gui.tutorial.mc.btn_skip.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			game.mc_gui.tutorial.mc.btn_next.addEventListener(MouseEvent.CLICK, onButton);
			game.mc_gui.tutorial.mc.btn_next.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// new wave indicator
			infoFlag = true;
			game.mc_gui.newEnemy.mc.gotoAndStop(manWave.wave >= 31 ? 31 : manWave.wave);
			game.mc_gui.newEnemy.mc.btn_start.addEventListener(MouseEvent.CLICK, infoAck);
			game.mc_gui.newEnemy.mc.btn_start.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			// cursor
			cursor = new GameCursor();
			game.mc_gui.addChild(cursor);
			cursor.visible = false;
		}
		
		// called by Engine every frame
		override public function step():Boolean
		{
			updateMoney();
			
			cursor.x = mouseX - game.x - game.mc_gui.x;
			cursor.y = mouseY - game.y - game.mc_gui.y;
			
			if (tutorialFlag)
			{
				if (game.mc_gui.tutorial.mc.currentFrame == game.mc_gui.tutorial.mc.totalFrames)
				{
					game.mc_gui.tutorial.gotoAndPlay("out");
					tutorialFlag = false;
					
					if (headStart == 1)
					{
						headStart = 2;
						
						addMoney(startWave == 8 ? 10000 : 40000)
						game.mc_gui.shop.visible = true;
						game.mc_gui.mc_statusHuge.visible = false;
						gameActive = false;
						
						armory.arm.repairGroup.tf_repair.text = "$100 x " + manWave.wave + " = $" + (100 * manWave.wave);
						armory.arm.repairGroup.visible = manWave.wave % 3 == 0;
						
						Mouse.show();
						cursor.visible = false;
					}
					else
						game.mc_gui.newEnemy.gotoAndPlay("in");
				}
				return false;
			}
			else if (infoFlag)
			{
				gameActive = false;
				Mouse.show();
				cursor.visible = false;
				return false;
			}
			else if (newWepFlag == 2)
			{
				gameActive = false;
				Mouse.show();
				cursor.visible = false;
				return false;
			}
			
			if (turretGrace > 0)
				turretGrace--;
			
			// delay between wave success and showing the shop
			if (intermission > 0)
			{
				if (--intermission == 0)
				{
					game.mc_gui.shop.visible = true;
					game.mc_gui.mc_statusHuge.visible = false;
					Mouse.show();
					cursor.visible = false;
					turret.reloadAll();
				}
			}
			
			updateHP();
			
			if (!gameActive) return completed;
			
			if (cityExplode > 2)
				if (--cityExplode % 3 == 0)
					explodeCity();
			
			mx = mouseX - game.x;
			my = mouseY - game.y;
			
			turret.step();		// update the Turret
			//ai.step();
			
			// update each manager
			for (var i:int = manLen; i >= 0; i--)
				managers[i].step();
			
			return completed;
		}
		
		private function infoAck(e:MouseEvent):void
		{
			game.mc_gui.newEnemy.gotoAndPlay("out");
			infoFlag = false;
			gameActive = true;
			SoundPlayer.play("sfx_menu_blip");
			Mouse.hide();
			cursor.visible = true;
		}
		
		private function wepAck(e:MouseEvent):void
		{
			game.mc_gui.newWeapon.gotoAndPlay("out");
			newWepFlag = 3;
			gameActive = true;
			SoundPlayer.play("sfx_menu_blip");
			Mouse.hide();
			cursor.visible = true;
		}
		
		private function armAck(e:MouseEvent):void
		{
			game.mc_gui.shop.mc_tutorial.visible = false;
			game.mc_gui.shop.mc_tutorial.buttonMode = false;
			game.mc_gui.shop.mc_tutorial.mouseChildren = false;
			game.mc_gui.shop.mc_tutorial.mouseEnabled = false;
			SoundPlayer.play("sfx_menu_blip");
		}
		
		// updates city's ghost HP bar
		private function updateHP():void
		{
			if (cityHPSlack == cityHP) return;
			
			if (cityHPCounter > 0)
			{
				cityHPCounter--;
				return;
			}
			
			if (--cityHPSlack < cityHP)
				cityHPSlack = cityHP;

			var life:Number = cityHPSlack / cityHPMax;
			game.mc_gui.mc_health.secondary.x = 74.75 - (1 - life) * 149.5;		// update health bar (secondary)
		}
		
		/**	Damage the city by m's damage.
		 * 	Called by an ABST_Missile.
		 * 
		 * 	@m		The missile that is damaging the city
		 */
		public function damageCity(param:*):void
		{
			var life:Number;
			if (param is ABST_Missile || param is ABST_Bullet)
			{
				//trace("Damage: " + param.damage);
				cityHP -= param.damage;
				cityHPCounter = 30;		// reset the slack counter
			
				life = cityHP / cityHPMax;
				game.mc_gui.mc_health.primary.x = 74.75 - (1 - life) * 149.5;		// update health bar (primary)
			}
			
			if (cityHP <= 0 && cityExplode == 0)
			{
				cityHP = 0;
				game.city.gotoAndStop(game.city.totalFrames);
				cityExplode = 75;
				SoundPlayer.playBGM(false);

				manWave.enemiesRemaining = 0;
				for (var i:int = 0; i < 7 + getRand(0, 3); i++)
					explodeCity();
			}
			else		// update the visual state of the city
			{
				if (life < .30)
					game.city.gotoAndStop(4);
				else if (life < .5)
					game.city.gotoAndStop(3);
				else if (life < .85)
					game.city.gotoAndStop(2);
				else
					game.city.gotoAndStop(1);
			}
		}
		
		private function explodeCity():void
		{
			manExpl.spawnExplosion(new Point(game.city.x + getRand( -100, 100), game.city.y + getRand( -50, 50)), 1, getRand(.5, 2));
			manPart.spawnParticle("smoke", new Point(game.city.x + getRand( -100, 100), game.city.y + getRand( -50, 50)), getRand(0, 360),
								  getRand( -5, 5), getRand( -2, 2));
		}
		
		// called by the "NEXT WAVE" button in the shop
		private function onShopDone(e:MouseEvent):void
		{
			manWave.startWave();
			game.mc_gui.shop.visible = false;
			infoFlag = true;
			game.mc_gui.newEnemy.gotoAndPlay("in");
			game.mc_gui.newEnemy.mc.gotoAndStop(manWave.wave >= 31 ? 31 : manWave.wave);
			game.bg.cacheAsBitmap = true;
			if (newWepFlag == 1)
			{
				newWepFlag = 2;
				game.mc_gui.newWeapon.visible = true;
				game.mc_gui.newWeapon.gotoAndPlay(1);
			}
			else
			{
				Mouse.hide();
				cursor.visible = true;
			}
			SoundPlayer.play("sfx_menu_blip");
			cursor.gotoAndStop(1);
			dayOffset = true;
			turret.reloadAll();
		}
		
		// end the current wave, enabling the shop, etc.
		// called after the intermission finishes
		public function endWave():void
		{
			gameActive = false;
			dayOffset = false;
			manPart.clear();

			if (cityHP <= 0)
			{
				game.mc_lose.visible = true;
				game.mc_lose.tf_day.text = "Day " + (manWave.wave - 1);
				Mouse.show();
				cursor.visible = false;
				return;
			}
			else if (manWave.wave == 31)
			{
				game.mc_win.visible = true;
				game.mc_win.tf_day.text = "Day " + (manWave.wave - 1);
				Mouse.show();
				cursor.visible = false;
				win = true;
				return;
			}

			game.mc_gui.tf_wave.text = manWave.wave;
			game.mc_gui.mc_statusHuge.visible = true;
			game.mc_gui.mc_statusHuge.tf_statusHuge.text = "Day " + (manWave.wave - 1) + " complete!";
			intermission = 60;
			
			armory.arm.repairGroup.tf_repair.text = "$100 x " + manWave.wave + " = $" + (100 * manWave.wave);
			armory.arm.repairGroup.visible = manWave.wave % 3 == 0;
		}
		
		private function onPause(e:MouseEvent):void
		{
			paused = !paused;
			turretGrace = 10;
			if (paused)
			{
				prevActive = gameActive;
				gameActive = false;
				Mouse.show();
				cursor.visible = false;
			}
			else
			{
				gameActive = prevActive;
				Mouse.hide();
				cursor.visible = true;
			}

			game.mc_pause.visible = paused;
			SoundPlayer.play("sfx_menu_blip");
		}
		
		private function onHelp(e:MouseEvent):void
		{
			turretGrace = 10;
			tutorialFlag = true;
			game.mc_gui.tutorial.gotoAndPlay("in");
			game.mc_gui.tutorial.mc.gotoAndStop(1);
			gameActive = false;
			SoundPlayer.play("sfx_menu_blip");
			Mouse.show();
			cursor.visible = false;
		}
		
		private function onQuit(e:MouseEvent):void
		{
			turretGrace = 10;
			turret.destroy(null);
			tutorialFlag = infoFlag = false;
			completed = true;
			SoundPlayer.play("sfx_menu_blip");
			Mouse.show();
			cursor.visible = false;
			
			engine.scoreArr = [(win ? 99 : manWave.wave - 1) + (dayOffset ? 1 : 0), moneyTotal];
		}
		
		// updates the displayed money to match the actual money
		private function updateMoney():void
		{
			if (moneyDisplay == money)
				return;
			var delta:int = moneyDisplay - money;
			if (Math.abs(delta) < MONEY_DELTA)
				moneyDisplay = money;
			else		// change money display faster as difference grows
				moneyDisplay += (delta > 0 ? -1 : 1) * (Math.abs(delta) < 400 ? MONEY_DELTA : .5 * Math.abs(delta) - 170);
			game.mc_gui.tf_money.text = moneyDisplay;
		}
		
		// changes the money by amount
		public function addMoney(amount:int):Boolean
		{
			if (amount > 0)
				moneyTotal += amount;
			if (money + amount < 0)
				return false;
			money += amount;
			return true;
		}
		
		// called when the mouse is moved
		/*protected function updateMouse(e:MouseEvent):void
		{
			if (!gameActive) return;

			mx = mouseX - game.x;
			my = mouseY - game.y;
			turret.updateMouse();			
		}*/
		
		// get a list of all live projectiles
		public function getProjectileArray():Array
		{
			var a:Array = [];
			return a.concat(manMiss.objArr).concat(manArty.objArr);
		}
		
		/*
		public function overrideMouse(tgtX:Number, tgtY:Number):void
		{
			mx = tgtX;
			my = tgtY;
			turret.updateMouse();	
		}*/
		
		protected function getRand(min:Number = 0, max:Number = 1):Number   
		{  
			return (Math.random() * (max - min + 1)) + min;  
		} 
		
		private function overButton(e:MouseEvent):void
		{
			SoundPlayer.play("sfx_menu_blip_over");
		}
		
		private function onButton(e:MouseEvent):void
		{
			SoundPlayer.play("sfx_menu_blip");
		}
		
		private function destroy(e:Event):void
		{
			for (var i:int = manLen; i >= 0; i--)
				managers[i].destroy();
			removeEventListener(Event.REMOVED_FROM_STAGE, destroy);
			//removeEventListener(MouseEvent.MOUSE_MOVE, updateMouse);
			Mouse.show();
		}
	}
}
