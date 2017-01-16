package deltabattery.weapons 
{
	import cobaltric.ContainerGame;
	import deltabattery.SoundPlayer;
	import flash.geom.Point;
	
	/**
	 * Rapid Anti-Air Missile.
	 * 
	 * Fast-travelling missile with high reload time and small area of effect.
	 *
	 * @author Alexander Huynh
	 */
	public class Weapon_RAAM extends ABST_Weapon 
	{
		public function Weapon_RAAM(_cg:ContainerGame, _slot:int) 
		{
			super(_cg, _slot);
			name = "RAAM";
			
			projectileParams["velocity"] = 16;
			projectileParams["partInterval"] = 2;
			projectileParams["explosionScale"] = .5;
			
			cooldownReset = cooldownBase = 60;
			ammo = ammoMax = ammoBase = 10;
		}
		
		override protected function createProjectile():void
		{
			cg.manMiss.spawnProjectile("fastT", new Point(turret.x, turret.y - 15),
												   new Point(cg.mx, cg.my),
										TURRET_ID, projectileParams);	
			SoundPlayer.play("sfx_launch_fast");
		}
		
	}

}