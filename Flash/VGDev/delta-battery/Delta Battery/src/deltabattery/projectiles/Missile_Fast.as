package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**	Fast Missile
	 *
	 *  Missile with twice the standard velocity.
	 * 
	 * @author Gavin Figueroa
	 */
	public class Missile_Fast extends ABST_Missile 
	{
		
		public function Missile_Fast(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			if (!params)
				params = { velocity:(2 * (Math.random() * 2 + 4)) };
			else
				params["velocity"] = 2 * (Math.random() * 2 + 4);
				
			super(_cg, _mc, _origin, _target, _type, params);
			
			money = params["money"] ? params["money"] : 125;
			params["explosionScale"] = .5;	
			damage = 4 + getRand(0, 2);
		}
	}
}