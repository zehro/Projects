// holds game contents (menu, cutscene, game, etc.)

package 
{
	import flash.display.MovieClip;
	public class Container extends MovieClip
	{
		protected var completed:Boolean;
		
		public function Container()
		{
			completed = false;
		}
		
		// returns TRUE to Engine when this container is finished
		public function step():Boolean
		{
			// -- override this function
			return completed;
		}
	}
}
