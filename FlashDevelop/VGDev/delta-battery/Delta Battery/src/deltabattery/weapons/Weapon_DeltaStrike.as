package deltabattery.weapons 
{
	import cobaltric.ContainerGame;
	import deltabattery.SoundPlayer;
	import deltabattery.managers.ABST_Manager;
	import deltabattery.projectiles.ABST_Missile;
	import deltabattery.projectiles.ABST_Vehicle;
	import deltabattery.Turret;
	
	/**
	 * A powerful, auto-targeting sky-clearer. Panic button.
	 * 
	 * @author Alexander Huynh
	 */
	public class Weapon_DeltaStrike extends ABST_Weapon 
	{
		private var isActive:Boolean = false
		
		public var duration:int;		// current active time left
		public var durationMax:int;		// active time given when activated
		
		public var delay:int;			// current delay between firing
		public var delayMax:int;		// delay between firing
		
		public var gfxCount:int;		// current laser drawing time
		public var gfxMax:int;
		
		public function Weapon_DeltaStrike(_cg:ContainerGame) 
		{
			super(_cg, 7);
			name = "DELTA";
			
			durationMax = duration = 90;	// 3 seconds
			delay = delayMax = 4;
			gfxCount = gfxMax = 2;
			
			cooldownReset = cooldownBase = 30 * 20;		// 20 seconds
			ammo = ammoMax = ammoBase = 1;
		}
		
		override public function step():void
		{
			if (!isActive && cooldownCounter > 0)
				cooldownCounter--;
				
			if (!isActive) return;
				
			if (--duration == 0)
			{
				isActive = false;
				turret.mc_cannon.deltaStrike.visible = false;		
				cg.game.c_part.graphics.clear();		
				return;
			}
			
			if (gfxCount > 0)
			{
				if (--gfxCount == 0)
					cg.game.c_part.graphics.clear();
			}
				
			if (delay > 0)
			{
				delay--;
				return;
			}
			
			// seek out nearest projectile
			var m:ABST_Missile;
			var tgt:ABST_Missile;
			var proj:Array = cg.getProjectileArray();
			var dist:Number;
			var minDist:Number = 9999;
			
			for (var i:int = proj.length - 1; i >= 0; i--)
			{
				m = proj[i];
				if (m.typeSmart == 1) continue;		// ignore player projectiles
				if (m.markedForDestroy) continue;
				
				dist = getDistance(turret.x, turret.y, m.mc.x, m.mc.y);
				if (dist > 680) continue;		// range is 680
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
				cg.game.c_part.graphics.lineStyle(1, 0xFF0000, .5);
				cg.game.c_part.graphics.moveTo(turret.x, turret.y);
				cg.game.c_part.graphics.lineTo(tgt.mc.x, tgt.mc.y);
				if (tgt is ABST_Vehicle)
					(tgt as ABST_Vehicle).hp = 0;
				tgt.destroy();
				delay = delayMax;
				SoundPlayer.play("sfx_launch_laser");
			}
		}
		
		override public function fire():int
		{
			if (cooldownCounter > 0 || ammo == 0) return -1;
			
			// enable firing
			isActive = true;
			turret.mc_cannon.deltaStrike.visible = true;
			duration = durationMax;
			delay = 0;
			gfxCount = 0;
			
			cooldownCounter = cooldownReset;
			return --ammo;
		}
		
		
	}
}