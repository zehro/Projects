package utils
{
	import flash.display.MovieClip;  
	
	public class BGFXParticle extends MovieClip
	{
		private var dir:uint;
		
		public function BGFXParticle()
		{
			init();
		}
		
		private function init():void
		{
			x = Math.floor(Math.random() * 17) * 40 - 240;
			y = Math.floor(Math.random() * 13) * 40 - 320;
			dir = Math.floor(Math.random() * 5);
		}
		
		public function step():void
		{
			switch (dir)
			{
				case 0: x += 5; break;
				case 1: x -= 5; break;
				case 2: y += 5; break;
				case 3: y -= 5; break;
			}
			if (x % 40 == 0 && y % 40 == 0)
				dir = Math.floor(Math.random() * 5);
			if (Math.abs(x) > 320 || Math.abs(y) > 240)
				init();
		}
	}
}
