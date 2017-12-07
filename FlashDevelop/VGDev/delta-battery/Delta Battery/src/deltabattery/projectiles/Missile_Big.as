package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class Missile_Big extends ABST_Missile 
	{
		
		public function Missile_Big(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			if (!params)
				params = { velocity: 2 + getRand(0, 1) };
			else if (!params["velocity"])
				params["velocity"] = 2 + getRand(0, 1);
				
			super(_cg, _mc, _origin, _target, _type, params);
			
			money = params["money"] ? params["money"] : 125;
			if (type == 0)
				params["explosionScale"] = 1.5;	
			damage = 13 + getRand(0, 4);
		}
		
	}

}