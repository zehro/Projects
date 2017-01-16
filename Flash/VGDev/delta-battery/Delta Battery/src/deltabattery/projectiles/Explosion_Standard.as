package deltabattery.projectiles 
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**	The standard explosion
	 *
	 * 	Used by default
	 * 
	 * @author Alexander Huynh
	 */
	public class Explosion_Standard extends ABST_Explosion 
	{
		
		public function Explosion_Standard(_par:MovieClip, _mc:MovieClip, _origin:Point, _type:int=0, _scale:Number = 1, _flak:Boolean = false) 
		{
			super(_par, _mc, _origin, _type, _scale, _flak);
		}
	}
}