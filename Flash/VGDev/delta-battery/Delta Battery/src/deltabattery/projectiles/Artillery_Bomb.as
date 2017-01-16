package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * A gravity-following, deployable bomb.
	 * 
	 * @author Alexander Huynh
	 */
	public class Artillery_Bomb extends ABST_Artillery 
	{
		public function Artillery_Bomb(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			super(_cg, _mc, _origin, _target, _type, params);
			
			// override super
			dx = velocity;
			dy = 0;
			
			// shrink
			mc.scaleX = mc.scaleY = .6;
			
			money = 50;
			damage = 15 + getRand(0, 10);
			explosionScale = 1.25;
		}	
	}
}