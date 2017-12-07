package deltabattery.managers
{
	import cobaltric.ContainerGame;
	import deltabattery.projectiles.ABST_Artillery;
	import deltabattery.projectiles.ABST_Bullet;
	import deltabattery.projectiles.ABST_Missile;
	import deltabattery.projectiles.Bullet_Chain;
	import deltabattery.projectiles.Bullet_Flak;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**	Manager for a generic gravity-ignoring bullet
	 * 
	 * @author Alexander Huynh
	 */
	public class ManagerBullet extends ABST_Manager 
	{	
		private var manMiss:ManagerMissile;
		private var manArty:ManagerArtillery;
		
		public function ManagerBullet(_cg:ContainerGame) 
		{
			super(_cg);
			manMiss = cg.manMiss;
			manArty = cg.manArty;
		}
		
		override public function step():void
		{
			var bull:ABST_Bullet;
			var miss:ABST_Missile;
			var arty:ABST_Artillery;
			var j:int;
			for (var i:int = objArr.length - 1; i >= 0; i--)
			{
				bull = objArr[i];
				if (bull.step())
				{
					if (cg.game.c_main.contains(bull.mc))
						cg.game.c_main.removeChild(bull.mc);
					objArr.splice(i, 1);
					bull = null;
				}
				else
				{
					// check for collisions
					for (j = manMiss.objArr.length - 1; j >= 0; j--)
					{
						miss = manMiss.objArr[j]
						if (bull.type == miss.type) continue;

						if (getDistance(bull.mc.x, bull.mc.y, miss.mc.x, miss.mc.y) < 15)		// TODO replace magic number
							miss.destroy();
					}
					for (j = manArty.objArr.length - 1; j >= 0; j--)
					{
						arty = manArty.objArr[j]

						if (getDistance(bull.mc.x, bull.mc.y, arty.mc.x, arty.mc.y) < 15)		// TODO replace magic number
							arty.destroy();
					}
				}
			}
		}
		
		/**	Spawn a gravity-ignoring bullet
		 * 
		 *	@proj		the type of projectile to spawn
		 * 	@origin		the starting location of the projectile
		 * 	@target		where the projectile will head toward
		 * 	@type		the affiliation that this projectile has (0 enemy; 1 player)
		 * 	@params		parameters for the projectile
		 */
		public function spawnProjectile(proj:String, origin:Point, target:Point,
										type:int = 0, life:int = 15, params:Object = null):void
		{
			switch (proj)
			{
				case "flak":
					addObject(new Bullet_Flak(cg, new BulletStandard(), origin, target, type, life, params));
				break;
				default:		// "chain"
					addObject(new Bullet_Chain(cg, new BulletStandard(), origin, target, type, life, params));
			}
		}
		
		private function addObject(b:ABST_Bullet):void
		{
			objArr.push(b);
			cg.game.c_main.addChild(b.mc);
		}
	}
}