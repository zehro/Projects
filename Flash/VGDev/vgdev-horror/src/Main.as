package 
{
	import flash.display.Sprite;
	import flash.events.Event;
	import cobaltric.Engine;

	/**
	 * ...
	 * @author Alexander Huynh
	 */
	[Frame(factoryClass = "Preloader")]
	[SWF(width="800", height="600", backgroundColor="#EAEAEA", frameRate="30")]
	public class Main extends Sprite 
	{
		public function Main():void 
		{
			if (stage)
				init();
			else
				addEventListener(Event.ADDED_TO_STAGE, init);
		}

		private function init(e:Event = null):void 
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			
			var engine:Engine = new Engine();
			stage.addChild(engine);
		}
	}
}