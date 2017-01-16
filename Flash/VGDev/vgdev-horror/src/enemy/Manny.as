package enemy 
{
	import core.Controller;
	
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class Manny extends ABST_Actor 
	{
		
		public function Manny(_controller:Controller, _index:uint,  _difficulty:uint) 
		{
			super(_controller, _index, _difficulty);
		}
		
		override public function step():void
		{
			if (activityTimer > 0)
			{
				activityTimer--;
				/*if (activityTimer % 30 == 0)
					trace("Enemy " + index + " is waiting with " + (activityTimer / 30) + " second(s) left.");*/
			}
			else
			{
				var moved:Boolean = false;
				
				switch (room)
				{
					case "1a":
						moved = moveRoom(getChoice(["2", "10"]));
					break;
					case "1b":
						moved = moveRoom(getChoice(["1a", "6", "8a"]));
					break;
					case "2":
						moved = moveRoom(getChoice(["1a", "3", "4"]));
					break;
					case "3":
						moved = moveRoom("2");
					break;
					case "4":
						moved = moveRoom(getChoice(["2", "5"]));
					break;
					case "5":
						moved = moveRoom(getChoice(["4", "6"]));
					break;
					case "6":
						moved = moveRoom(getChoice(["1b", "5", "7b"]));
					break;
					case "7a":
						moved = moveRoom(getChoice(["7b", "8a", "8b"]));
					break;
					case "7b":
						moved = moveRoom(getChoice(["7a", "6"]));
					break;
					case "8a":
						moved = moveRoom(getChoice(["1b", "7a", "8b", "9", "10"]));
					break;
					case "8b":
						moved = moveRoom(getChoice(["8a", "7b"]));
					break;
					case "9":
						moved = moveRoom("8a");
					break;
					case "10":
						moved = moveRoom(getChoice(["1a", "8a"]));
					break;
				}
				if (moved)
					resetTimer();
			}
		}
	}
}