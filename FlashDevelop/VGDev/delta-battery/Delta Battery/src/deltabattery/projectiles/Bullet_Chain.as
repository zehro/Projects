package deltabattery.projectiles
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**	Chaingun bullet
	 * 
	 *	Standard Chaingun secondary weapon.
	 * 
	 * @author Alexander Huynh
	 */
	public class Bullet_Chain extends ABST_Bullet 
	{
		
		public function Bullet_Chain(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point,
									 _type:int = 0, _life:int = 20, stats:Object = null) 
		{			
			super(_cg, _mc, _origin, _target, _type, _life, stats);
			
			// tracer
			if (Math.random() > .8)
				mc.gotoAndStop("tracer");
			damage = 5 + getRand(0, 8);
		}
	}
}