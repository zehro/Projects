package enemy 
{
	import core.Controller;
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class ABST_Actor 
	{
		protected var controller:Controller;
		protected var index:uint;
		protected var difficulty:uint;			// 0-10
		
		public var room:String;
		protected var activityTimer:int;
		
		public function ABST_Actor(_controller:Controller, _index:uint, _difficulty:uint)
		{
			controller = _controller;
			index = _index;
			difficulty = _difficulty;
			
			room = "1a";
			resetTimer();
		}
		
		public function step():void
		{
			// -- override this function
		}
		
		protected function resetTimer():void
		{
			activityTimer = (11 - difficulty) * 30;
		}
		
		protected function moveRoom(roomNew:String):Boolean
		{
			// quit if destination is same as origin
			if (roomNew == room)
				return false;
				
			// quit if player is looking at same camera as destination or origin
			if (controller.cameraCurrent &&
				controller.cameraCurrentString == room ||
				controller.cameraCurrentString == roomNew)
			{
				// TODO additional logic here, ex:
				// moveBlockedByCamera();
				return false;
			}
			
			controller.moveRoom(index, room, roomNew);

			trace("Enemy " + index + " moved from " + room + " to " + roomNew + ".");
			room = roomNew;
			
			return true;
		}
		
		protected function getRand(min:Number = 0, max:Number = 1):Number   
		{  
			return (Math.random() * (max - min + 1)) + min;  
		} 
		
		protected function getChoice(arr:Array):String
		{
			return arr[int(Math.random() * arr.length)];
		}
	}
}