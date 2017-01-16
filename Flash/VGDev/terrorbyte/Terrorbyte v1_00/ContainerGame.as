package 
{
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.ui.Keyboard;
	import flash.utils.Timer;
	import flash.events.TimerEvent;
	import flash.display.MovieClip;
	import flash.display.BitmapData;
	import flash.geom.Point;
	import props.*;
	import managers.*;
	import scenarios.*;

	public class ContainerGame extends Container
	{
		public var eng:Engine;
		
		// -- main
		private var listeners:Array = [];
		public var gameMan:GameManager;
		public var planeMan:PlaneManager;
		public var missMan:MissileManager;
		public var nodeMan:NodeManager;
		public var turrMan:TurretManager;
		public var objArr:Array = [];		// for generic un-interactive things
		
		// -- controls
		public var leftDown:Boolean, rightDown:Boolean;

		// -- timing
		public var limitTimer:Timer;
		public var timeLeft:Number;
		public var TICK:Number = 33, tick:Number = TICK;
		public var timeMin:int, timeSec:int, timeMSec:int;
		public var isPaused:Boolean;

		// -- other
		private var st:int = 0;		// use in loops
		private var scenario:Scenario;
		public var levelName:String;		// name as listed in Engine
		public var failed:Boolean;			// TRUE if a vital plane is shot down
		public var won:Boolean;
		public var awaitThreats:Boolean;	// becomes TRUE if all planes are safe but missiles are still in flight
		
		public var cnt:int;		// debugging
		
		public function ContainerGame(_eng:Engine, _scenario:Scenario)
		{
			super();
			eng = _eng;
			scenario = _scenario;
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			stage.addEventListener(MouseEvent.RIGHT_MOUSE_DOWN, onMRdown);
			stage.addEventListener(MouseEvent.CLICK, onMouse);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyboardDn);
			stage.addEventListener(KeyboardEvent.KEY_UP, onKeyboardUp);
			
			gameMan = new GameManager(this, guiGFX, GUI_top, GUI_bottom);
			planeMan = new PlaneManager(this, wayGFX);
			missMan = new MissileManager(this);
			nodeMan = new NodeManager(this);
			turrMan = new TurretManager(this, gameMan, nodeMan, planeMan, missMan);
			
			gui_pause.visible = false;
			missilesLive.visible = false;
			
			timeLeft = 0;

			limitTimer = new Timer(tick);
			limitTimer.addEventListener(TimerEvent.TIMER, limitTimerTick);
			
			stageClear.buttonMode = stageClear.mouseEnabled = false;
			mouseEnabled = buttonMode = false;
			
			scenario.loadScenario();
			
			levelIntro.base.tf_title.text = eng.chosenScenario;
			levelIntro.buttonMode = levelIntro.mouseEnabled = false;
			
			cursor.visible = false;
		}
		
		public function addExplosion(xP:int, yP:int, life:int = 90, dAlpha:Number = .02, scale:Number = 1):void
		{
			var fx:FXExplosion = new FXExplosion(xP, yP, life, dAlpha, scale);
			cont.addChild(fx);
			objArr.push(fx);
		}
		
		public function addHacking(mc:MovieClip):void
		{
			var fxHacking:MovieClip = new FXHacking();
			fxHacking.x = mc.x;
			fxHacking.y = mc.y;
			fxHacking.rotation = getRand(0, 360);
			fxHacking.base.ch.rotation = 360 - fxHacking.rotation;
			fxHacking.mouseEnabled = fxHacking.buttonMode = false;
			cont.addChild(fxHacking);
		}
		
		public function addHacked(mc:MovieClip):void
		{
			var fxHacked:MovieClip = new FXHacked();
			fxHacked.x = mc.x;
			fxHacked.y = mc.y;
			fxHacked.mouseEnabled = fxHacked.buttonMode = false;
			cont.addChild(fxHacked);
		}
		
		public function addTransferred(mc:MovieClip):void
		{
			var fxTransferred:MovieClip = new FXTransferred();
			fxTransferred.x = mc.x;
			fxTransferred.y = mc.y;
			fxTransferred.mouseEnabled = fxTransferred.buttonMode = false;
			cont.addChild(fxTransferred);
		}
		
		public function addReduce(mc:MovieClip):void
		{
			var fxReduce:MovieClip = new FXLowerResist();
			fxReduce.x = mc.x;
			fxReduce.y = mc.y;
			fxReduce.mouseEnabled = fxReduce.buttonMode = false;
			cont.addChild(fxReduce);
		}
		
		public function startScenario():void
		{
			GUI_bottom.btn_start.visible = false;
			GUI_bottom.btn_reset.visible = true;
			planeMan.setRunning(true);
			turrMan.setRunning(true);
			limitTimer.start();
		}
		
		public function clearScenario():void
		{
			if (awaitThreats || missMan.hasThreats())
			{
				if (!awaitThreats)
					missilesLive.gotoAndPlay(2);
				awaitThreats = true;
				return;
			}
			stageClear.play();
			eng.soundMan.playSound("SFX_complete");
			if (eng.willUnlock)
			{
				trace("GAME: Unlocking the next level! " + eng.prevPage + "," + eng.prevLevel);
				eng.cleared[eng.prevPage][eng.prevLevel] = 2;
				eng.unlockLevel();
			}
			//trace("GAME: Stopping timer.");
			//limitTimer.stop();
			won = true;
		}
		
		public function failScenario():void
		{
			if (awaitThreats)
			{
				missilesLive.gotoAndPlay("out");
				awaitThreats = false;
			}
			stageFail.play();
			//limitTimer.stop();
			failed = true;
		}
		
		// called by scenario cleared MC after success animation
		public function setStars(stars:MovieClip):void					// KILLED FEATURE
		{
			return;
		}
		
		// --		--		--		--		--		--		--		--		--		CONTROL
		private function onKeyboardDn(e:KeyboardEvent):void
		{
			switch (e.keyCode)
			{
				case Keyboard.P:
					if (!failed && !won)
					{
						isPaused = !isPaused;
						gui_pause.visible = isPaused;
						isPaused ? limitTimer.stop() : limitTimer.start();
					}
				break;
			}
		}
		
		private function onKeyboardUp(e:KeyboardEvent):void
		{
			switch (e.keyCode)
			{
				case Keyboard.Q: onMLup(null); break;
				case Keyboard.E: onMRup(null); break;
			}
		}
		
		private function doNothing(e:MouseEvent):void
		{
			// -- TEMPORARY
		}
		
		private function onMLdown(e:MouseEvent):void
		{
			leftDown = true;
		}
		
		private function onMLup(e:MouseEvent):void
		{
			leftDown = false;
			high.graphics.clear();
		}
		
		private function onMRdown(e:MouseEvent):void
		{
			gameMan.onRightMouse();
		}
		
		private function onMRup(e:MouseEvent):void
		{
			rightDown = false;
			high.graphics.clear();
		}
		
		protected function onMouse(e:MouseEvent):void
		{
			gameMan.onMouse();
		}
		
		// --		--		--		--		--		--		--		--		--		LOOPS
		
		public function setCompleted():void
		{
			completed = true;
			limitTimer.stop();
		}
		
		override public function step():Boolean
		{
			cursor.x = mouseX;
			cursor.y = mouseY;
			
			if (isPaused || completed) return completed;

			gameMan.updateGFX();
			planeMan.step(leftDown, rightDown, new Point(mouseX, mouseY));
			missMan.step(leftDown, rightDown, new Point(mouseX, mouseY));
			turrMan.step(leftDown, rightDown, new Point(mouseX, mouseY));

			for (st = objArr.length - 1; st >= 0; st--)
				if (objArr[st].step())
					objArr.splice(st, 1);
					
			if (awaitThreats && !missMan.hasThreats())
			{
				missilesLive.gotoAndPlay("out");
				awaitThreats = false;
				clearScenario();
			}
			
			return completed;
		}
		
		private function limitTimerTick(e:TimerEvent):void
		{
			timeLeft += tick;
			nodeMan.updateTime(tick);
			if (!failed && !won)
				gameMan.updateTime(getTime());
		}
		
		// --		--		--		--		--		--		--		--		--		HELPERS
		
		private function getTime():String
		{
			timeMin = Math.floor(timeLeft / 60000);
			timeSec = Math.floor((timeLeft - timeMin * 60000) * .001);
			timeMSec = Math.floor((timeLeft - timeMin * 60000 - timeSec * 1000) * .1);
			return timeMin + ":" +
				   (timeSec < 10 ? "0" : "" ) + timeSec + "." +
				   (timeMSec < 10 ? "0" : "" ) + timeMSec;
		}
		
		public function destroy():void
		{
			for (st = 0; st < listeners.length; st++)
				if (hasEventListener(listeners[st].type))
					removeEventListener(listeners[st].type, listeners[st].listener);
			listeners = null;
			// -- TODO kill managers, objArr
			planeMan.destroy();
			missMan.destroy();
			//nodeMan.destroy();
			//turrMan.destroy();
			
			for each (var m:MovieClip in objArr)
				if (cont.contains(m))
					cont.removeChild(m);
			objArr = null;
			
			completed = true;
			limitTimer.stop();
		}
		
		override public function addEventListener(type:String, listener:Function, useCapture:Boolean = false, priority:int = 0, useWeakReference:Boolean = false):void
		{
       		super.addEventListener(type, listener, useCapture, priority, useWeakReference);
       		listeners.push({type:type, listener:listener});
		}
		
		private function getRand(min:Number, max:Number, useInt:Boolean = true):Number   
		{  
			if (useInt)
				return (Math.floor(Math.random() * (max - min + 1)) + min);
			return (Math.random() * (max - min + 1)) + min;  
		} 
	}
}