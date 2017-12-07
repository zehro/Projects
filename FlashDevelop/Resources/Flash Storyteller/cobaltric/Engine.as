package cobaltric
{	
	import flash.display.MovieClip;
	import CBstory.StoryExample;
	
	public class Engine extends MovieClip
	{
		public var soundMan:SoundManager;
		
		public function Engine()
		{
			soundMan = new SoundManager(this);
			
			new StoryExample(mc_storyteller);		// mc_storyteller is in Engine on stage
		}
	}
}
