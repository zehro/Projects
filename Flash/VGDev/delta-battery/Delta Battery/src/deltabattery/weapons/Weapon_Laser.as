package deltabattery.weapons 
{
	import cobaltric.ContainerGame;
	import deltabattery.SoundPlayer;
	import deltabattery.managers.ABST_Manager;
	import deltabattery.projectiles.ABST_Missile;
	import deltabattery.projectiles.ABST_Vehicle;
	import deltabattery.Turret;
	
	/**
	 * Lesser version of the DeltaStrike.
	 * 
	 * @author Alexander Huynh
	 */
	public class Weapon_Laser extends ABST_Weapon 
	{
		private var isActive:Boolean = false

		public var gfxCount:int;		// current laser drawing time
		public var gfxMax:int;
		
		public function Weapon_Laser(_cg:ContainerGame, _slot:int) 
		{
			super(_cg, _slot);

			name = "LASER";
			
			gfxCount = gfxMax = 4;
			
			cooldownReset = cooldownBase = 30 * 5;
			ammo = ammoMax = ammoBase = 5;
		}
		
		override public function step():void
		{
			if (!isActive && cooldownCounter > 0)
				cooldownCounter--;
			
			if (isActive && gfxCount > 0)
			{
				if (--gfxCount == 0)
				{
					isActive = false;	
					cg.game.c_part.graphics.clear();
					return;
				}
			}
		}

		override public function fire():int
		{
			if (isActive || turret.mc_cannon.deltaStrike.visible ||
			cooldownCounter > 0 || ammo == 0) return -1;
			
			// enable firing
			isActive = true;
			gfxCount = gfxMax;
			
			// seek out nearest projectile to the mouse
			var m:ABST_Missile;
			var tgt:ABST_Missile;
			var proj:Array = cg.getProjectileArray();
			var dist:Number;
			var minDist:Number = 150;				// min distance to mouse
			
			for (var i:int = proj.length - 1; i >= 0; i--)
			{
				m = proj[i];
				if (m.typeSmart == 1) continue;		// ignore player projectiles
				if (m.markedForDestroy) continue;
				
				dist = getDistance(cg.mx, cg.my, m.mc.x, m.mc.y);
				if (dist < minDist)
				{
					minDist = dist;
					tgt = m;
				}
			}
			
			if (tgt)
			{
				gfxCount = gfxMax;
				cg.game.c_part.graphics.clear();
				cg.game.c_part.graphics.lineStyle(2, 0xFF0000, .5);
				cg.game.c_part.graphics.moveTo(turret.x, turret.y);
				cg.game.c_part.graphics.lineTo(tgt.mc.x, tgt.mc.y);
				if (tgt is ABST_Vehicle)
					(tgt as ABST_Vehicle).hp = 0;
				tgt.destroy();
				cooldownCounter = cooldownReset;
				SoundPlayer.play("sfx_launch_laser");
				return --ammo;
			}
			
			// TODO target not found
			
			return ammo;
		}
		
	}

}