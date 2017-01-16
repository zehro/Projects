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
			
			//this.loaderInfo.addEventListener(Event.COMPLETE, onComplete);
			//this.loaderInfo.addEventListener(ProgressEvent.PROGRESS, onProgress);
			
			play();		// debug override, since FlashDevelop doesn't like this
		}
          
        private function onProgress(evt:ProgressEvent):void  
        {  
			var loaded:Number = evt.bytesLoaded / evt.bytesTotal; 
			percent_txt.text = (loaded*100).toFixed(0) + "%";
			loadBar.width = loaded*WIDTH;
        }  
          
        private function onComplete(e:Event):void  
        {  
			this.loaderInfo.removeEventListener(Event.COMPLETE, onComplete);
			this.loaderInfo.removeEventListener(ProgressEvent.PROGRESS, onProgress);
			play(); 
        }  
	}
}
