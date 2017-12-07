package deltabattery.weapons
{
	import cobaltric.ContainerGame;
	import deltabattery.ABST_Base;
	import deltabattery.Armory;
	import flash.display.MovieClip;
	/**	An abstract weapon used in Turret.as
	 *
	 * @author Alexander Huynh
	 */
	public class ABST_Weapon extends ABST_Base
	{		
		protected var cg:ContainerGame;	
		protected var turret:MovieClip;			// the Turret MovieClip
		protected const TURRET_ID:int = 1;		// type to use
		
		public var name:String;					// display name for the weapon (SAM, RAAM, Chain, etc.)
		public var slot:int;					// weapon slot
		
		public var cooldownBase:int = 20;		// base cooldown time
		public var cooldownReset:int = 20;		// amount to set the cooldown to after firing - affected by upgrades
		public var cooldownCounter:int = 0;		// actual cooldown timer, 0 if ready to fire
		
		public var projectileParams:Object = new Object();
		public var projectileLife:int = -1;
		public var projectileRange:int = -1;	// uses life to determine when to despawn
		
		public var ammoBase:int = 25;			// base reserve ammo
		public var ammoMax:int = ammoBase;		// max reserve ammo - affected by upgrades
		public var ammo:int = ammoMax;			// current ammo
		
		public function ABST_Weapon(_cg:ContainerGame, _slot:int) 
		{
			cg = _cg;
			turret = cg.game.mc_turret;
			slot = _slot;

			projectileParams["target"] = true
		}
		
		/**
		 * Sets this projectile's upgrades based on the multipliers in Armory.
		 */
		public function setUpgrades():void
		{
			var arm:Armory = cg.armory;
			projectileParams["upgradeE"] = arm.upgMod[0][arm.upgLevel[0]];
			projectileParams["upgradeV"] = arm.upgMod[1][arm.upgLevel[1]];
			
			cooldownReset = int(cooldownBase * (1 / arm.upgMod[2][arm.upgLevel[2]]));
			ammoMax = int(ammoBase * arm.upgMod[3][arm.upgLevel[3]]);
			
			/*trace("## " + this);
			trace("EXPL:\t" + projectileParams["upgradeE"]);
			trace("SPED:\t" + projectileParams["upgradeV"]);
			trace("COOL:\t" + cooldownReset + "/" + cooldownBase);
			trace("AMMO:\t" + ammoMax + "/" + ammoBase);*/
		}
		
		public function step():void
		{
			if (cooldownCounter > 0)		// GUI updated in Turret
				cooldownCounter--;
		}
		
		/**
		 * Resets this weapon's ammo and cooldowns.
		 */
		public function reset():void
		{
			cooldownCounter = 0;
			ammo = ammoMax;
		}

		// returns new remaining ammo, or -1 if couldn't fire
		public function fire():int
		{
			if (cooldownCounter > 0 || ammo == 0) return -1;

			
			createProjectile();
			cooldownCounter = cooldownReset;
			return --ammo;
		}
		
		protected function createProjectile():void
		{
			// -- override this function
		}
		
		// returns new remaining ammo
		// can also removeAmmo with a -amt
		public function addAmmo(amt:int):int
		{
			ammo += amt;
			if (ammo < 0)
				ammo = 0;
			else if (ammo > ammoMax)
				ammo = ammoMax;
			return ammo;
		}
	}
}