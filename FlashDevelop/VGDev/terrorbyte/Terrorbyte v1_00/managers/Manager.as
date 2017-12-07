package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;

	public class Manager
	{
		public var cg:MovieClip;

		public function Manager(_cg:MovieClip)
		{
			cg = _cg;
		}
		
		public function step(ld:Boolean, rd:Boolean, p:Point):void
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
