package 
{
	import flash.events.Event;

	public class ContainerHangar extends Container
	{
		private var slid:SliderUI;
		private var prevZoom:Number = 0;
		
		public function ContainerHangar()
		{
			super();
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			slid = new SliderUI(stage, "x", obj_track, obj_slider, 0, 100, 50);
		}
		
		override public function step():Boolean
		{
			if (slid.percent == prevZoom) return completed;
			prevZoom = slid.percent;
			var zm:Number = prevZoom * 100;
			hangar.contents.scaleX = hangar.contents.scaleY = 1.25 * (.0002 * zm * zm + 0.01 * zm);
			return completed;
		}
	}
}
