package taskevents 
{
	import core.Controller;
	import flash.display.MovieClip;
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class TaskServerEject extends TaskEvent 
	{
		private var tray:MovieClip;
		
		public function TaskServerEject(_controller:Controller, _taskTime:Number) 
		{
			super(_controller, "Close Tray01 in Server room.", _taskTime);
			tray = controller.cg.server.mc_tray0;
		}
		
		override protected function dispatchEvent():void 
		{
			tray.gotoAndStop(9);
		}
		
		override protected function checkEvent():int 
		{
			trace("TaskServerEject: " + tray.currentFrame);
			if (tray.currentFrame == 1)
			{
				controller.cg.lightFlickerCount = 90;
				return 1;
			}
			return -1;
		}
	}
}