package deltabattery.managers {
	import cobaltric.ContainerGame;
	import deltabattery.projectiles.ABST_Explosion;
	import deltabattery.projectiles.Explosion_Standard;
	import flash.geom.Point;

	/**	Manager for a generic explosion
	 *
	 * @author Alexander Huynh
	 */
	public class ManagerExplosion extends ABST_Manager 
	{
		private var manMiss:ManagerMissile;
		private var manArty:ManagerArtillery;
		
		public function ManagerExplosion(_cg:ContainerGame) 
		{
			super(_cg);
			manMiss = cg.manMiss;
			manArty = cg.manArty;
		}
		
		override public function step():void
		{
			var expl:ABST_Explosion;
			for (var i:int = objArr.length - 1; i >= 0; i--)
			{
				expl = objArr[i];
				if (expl.step(manMiss, manArty))
				{
					if (cg.game.c_main.contains(expl.mc))
						cg.game.c_main.removeChild(expl.mc);
					objArr.splice(i, 1);
					expl.mc = null;
					expl = null;
				}
			}
		}
		
		public function spawnExplosion(origin:Point, type:int = 0, scale:Number = 1, flak:Boolean = false):void
		{
			var m:ABST_Explosion = new Explosion_Standard(cg.game.c_main, new ExplosionStandard(), origin, type, scale, flak);
			objArr.push(m);
			cg.game.c_main.addChild(m.mc);
			
			for (var i:int = 0; i < int(getRand(2, 4)) + 2; i++)
			{
				cg.manPart.spawnParticle("", new Point(origin.x + getRand(-5, 5), origin.y + getRand(-5, 5)), 0, getRand(-.5, .5), getRand(-.5, .5), .05);
			}
		}
	}
}