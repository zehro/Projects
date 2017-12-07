package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 *	Standard artillery projectile
	 *
	 *	A projectile following a parabolic arc, initialized with some initial negative y velocity.
	 * 
	 *	@author Alexander Huynh
	 */
	public class Artillery_Standard extends ABST_Artillery 
	{
		public function Artillery_Standard(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			params = new Object();
			params["velocity"] = 2;

			super(_cg, _mc, _origin, _target, _type, params);	
			
			// override super
			dx = velocity * Math.cos(rot) * 2;
			dy = -velocity * Math.sin(rot) * 1.5;	
			
			money = 125;
			damage = 12 + getRand(0, 4);
		}
	}
}