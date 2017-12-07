package props
{
	import flash.display.MovieClip;
	import flash.filters.GlowFilter;
	import managers.Manager;
	import flash.geom.ColorTransform;

	public class Prop extends MovieClip
	{
		public var manager:Manager;			// this object's manager
		public var isRunning:Boolean;		// true if stage has started
		
		protected var glowB:GlowFilter;
		protected var glowR:GlowFilter;
		protected var colorW:ColorTransform;
		
		public function Prop(man:Manager, _xP:int, _yP:int)
		{
			manager = man;
			x = _xP; y = _yP;
			
			glowB = new GlowFilter();
			glowB.color = 0x0099FF; 
			glowB.alpha = 2;
			glowB.blurX = glowB.blurY = 10; 

			glowR = new GlowFilter();
			glowR.color = 0xEE422D; 
			glowR.alpha = 2;
			glowR.blurX = glowR.blurY = 10; 
			
			colorW = new ColorTransform();
			colorW.color = 0xFFFFFF;
		}
		
		public function step():Boolean
		{
			// -- override this function
			childStep();
			return false;
		}
		
		public function childStep():Boolean
		{
			// -- override this function
			return false;
		}
		
		public function leftMouse():Boolean
		{
			// -- override this function
			return false;
		}		
		
		public function rightMouse():Boolean
		{
			// -- override this function
			return false;
		}		
		
		// called when stage needs to be restarted
		public function restart():void
		{
			// -- override this function
		}		
		
		// more efficient than Math.abs(...)
		protected function abs(n:Number):Number
		{
			return n < 0 ? -n : n;
		}
 
 		// get the distance between 2 MovieClips
		public function getDist(m1:MovieClip, m2:MovieClip):Number
		{
			return Math.sqrt((m2.x - m1.x) * (m2.x - m1.x) + (m2.y - m1.y) * (m2.y - m1.y));
		}
		
		// get the distance between 2 points
		public function getDistN(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			return Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
		}
		
		// get the angle between 2 points
		protected function getAngle(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			var dx:Number = x2 - x1;
			var dy:Number = y2 - y1;
			return radiansToDegrees(Math.atan2(dy,dx));
		}
		
		protected function degreesToRadians(degrees:Number):Number
		{
			return degrees * .0175;
		}
		
		protected function radiansToDegrees(radians:Number):Number
		{
			return radians * 57.296;
		}
		
		protected function getRand(min:Number, max:Number, useInt:Boolean = false):Number   
		{  
			if (useInt)
				return (Math.floor(Math.random() * (max - min + 1)) + min);
			return (Math.random() * (max - min + 1)) + min;  
		} 
		
		public function destroy():void
		{
			
		}
	}
}
