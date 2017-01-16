package CBstory
{
	import cobaltric.Storyteller;
	import cobaltric.Dialogue;
	import flash.display.MovieClip;

	public class Story
	{
		protected var st:MovieClip;
		
		/**	Story
		 *	Takes in a StoryTeller and initializes it
		 */
		public function Story(_st:MovieClip)
		{
			st = _st;
			init();
			st.nextSay();
		}
		
		protected function init():void
		{
			// -- OVERRIDE THIS FUNCTION
		}
	}
}