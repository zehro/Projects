package deltabattery.projectiles
{
	import cobaltric.ContainerGame;
	import deltabattery.ABST_Base;
	import flash.display.MovieClip;
	import flash.events.TimerEvent;
	import flash.geom.Point;
	import flash.utils.Timer;
	
	/**	
	 *	A generic, gravity-ignoring projectile
	 * 	@author Alexander Huynh
	 */
	public class ABST_Missile extends ABST_Base 
	{
		public var cg:ContainerGame;					// reference to ContainerGame
		public var mc:MovieClip;						// the MovieClip corresponding to this missile
		
		public var type:int;							// affilation, 0 - enemy, 1 - player
		public var typeSmart:int;						// type for smart weapons to use (i.e. DELTA STRIKE)
		public var damage:Number;						// damage to deal to the city
		
		public var origin:Point;						// spawn location
		protected var target:Point;						// target coordinates
		protected var targetMC:MovieClip;				// target MovieClip (default city)
		
		protected var partEnabled:Boolean = true;		// if TRUE, spawn particles while in flight
		protected var partCount:int = 5;				// particle timer
		protected var partInterval:int = partCount;		// frames inbetween particle spawn
		protected var partType:String = "";				// type of particle
		
		protected var explosionScale:Number = 1;		// scale factor on size of explosion
		
		protected var dist:Number;						// distance to the target
		protected var prevDist:Number;					// distance to the target 1 frame before

		public var velocity:Number;						// velocity (speed) of this missile
		public var rot:Number;							// rotation in rads - handled automatically
		
		protected var createExplosion:Boolean;			// if TRUE, spawn an explosion upon destruction
		public var markedForDestroy:Boolean;			// helper for destroy
		public var readyToDestroy:Boolean;				// helper for destroy
		protected var timerKill:Timer;					// helper for destroy
		
		protected var tgt:MissileTarget;				// + indicator on player-made projectiles
		
		protected var awardMoney:Boolean = true;		// if TRUE, money will be affected upon destruction
		protected var money:int = 100;					// money awarded if the player shoots this down
		
		protected var dx:Number = 0;
		protected var dy:Number = 0;
		
		public function ABST_Missile(_cg:ContainerGame, _mc:MovieClip, _origin:Point,
								     _target:Point, _type:int = 0, params:Object = null)
		{
			cg = _cg;
			mc = _mc;		
			origin = _origin;
			target = _target;
			
			type = typeSmart = _type;
			
			// some magic to prevent some edge case from erroring - kill if invalid parameter(s)
			if (!mc || !origin || !target)
			{
				markedForDestroy = true;
				cleanup(null);
				return;
			}

			mc.x = origin.x;
			mc.y = origin.y;
			
			targetMC = cg.game.city;			// default target is city MC

			// default values
			var useTarget:Boolean = false;
			damage = 9 + getRand(0, 2);
			velocity = 4 + getRand(0, 2);
			createExplosion = true;

			// apply custom values if present
			if (params)
			{
				if (params["target"])
					useTarget = params["target"];
				if (params["velocity"])
					velocity = params["velocity"];
				if (params["partInterval"])
					partInterval = params["partInterval"];
				if (params["explode"])
					createExplosion = params["explode"];
				if (params["explosionScale"])
					explosionScale = params["explosionScale"];
				if (params["damage"])
					damage = params["damage"];
					
				if (params["upgradeV"])								// velocity upgrade
					velocity *= params["upgradeV"];
				if (params["upgradeE"])								// explosion upgrade
					explosionScale *= params["upgradeE"];
			}
			
			// spawn missile target marker (player-fired missiles)
			if (useTarget)
			{
				tgt = new MissileTarget();
				tgt.x = target.x;
				tgt.y = target.y;
				cg.game.c_main.addChild(tgt);
				if (cg.game.bg.ocean.currentFrame > 152) 	// post-sunset
					tgt.gotoAndStop(2);						// white
			}

			mc.rotation = getAngle(origin.x, origin.y, target.x, target.y);
			rot = degreesToRadians(mc.rotation);
			
			dist = prevDist = getDistance(origin.x, origin.y, target.x, target.y);
		}
		
		/**
		 * Main driving function; called 30/sec by ContainerGame.
		 * 
		 * @return	TRUE if object needs to be removed
		 */
		public function step():Boolean
		{
			if (!markedForDestroy)
			{
				// calculate and perform movement
				dx = velocity * Math.cos(rot);
				dy = velocity * Math.sin(rot);
				
				mc.x += dx;
				mc.y += dy;
				
				updateParticle(dx, dy);		// update/spawn particles
				checkTarget();				// check if hit city

				dist = getDistance(mc.x, mc.y, target.x, target.y);

				// destroy if too far out horizontally OR close to target OR moving away from target OR too low vertically
				if ((Math.abs(mc.x) > 800 || dist < 5 || dist > prevDist || mc.y > 170))
					destroy();
				else
					prevDist = dist;
			}

			return readyToDestroy;
		}
		
		/**
		 *	spawns a particle if necessary; update particle counter 
		 */
		protected function updateParticle(dx:Number, dy:Number):void
		{
			if (partEnabled && --partCount == 0)
			{
				partCount = partInterval;
				cg.manPart.spawnParticle(partType, new Point(mc.x, mc.y), 0, dx * .1, dy * .10, .05);
			}
		}

		/**
		 * if this projectile is close to the city, with a 20% chance of happening per frame,
		 * destroy this projectile and damage the city
		 * @param	dest	TRUE always except when called in destroy(), see below
		 */
		protected function checkTarget(dest:Boolean = true):void
		{
			if (type == 1) return;				// ignore player projectiles
			if (!mc) return;					// catch error-causing edge case
			if (!targetMC) return;				// catch error-causing edge case
			if (abs(mc.x - targetMC.x) < 100 && abs(mc.y - (targetMC.y + 50)) < 50 && Math.random() > .8)
			{
				cg.damageCity(this);
				if (dest)
				{
					awardMoney = false;
					destroy();
				}
			}
		}

		/**
		 *	Kill this projectile, spawning an explosion if spawnExplosion is TRUE 
		 * 	Awards up to 50% extra money based on how close the explosion was to this projectile.
		 * 
		 *	@param distance		The distance between this and what destroyed it (or nothing if n/a)
		 */
		public function destroy(distance:Number = 40):void
		{
			if (markedForDestroy) return;		// already going to be destroyed, quit
			checkTarget(false);

			if (awardMoney && type == 0)
			{
				var m:int = money + money * (distance < 40 ? -.0125 * distance + .5 : 0)
				cg.addMoney(m);
				cg.manPart.popup(new Point(mc.x, mc.y - 20), m);
			}
			
			markedForDestroy = true;
			mc.visible = false;
 
			timerKill = new Timer(2000);
			timerKill.addEventListener(TimerEvent.TIMER, cleanup);
			timerKill.start();
			
			if (createExplosion)
				cg.manExpl.spawnExplosion(new Point(mc.x, mc.y), type, explosionScale);

			if (tgt)
			{
				cg.game.c_main.removeChild(tgt);
				tgt.visible = false;
				tgt = null;
			}
		}
		
		public function cleanup(e:TimerEvent):void
		{
			if (timerKill)
			{
				timerKill.removeEventListener(TimerEvent.TIMER, cleanup);
				timerKill = null;
			}			
			readyToDestroy = true;
		}
	}
}