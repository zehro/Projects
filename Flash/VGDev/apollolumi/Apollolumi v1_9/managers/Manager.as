package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;

	public class Manager
	{
		public var par:MovieClip;

		public function Manager(_par:MovieClip)
		{
			par = _par;
		}
		
		public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			// -- override this function
		}
		
		public function getVec():Vector.<MovieClip>
		{
			// -- override this function
			return null;
		}
		
		protected function getRand(min:Number, max:Number):Number   
		{  
			return (Math.random() * (max - min + 1)) + min; 
		} 
		
		public function destroy():void
		{
			// -- override this function
		}
	}
}
