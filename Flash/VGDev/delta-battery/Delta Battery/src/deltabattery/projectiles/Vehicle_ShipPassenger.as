package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * A cruise ship that can be damaged by enemy projectiles.
	 * Protect it for money; otherwise, lose money.
	 * 
	 * Uses Vehicle_PlanePassenger code.
	 * 
	 * @author Alexander Huynh
	 */
	public class Vehicle_ShipPassenger extends Vehicle_PlanePassenger
	{
		public function Vehicle_ShipPassenger(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			if (!params)
				params = { velocity:(.5 + getRand(0, 1)) };
			else
				params["velocity"] = .5 + getRand(0, 1);

			super(_cg, _mc, _origin, _target, _type, params);

			money = -700;
			hp = hpMax = 25;
			type = 1;
		}
	}
}