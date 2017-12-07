package deltabattery.managers
{
	import cobaltric.ContainerGame;
	import deltabattery.projectiles.ABST_Artillery;
	import deltabattery.projectiles.Artillery_Standard;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**	Manager for a generic ballistic artillery shell
	 * 
	 * @author Alexander Huynh
	 */
	public class ManagerArtillery extends ABST_Manager 
	{	
		
		public function ManagerArtillery(_cg:ContainerGame) 
		{
			super(_cg);
		}
		
		override public function step():void
		{			
			var miss:ABST_Artillery;
			for (var i:int = objArr.length - 1; i >= 0; i--)
			{
				miss = objArr[i];
				if (miss.step())
				{
					if (cg.game.c_main.contains(miss.mc))
						cg.game.c_main.removeChild(miss.mc);
					objArr.splice(i, 1);
					miss = null;
				}
			}
		}
		
		/**	Spawn a ballistic artillery shell
		 * 
		 *	@proj		the type of projectile to spawn
		 * 	@origin		the starting location of the projectile
		 * 	@target		where the projectile will head toward
		 * 	@type		the affiliation that this projectile has (0 enemy; 1 player)
		 * 	@params		parameters for the projectile
		 */
		public function spawnProjectile(proj:String, origin:Point, target:Point, type:int = 0, params:Object = null):void
		{
			switch (proj)
			{
				default:		// "standard"
					addObject(new Artillery_Standard(cg, new ArtilleryStandard(), origin, target, type, params));
			}
		}
		
		private function addObject(a:ABST_Artillery):void
		{
			objArr.push(a);
			cg.game.c_main.addChild(a.mc);
		}
	}
}