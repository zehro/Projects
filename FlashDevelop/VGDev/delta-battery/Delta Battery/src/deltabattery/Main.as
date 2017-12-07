package deltabattery
{
	import flash.display.Sprite;
	import flash.events.Event;
	import cobaltric.Engine;

	/**
	 * Initialization and entry point
	 * @author Alexander Huynh
	 */
	//[Frame(factoryClass = "Preloader")]
	[SWF(width="800", height="500", backgroundColor="#666666", frameRate="30")]
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