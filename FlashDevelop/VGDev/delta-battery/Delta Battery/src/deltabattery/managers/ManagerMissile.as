package deltabattery.managers
{
	import cobaltric.ContainerGame;
	import deltabattery.projectiles.*;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**	Manager for a generic gravity-ignoring missile
	 * 
	 * @author Alexander Huynh
	 */
	public class ManagerMissile extends ABST_Manager 
	{	
		
		public function ManagerMissile(_cg:ContainerGame) 
		{
			super(_cg);
		}
		
		override public function step():void
		{			
			var miss:ABST_Missile;
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

		/**	Spawn a gravity-ignoring missile
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
				case "fast":
					addObject(new Missile_Fast(cg, new MissileFast(), origin, target, type, params));
				break;
				case "fastT":	// turret RAAM
					addObject(new Missile_Standard(cg, new MissileFast(), origin, target, type, params));
				break;
				case "big":
					addObject(new Missile_Big(cg, new MissileCruise(), origin, target, type, params));
				break;
				case "rocket":
					addObject(new Missile_Fast(cg, new MissileRocket(), origin, target, type, params));
				break;
				case "cluster":
					addObject(new Missile_Cluster(cg, new MissileBig(), origin, target, type, params));
				break;
				case "LASM":
					addObject(new Missile_LASM(cg, new MissileLASM(), origin, target, type, params));
				break;
				case "pop":
					addObject(new Missile_Popup(cg, new MissilePop(), origin, target, type, params));
				break;
				case "bomb":
					addObject(new Artillery_Bomb(cg, new ArtilleryBomb(), origin, target, type, params));
				break;
				case "bomber":
					addObject(new Vehicle_Bomber(cg, new Bomber(), origin, target, type, params));
				break;
				case "satellite":
					addObject(new Vehicle_Satellite(cg, new Satellite(), origin, target, type, params));
				break;
				case "helicopter":
					addObject(new Vehicle_Helicopter(cg, new Helicopter(), origin, target, type, params));
				break;
				case "plane":
					addObject(new Vehicle_PlanePassenger(cg, new Airplane(), origin, target, type, params));
				break;
				case "ship":
					addObject(new Vehicle_ShipPassenger(cg, new ShipPassenger(), origin, target, type, params));
				break;
				default:		// "standard"
					addObject(new Missile_Standard(cg, new MissileStandard(), origin, target, type, params));
			}
		}
		
		private function addObject(a:ABST_Missile):void
		{
			objArr.push(a);
			cg.game.c_main.addChild(a.mc);
		}
	}
}