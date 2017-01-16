package 
{
    import flash.events.Event;  
    import flash.events.ProgressEvent;  

	public class ContainerPreloader extends Container
	{
		private var WIDTH:Number;

		public function ContainerPreloader()
		{
			super();
			WIDTH = loadBar.width;

			this.loaderInfo.addEventListener(Event.COMPLETE, onComplete);
			this.loaderInfo.addEventListener(ProgressEvent.PROGRESS, onProgress);
		}

        private function onProgress(evt:ProgressEvent):void  
        {  
			var loaded:Number = evt.bytesLoaded / evt.bytesTotal; 
			loadBar.width = loaded*WIDTH;
        }  

        private function onComplete(e:Event):void  
        {  
			play(); 
			this.loaderInfo.removeEventListener(Event.COMPLETE, onComplete);
			this.loaderInfo.removeEventListener(ProgressEvent.PROGRESS, onProgress);
        }  
	}
}