package deltabattery.weapons 
{
	import cobaltric.ContainerGame;
	import deltabattery.SoundPlayer;
	import flash.geom.Point;
	
	/**
	 * Flak Gun.
	 * 
	 * Good at firing with the primary weapon to fill the sky randomly.
	 * 
	 * @author Alexander Huynh
	 */
	public class Weapon_Flak extends ABST_Weapon 
	{
		
		public function Weapon_Flak(_cg:ContainerGame, _slot:int) 
		{
			super(_cg, _slot);
			name = "FLAK";
			
			cooldownReset = cooldownBase = 18;
			projectileLife = 30 + getRand(0, 10);
			ammo = ammoMax = ammoBase = 25;
		}
		
		override protected function createProjectile():void
		{
			cg.manBull.spawnProjectile("flak", new Point(turret.x, turret.y - 15),
											   new Point(cg.mx, cg.my ),
										TURRET_ID, projectileLife, projectileParams);
			SoundPlayer.play("sfx_launch_flak");
		}
	}
}