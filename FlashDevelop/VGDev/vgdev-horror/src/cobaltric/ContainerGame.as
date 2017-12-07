package cobaltric
{	
	import core.Controller;
	import flash.display.Bitmap;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.text.TextField;
	import flash.text.TextFormat;
	
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class ContainerGame extends ABST_Container
	{
		private var controller:Controller;
		
		// base MovieClips
		public var office:Office;					// primary view
		public var server:Server;					// left room view
		
		public var overlayCamera:OverlayCamera;		// cameras
		public var overlayTask:OverlayTask;			// task computer
		public var overlayDark:OverlayDark;			// transparent dark overlay
		
		public var lightFlickerCount:int;
		
		// actual MovieClip instances
		public var doors:Array;			// door buttons
		public var shutters:Array;		// shutters
		
		public var lights:Array;		// light buttons
		public var lightCovers:Array;	// light MCs
		
		public var cameras:Array;		// camera buttons
		public var camerasMap:Object;	// string -> camera MC
		
		public function ContainerGame()
		{
			super();
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			
			server = new Server();
			addChild(server);
			server.x = 400;
			server.y = 300;
			server.visible = false;
			
			office = new Office();
			addChild(office);
			office.x = 400;
			office.y = 300;
			
			overlayDark = new OverlayDark();
			addChild(overlayDark);
			overlayDark.x = 400;
			overlayDark.y = 300;
			overlayDark.alpha = .5;
			overlayDark.buttonMode = false;
			overlayDark.mouseEnabled = false;
			
			overlayCamera = new OverlayCamera();
			addChild(overlayCamera);
			overlayCamera.x = 400;
			overlayCamera.y = 300;
			overlayCamera.visible = false;
			overlayCamera.cam_1a.gotoAndStop("1a_main");
			
			overlayTask = new OverlayTask();
			addChild(overlayTask);
			overlayTask.x = 400;
			overlayTask.y = 300;
			overlayTask.visible = false;
			
			var maskAll:MaskAll = new MaskAll();
			addChild(maskAll);
			maskAll.x = 400;
			maskAll.y = 300;
			
			controller = new Controller(this);
			
			var mc:MovieClip;
			var i:uint;
			
			doors = [office.btn_doorL, office.btn_doorR];
			for (i = 0; i < 2; i++)
			{
				mc = doors[i];	
				mc.ind = i;			
				doors.push(mc);				
				mc.addEventListener(MouseEvent.CLICK, controller.onShutter);
			}
			
			lights = [office.btn_lightL, office.btn_lightR];
			for (i = 0; i < 2; i++)
			{
				mc = lights[i];
				mc.ind = i;
				lights.push(mc);
				mc.addEventListener(MouseEvent.MOUSE_DOWN, controller.onLight);
			}
			
			lightCovers = [office.lightL, office.lightR];
			
			cameras = [overlayCamera.cam_1a, overlayCamera.cam_1b, overlayCamera.cam_2, overlayCamera.cam_3,
					   overlayCamera.cam_4, overlayCamera.cam_5, overlayCamera.cam_6, overlayCamera.cam_7a,
					   overlayCamera.cam_7b, overlayCamera.cam_8a, overlayCamera.cam_8b, overlayCamera.cam_9,
					   overlayCamera.cam_10];
			camerasMap = new Object();
			var keys:Array = ["1a", "1b", "2", "3", "4", "5", "6", "7a", "7b", "8a", "8b", "9", "10"];
			for (i = 0; i < cameras.length; i++)
			{
				cameras[i].addEventListener(MouseEvent.MOUSE_DOWN, controller.onCamera);
				camerasMap[keys[i]] = cameras[i];
			}
			
			shutters = [office.shutterL, office.shutterR];
			
			overlayCamera.btn_camOff.addEventListener(MouseEvent.MOUSE_DOWN, controller.onCameraOff);
			overlayTask.btn_camOff.addEventListener(MouseEvent.MOUSE_DOWN, controller.onTaskOff);		// TODO change button name
			
			office.mc_camera.addEventListener(MouseEvent.CLICK, controller.onCameraMain);
			office.mc_laptop.addEventListener(MouseEvent.CLICK, controller.onTask);
			office.hs_doorL.addEventListener(MouseEvent.CLICK, officeToServer);
			
			server.hs_door.addEventListener(MouseEvent.CLICK, serverToOffice);
			
			stage.addEventListener(MouseEvent.MOUSE_UP, controller.mouseUp);
		}
		
		private function officeToServer(e:MouseEvent):void
		{
			office.visible = false;
			server.visible = true;
		}
		
		private function serverToOffice(e:MouseEvent):void
		{
			office.visible = true;
			server.visible = false;
		}
		
		override public function step():Boolean
		{
			controller.step();
			
			office.mc_displayPower.tf_fuel.text = "Fuel: " + controller.fuel.toFixed(0) + "%";
			office.mc_displayPower.tf_usage.text = "Load: " + controller.fuelDrainMultiplier + "x";
			
			overlayTask.tf_progress.text = ((controller.task / controller.taskGoal) * 100).toFixed(0) + "%";
			
			if (lightFlickerCount > 0)
			{
				if (--lightFlickerCount == 0)
					overlayDark.alpha = .5;			// TODO use const
				else
					flickerLights();
			}
			
			return false;
		}
		
		private function flickerLights():void
		{
			overlayDark.alpha = getRand(0.2, 0.6);
		}
		
		protected function getRand(min:Number = 0, max:Number = 1):Number   
		{  
			return (Math.random() * (max - min + 1)) + min;  
		} 
		
	}
}
