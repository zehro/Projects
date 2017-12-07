package core 
{
	import cobaltric.ABST_Container;
	import cobaltric.ContainerGame;
	import enemy.Manny;
	import flash.display.MovieClip;
	import flash.events.MouseEvent;
	import taskevents.TaskEvent;
	import taskevents.TaskServerEject;
	
	/**
	 * Controls core mechanics
	 * @author Alexander Huynh
	 */
	public class Controller 
	{
		public var cg:ContainerGame;
		public var roomMap:Object;					// map of camera numbers to boolean arrays of length 4
		private const NUM_ENEMIES:uint = 3;
		
		private var camMoveRate:Number = 14;				// pixels per frame to move the camera
		
		// fuel usage
		public var fuel:Number;						// current generator fuel level
		public var fuelDrainRate:Number;			// base drain per step
		public var fuelDrainMultiplier:int;			// usage multiplier
		public var fuelDrainMultiplierLimit:int;	// max usage multiplier (min is 1)
		
		// task progress
		public var task:Number;						// current task progress
		public var taskWorkRate:Number;				// base progress per step
		public var taskGoal:Number;					// when task is 100% complete
		public var taskCompleted:Boolean;
		
		// shutters
		public var shutters:Array;					// boolean array indicating shutter state (T:closed, F:open)
		//public var shuttersStatus:Array;			// ??? array indicating shutter abnormalities
		
		// lights
		public var lights:Array;					// boolean array indicating light state (T:on, F:off)
		
		// cameras
		public var cameraCurrent:MovieClip;			// currently-selected camera (MovieClip)
		public var cameraCurrentString:String;		// currently-selected camera (String)
		
		// enemies
		public var enemies:Array;
		
		// task events
		public var taskIndex:uint;
		public var taskEvents:Array;
		
		/**
		 * Game controller requires reference to containing ContainerGame
		 *
		 * @param	_cg		The containing ContainerGame
		 */
		public function Controller(_cg:ContainerGame) 
		{
			cg = _cg;
			
			// setup room map
			roomMap = new Object();
			roomMap["1a"] = makeArr(NUM_ENEMIES);
			roomMap["1b"] = makeArr(NUM_ENEMIES);
			roomMap["2"] = makeArr(NUM_ENEMIES);
			roomMap["3"] = makeArr(NUM_ENEMIES);
			roomMap["4"] = makeArr(NUM_ENEMIES);
			roomMap["5"] = makeArr(NUM_ENEMIES);
			roomMap["6"] = makeArr(NUM_ENEMIES);
			roomMap["7a"] = makeArr(NUM_ENEMIES);
			roomMap["7b"] = makeArr(NUM_ENEMIES);
			roomMap["8a"] = makeArr(NUM_ENEMIES);
			roomMap["8b"] = makeArr(NUM_ENEMIES);
			roomMap["9"] = makeArr(NUM_ENEMIES);
			roomMap["10"] = makeArr(NUM_ENEMIES);

			// setup fuel
			fuel = 100;
			fuelDrainRate = 0.007;
			fuelDrainMultiplier = 1;
			fuelDrainMultiplierLimit = 5;
			
			// setup task
			task = 0;
			taskGoal = 3600;		// 2 minutes
			taskWorkRate = 1;		// 30 progress per sec
			
			// setup camera
			/*cameraCurrentString = "1a";
			cameraCurrent = cg.overlayCamera.cam_1a;*/

			// setup shutters
			shutters = makeArr(2);
			
			// setup lights
			lights = makeArr(2);
			
			// -- TODO put the following into something extending this controller
			
			// setup enemies
			enemies = [new Manny(this, 0, 1)];
			
			// setup task events
			taskIndex = 0;
			taskEvents = [new TaskServerEject(this, 30 * 3), new TaskEvent(this, "", taskGoal + 1)];
		}
		
		private function makeArr(size:uint):Array
		{
			var arr:Array = [];
			for (var i:uint = 0; i < size; i++)
				arr.push(false);
			return arr;
		}
		
		public function step():void
		{
			var len:uint = enemies.length;
			for (var i:uint = 0; i < len; i++)
				enemies[i].step();
			
			updateFuel();
			updateCamera();
			
			if (!taskCompleted)
			{
				var taskStatus:int = taskEvents[taskIndex].fireEvent(task);

				if (taskStatus == 1)
				{
					cg.overlayTask.tf_status.text = "System Repair Wizard running...";
					cg.overlayTask.status.gotoAndStop(1);
					if (++taskIndex == taskEvents.length)
						taskCompleted = true;
				}
				else if (taskStatus == 0)
					updateTask();
			}
		}

		public function moveRoom(index:uint, roomCurr:String, roomNew:String):void
		{
			roomMap[roomCurr][index] = false;
			roomMap[roomNew][index] = true;
			
			// TODO update visuals
			cg.overlayCamera.locator0.x = cg.camerasMap[roomNew].x;
			cg.overlayCamera.locator0.y = cg.camerasMap[roomNew].y;
		}
		
		public function onShutter(e:MouseEvent):void
		{
			if (cg.shutters[e.target.ind].currentLabel != "closed" &&
				cg.shutters[e.target.ind].currentLabel != "opened")
				return;
			
			shutters[e.target.ind] = !shutters[e.target.ind];
			
			if (shutters[e.target.ind])
			{
				e.target.gotoAndStop("greenNorm");
				fuelDrainMultiplier++;
				
				cg.shutters[e.target.ind].gotoAndPlay("close");
			}
			else
			{
				e.target.gotoAndStop("redNorm");
				fuelDrainMultiplier--;
				
				cg.shutters[e.target.ind].gotoAndPlay("open");
			}
		}
		
		public function onLight(e:MouseEvent):void
		{
			if (!lights[e.target.ind])
				fuelDrainMultiplier++;
			
			lights[e.target.ind] = true;
			cg.lightCovers[e.target.ind].visible = false;
			
			e.target.gotoAndStop("lightOn");
		}
		
		public function mouseUp(e:MouseEvent):void
		{
			var len:uint = lights.length;
			
			for (var i:uint = 0; i < len; i++)
			{
				if (lights[i])
					fuelDrainMultiplier--;
				lights[i] = false;
				cg.lights[i].gotoAndStop("lightOff");
				cg.lightCovers[i].visible = true;
			}
		}
		
		public function onCameraMain(e:MouseEvent):void
		{
			cg.overlayCamera.visible = true;
		}

		public function onCamera(e:MouseEvent):void
		{			
			if (cameraCurrent)
			{
				cameraCurrent.gotoAndStop("off");
			}
			else
			{
				//fuelDrainMultiplier++;
			}
	
			cameraCurrent = MovieClip(e.target);
			cameraCurrentString = cameraCurrent.name.substring(4);		// format is cam_XX
			cameraCurrent.gotoAndStop("on");

			cg.overlayCamera.rooms.gotoAndStop(cameraCurrentString + "_main");
			cg.overlayCamera.tf_name.text = cg.overlayCamera.rooms.roomName;
		}

		public function onCameraOff(e:MouseEvent):void
		{						
			if (cameraCurrent)
			{
				cameraCurrent.gotoAndStop("off");
				//fuelDrainMultiplier--;
			}
			//cameraCurrent = null;
			//cameraCurrentString = null;
			
			cg.overlayCamera.visible = false;
		}
		
		public function onTask(e:MouseEvent):void
		{									
			cg.overlayTask.visible = true;
		}
		
		public function onTaskOff(e:MouseEvent):void
		{									
			cg.overlayTask.visible = false;
		}
		
		/**
		 * Reduces the fuel by the drain rate * the drain multiplier
		 * 
		 * Returns true if out of fuel; false otherwise
		 */
		private function updateFuel():Boolean
		{
			fuel -= fuelDrainRate * fuelDrainMultiplier;
			if (fuel <= 0)
			{
				fuel = 0;
				return true;
			}
			return false;
		}
		
		private function updateTask():Boolean
		{
			task += taskWorkRate;
			if (task >= taskGoal)
			{
				task = taskGoal;
				return true;
			}
			return false;
		}
		
		private function updateCamera():void
		{
			if (!cg.office.visible)
				return;
			
			if (cg.mouseX > 700)
			{
				cg.office.x -= camMoveRate;
				if (cg.office.x < 300)
					cg.office.x = 300;
			}
			else if (cg.mouseX < 100)
			{
				cg.office.x += camMoveRate;
				if (cg.office.x > 500)
					cg.office.x = 500;
			}
		}
	}
}