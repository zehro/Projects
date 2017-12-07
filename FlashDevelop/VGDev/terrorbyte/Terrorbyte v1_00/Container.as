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
		
		public function step():Boolean
		{
			return completed;
		}
	}
}
