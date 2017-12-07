package deltabattery.weapons 
{
	import cobaltric.ContainerGame;
	import deltabattery.SoundPlayer;
	import flash.geom.Point;
	import flash.media.Sound;
	
	/**	Standard primary weapon
	 * 
	 * 	Surface-to-Air Missile
	 *
	 * @author Alexander Huynh
	 */
	public class Weapon_SAM extends ABST_Weapon 
	{
		private var sfx_missile:Class;
		
		public function Weapon_SAM(_cg:ContainerGame, _slot:int) 
		{
			super(_cg, _slot);
			name = "SAM";
			projectileParams["velocity"] = 8;
		}
		
		override protected function createProjectile():void
		{
			cg.manMiss.spawnProjectile("standard", new Point(turret.x, turret.y - 15),
												   new Point(cg.mx, cg.my),
										TURRET_ID, projectileParams);
										
			SoundPlayer.play("sfx_launch_standard");
		}
	}
}