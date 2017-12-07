package cobaltric
{
	import flash.display.MovieClip;
	
	/**
	 * ABSTRACT CLASS - Do not instantiate
	 * @author Alexander Huynh
	 */
	public class ABST_Container extends MovieClip
	{
		protected var completed:Boolean;
		
		public function ABST_Container()
		{
			completed = false;
		}
		
		public function step():Boolean
		{
			return completed;
		}
	}
}
