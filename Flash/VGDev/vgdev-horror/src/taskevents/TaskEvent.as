package taskevents 
{
	import core.Controller;
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class TaskEvent 
	{
		protected var controller:Controller;
		protected var fired:Boolean;
		
		public var taskName:String;		// the task's name
		public var taskTime:Number;		// when the task is triggered
		
		public function TaskEvent(_controller:Controller, _taskName:String, _taskTime:Number) 
		{
			controller = _controller;
			taskName = _taskName;
			taskTime = _taskTime;
			
			fired = false;
		}
		
		/**
		 * 
		 * 
		 * @param	time	Current progress
		 * @return	0 		if event hasn't fired
		 * 			-1		if event is waiting
		 * 			1		if event is complete
		 */
		public function fireEvent(time:Number):int
		{
			if (time < taskTime)
				return 0;
			
			if (!fired)
			{
				controller.cg.overlayTask.tf_status.text = taskName;
				controller.cg.overlayTask.status.gotoAndStop("problem");
				dispatchEvent();
				fired = true;
				return -1;
			}
			
			return checkEvent();
		}
		
		protected function dispatchEvent():void
		{
			// -- override this function
		}
		
		// return -1 if not complete; 1 otherwise
		protected function checkEvent():int
		{
			// -- override this function
			return 0;
		}
	}
}