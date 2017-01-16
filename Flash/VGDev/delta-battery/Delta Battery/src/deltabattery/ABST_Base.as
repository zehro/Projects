package deltabattery 
{
	/**	Contains utility functions
	 * @author Alexander Huynh
	 */
	public class ABST_Base 
	{
		public function ABST_Base() 
		{
			// -- do not initialize or call super()
		}
		
		protected function degreesToRadians(degrees:Number):Number
		{
			return degrees * .0175;
		}
		
		protected function radiansToDegrees(radians:Number):Number
		{
			return radians * 57.296;
		}
		
		// faster than Math.abs()
		protected function abs(x:Number):Number
		{
			return (x >= 0 ? x : -x);
		}
		
		protected function getDistance(x1:Number, y1:Number,  x2:Number, y2:Number):Number
		{
			var dx:Number = x1 - x2;
			var dy:Number = y1 - y2;
			return Math.sqrt(dx * dx + dy * dy);
		}
		
		protected function getAngle(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			var dx:Number = x2 - x1;
			var dy:Number = y2 - y1;
			return radiansToDegrees(Math.atan2(dy,dx));
		}
		
		protected function getRand(min:Number = 0, max:Number = 1):Number   
		{  
			return (Math.random() * (max - min + 1)) + min;  
		}
	}
}