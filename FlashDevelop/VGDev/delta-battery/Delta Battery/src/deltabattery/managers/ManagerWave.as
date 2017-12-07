package deltabattery.managers
{
	import cobaltric.ABST_Container;
	import cobaltric.ContainerGame;
	import deltabattery.projectiles.ABST_Missile;
	import flash.geom.Point;
	
	/**	Directs enemy spawns throughout the game
	 *
	 * @author Alexander Huynh
	 */
	public class ManagerWave extends ABST_Manager 
	{
		private var manMiss:ManagerMissile;
		private var manArty:ManagerArtillery;
		public var wave:int;

		public var waveActive:Boolean;
		
		private const SEC:int = 30;
		public var enemiesRemaining:int;
		
		private var spawnDelay:int;		// counter
		private var spawnMin:int;		// counter reset value (minimum)
		private var spawnMax:int;		// maximum counter value
		private var spawnRandom:Number;	// chance to spawn a projectile per tick
		private var spawnDouble:Number;	// chance to spawn 2 projectiles this time
		private var spawnTriple:Number;	// chance to spawn 3 projectiles this time
		
		private var spawnLoc:Object = new Object();
		/*	spawnLoc[type] -> [null, [(x1, y1), (x2, y2)], null]
		 * 
		 * 	let arr be spawnLoc[type]
		 * 	arr				null if not able to spawn in this wave
		 * 	arr[]			regions to spawn in
		 * 	arr[][0..1]		upper left and lower right corners of
		 * 					spawn region
		 */
		
		private var spawnType:Array;
		/*	chance (weights) of given projectile to spawn
		 */
		private const MISSILE:int 	= 0;
		private const ARTY:int 		= 1;
		private const FAST:int 		= 2;
		private const BIG:int 		= 3;
		private const CLUSTER:int 	= 4;
		private const LASM:int 		= 5;
		private const BOMBER:int 	= 6;
		private const HELI:int 		= 7;
		private const PLANE:int 	= 8;
		private const SHIP:int 		= 9;
		private const SAT:int		= 10;
		private const POPUP:int		= 11;
		
		private var targetX:int = 390;
		private var targetY:int = 150;
		private var targetVarianceX:int = 80;
		private var targetVarianceY:int = 30;
		
		private var dayFlag:int = 0;	// 0 day, 1 sunset, 2 night
		private var sunsetAmt:int = 0;
		private var nightAmt:int = 0;
		
		// region helpers
		private const R_TOP_LEFT:Array = [new Point( -400, -350), new Point( -300, -260)];
		private const R_TOP_CENTER:Array = [new Point( -300, -350), new Point( -100, -260)];
		
		private const R_CORNER_SMALL:Array = [new Point( -450, -350), new Point( -410, -260)];
		private const R_CORNER_LARGE:Array = [new Point( -500, -300), new Point( -410, -260)];
		
		private const R_LEFT_TOP:Array = [new Point( -450, -200), new Point( -410, -150)];
		private const R_LEFT_LASM:Array = [new Point( -450, -230), new Point( -410, -150)];
		private const R_LEFT_CENTER:Array = [new Point( -450, -75), new Point( -410, 75)];
		private const R_LEFT_BOT:Array = [new Point( -450, 75), new Point( -410, 180)];
		
		private const R_RIGHT_LASM:Array = [new Point( 450, -250), new Point( 410, -150)];	// passenger planes
		private const R_RIGHT_BOT:Array = [new Point( 150, 160), new Point( 150, 170)];		// passenger ships
		
		private const R_SEA:Array = [new Point( -360, 120), new Point( -200, 130)];			// popups
		
		private const R_ARTY_NORM:Array = [new Point( -530, -170), new Point( -480, -220)];
		
		// param helpers
		private const P_FAST:Object = { explosionScale: .75 };
		private const P_BIG:Object = { explosionScale: 2.25 };
		
		public function ManagerWave(_cg:ContainerGame, _wave:int = 1)
		{
			super(_cg);
			
			manMiss = cg.manMiss;
			manArty = cg.manArty;
			wave = _wave;
			
			resetSpawnType();
			startWave();
		}
		
		public function startWave():void
		{
			waveActive = true;
			dayFlag = 0;
			advanceTime();		// reset sky/ocean
			resetSpawnType();	// reset probabilities of all projectiles spawning to 0
			
			// TEMPLATE
			/*
					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP];
					spawnLoc[BIG] = [R_LEFT_TOP];
					spawnLoc[CLUSTER] = [R_LEFT_TOP];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[POPUP] = [R_SEA];
					spawnLoc[HELI] = [R_LEFT_CENTER];
					spawnLoc[BOMBER] = [R_LEFT_LASM];
					spawnLoc[SAT] = [R_LEFT_TOP];
					spawnLoc[PLANE] = [R_RIGHT_LASM];
					spawnLoc[SHIP] = [R_RIGHT_BOT];

					// set spawn probabilities
					spawnType[MISSILE] = 1;
					spawnType[ARTY] = 1;
					spawnType[FAST] = 1;
					spawnType[BIG] = 1;
					spawnType[CLUSTER] = 1;
					spawnType[LASM] = 1;
					spawnType[POPUP] = 1;
					spawnType[HELI] = 1;
					spawnType[BOMBER] = 1;
					spawnType[SAT] = 1;
					spawnType[PLANE] = 1;
					spawnType[SHIP] = 1;
					
					spawnDelay = 30 * 2;
					spawnMin = 30 * 2;
					spawnMax = -30 * 1.5;
					spawnRandom = .98;
			*/
			
			
			switch (wave)
			{
				// standard missile
				case 1:
					enemiesRemaining = 6;

					// enable missiles
					spawnLoc[MISSILE] = [R_LEFT_TOP];

					// enable projectiles and regions
					spawnType[MISSILE] = 1;
					
					spawnDelay = SEC * 2;		// 2 seconds initial delay
					spawnMin = SEC * 2;			// 2 seconds minimum
					spawnMax = -SEC * 4;		// 4 seconds maximum
					spawnRandom = .98;
					spawnDouble = 0;
					spawnTriple = 0;
				break;
				// standard missile, increased rate
				case 2:
					enemiesRemaining = 10;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_CORNER_SMALL];

					// set spawn probabilities
					spawnType[MISSILE] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 1.2;
					spawnMax = -SEC * 2.5;
					spawnRandom = .965;
					spawnDouble = 0;
					spawnTriple = 0;
				break;
				// artillery
				case 3:
					enemiesRemaining = 6;

					// enable projectiles and regions
					spawnLoc[ARTY] = [R_ARTY_NORM];

					// set spawn probabilities
					spawnType[ARTY] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 4;
					spawnRandom = .98;
					spawnDouble = 0;
					spawnTriple = 0;
				break;
				// standard and artillery
				case 4:
					enemiesRemaining = 16;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_CORNER_SMALL];
					spawnLoc[ARTY] = [R_ARTY_NORM];

					// set spawn probabilities
					spawnType[MISSILE] = 3;
					spawnType[ARTY] = 2;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 3;
					spawnRandom = .965;
					spawnDouble = .5;
					spawnTriple = 0;
				break;
				// big
				case 5:
					enemiesRemaining = 8;

					// enable projectiles and regions
					spawnLoc[BIG] = [R_CORNER_SMALL];

					// set spawn probabilities
					spawnType[BIG] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 4;
					spawnRandom = .98;
				break;
				// standard and big
				case 6:
					enemiesRemaining = 16;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_CORNER_SMALL, R_LEFT_TOP];
					spawnLoc[BIG] = [R_CORNER_SMALL, R_LEFT_TOP];

					// set spawn probabilities
					spawnType[MISSILE] = 3;
					spawnType[BIG] = 2;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 3;
					spawnRandom = .97;
					spawnDouble = .25;
					spawnTriple = 0;
				break;
				// fast
				case 7:
					enemiesRemaining = 10;

					// enable projectiles and regions
					spawnLoc[FAST] = [R_CORNER_LARGE];

					// set spawn probabilities
					spawnType[FAST] = 1;
					
					spawnDelay = SEC * 3;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 6;
					spawnRandom = .98;
					spawnDouble = 0;
					spawnTriple = 0;
				break;
				// assault
				case 8:
					enemiesRemaining = 26;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_CORNER_LARGE];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE];

					// set spawn probabilities
					spawnType[MISSILE] = 2;
					spawnType[ARTY] = 1;
					spawnType[FAST] = 1;
					spawnType[BIG] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 1.5;
					spawnMax = -SEC * 4;
					spawnRandom = .96;
					spawnDouble = .25;
					spawnTriple = 0;
				break;
				// LASM
				case 9:
					enemiesRemaining = 16;

					// enable projectiles and regions
					spawnLoc[LASM] = [R_LEFT_LASM];

					// set spawn probabilities
					spawnType[LASM] = 1;
					
					spawnDelay = SEC * 3;
					spawnMin = SEC * 1.25;
					spawnMax = -SEC * 3;
					spawnRandom = .97;
					spawnDouble = 0;
					spawnTriple = 0;
				break;
				// Missile Blitz
				case 10:
					enemiesRemaining = 26;

					// enable projectiles and regions
					spawnLoc[FAST] = [R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_TOP_LEFT];
					spawnLoc[LASM] = [R_LEFT_LASM];

					// set spawn probabilities
					spawnType[FAST] = 1;
					spawnType[BIG] = 1;
					spawnType[LASM] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 4;
					spawnRandom = .965;
					spawnDouble = .35;
					spawnTriple = 0;
				break;
				// Helicopter
				case 11:
					enemiesRemaining = 6;

					// enable projectiles and regions
					spawnLoc[HELI] = [R_LEFT_CENTER];

					// set spawn probabilities
					spawnType[HELI] = 1;
					
					spawnDelay = SEC * 3;
					spawnMin = SEC * 3;
					spawnMax = -SEC * 5;
					spawnRandom = .95;
					spawnDouble = .3;
					spawnTriple = 0;
				break;
				// high trajectory
				case 12:
					enemiesRemaining = 24;

					// enable projectiles and regions
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[LASM] = [R_LEFT_LASM];

					// set spawn probabilities
					spawnType[ARTY] = 1;
					spawnType[LASM] = 1;
					
					spawnDelay = SEC * 1;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 2.5;
					spawnRandom = .965;
					spawnDouble = .6;
					spawnTriple = .3;
				break;
				// assault
				case 13:
					enemiesRemaining = 36;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP, R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_LEFT_CENTER];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[HELI] = [R_LEFT_CENTER];

					// set spawn probabilities
					spawnType[MISSILE] = 6;
					spawnType[ARTY] = 1;
					spawnType[FAST] = 3;
					spawnType[BIG] = 3;
					spawnType[LASM] = 1;
					spawnType[HELI] = 1;
					
					spawnDelay = SEC * 4;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 3;
					spawnRandom = .98;
					spawnDouble = .5;
					spawnTriple = .2;
				break;
				// Cluster
				case 14:
					enemiesRemaining = 8;

					// enable projectiles and regions
					spawnLoc[CLUSTER] = [R_LEFT_TOP];

					// set spawn probabilities
					spawnType[CLUSTER] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 5;
					spawnRandom = .96;
					spawnDouble = .5;
					spawnTriple = .25;
				break;
				// assault
				case 15:
					enemiesRemaining = 40;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_LEFT_CENTER];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_CORNER_SMALL];

					// set spawn probabilities
					spawnType[MISSILE] = 6;
					spawnType[ARTY] = 1;
					spawnType[FAST] = 1;
					spawnType[BIG] = 1;
					spawnType[LASM] = 1;
					spawnType[CLUSTER] = 1;
					
					spawnDelay = SEC * 4;
					spawnMin = SEC * .7;
					spawnMax = -SEC * 2.6;
					spawnRandom = .975;
					spawnDouble = .65;
					spawnTriple = .3;
				break;
				// Fast Blitz
				case 16:
					enemiesRemaining = 16;

					// enable projectiles and regions
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_SMALL, R_TOP_LEFT];

					// set spawn probabilities
					spawnType[FAST] = 1;
					
					spawnDelay = SEC * 1;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 3;
					spawnRandom = .95;
					spawnDouble = .5;
					spawnTriple = .25;
				break;
				// Passenger Plane
				case 17:
					enemiesRemaining = 16;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[PLANE] = [R_RIGHT_LASM];

					// set spawn probabilities
					spawnType[MISSILE] = 2;
					spawnType[ARTY] = 2;
					spawnType[LASM] = 4;
					spawnType[PLANE] = 1;
					
					spawnDelay = SEC * 1;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 4;
					spawnRandom = .95;
					spawnDouble = .3;
					spawnTriple = 0;
				break;
				// assault
				case 18:
					enemiesRemaining = 50;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_LEFT_CENTER];
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_CORNER_SMALL];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[HELI] = [R_LEFT_CENTER];

					// set spawn probabilities
					spawnType[MISSILE] = 4;
					spawnType[ARTY] = 1;
					spawnType[FAST] = 2;
					spawnType[BIG] = 2;
					spawnType[CLUSTER] = 1;
					spawnType[LASM] = 1;
					spawnType[HELI] = 1;
					
					spawnDelay = SEC * 4;
					spawnMin = SEC * .5;
					spawnMax = -SEC * 2.5;
					spawnRandom = .985;
					spawnDouble = .75;
					spawnTriple = .3;
				break;
				// Pop
				case 19:
					enemiesRemaining = 14;

					// enable projectiles and regions
					spawnLoc[POPUP] = [R_SEA];

					// set spawn probabilities
					spawnType[POPUP] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 4;
					spawnRandom = .95;
					spawnDouble = .4;
					spawnTriple = 0;
				break;
				// Clustertruck
				case 20:
					enemiesRemaining = 20;

					// enable projectiles and regions
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_SMALL, R_TOP_LEFT];

					// set spawn probabilities
					spawnType[CLUSTER] = 1;
					
					spawnDelay = SEC * 3;
					spawnMin = SEC * .4;
					spawnMax = -SEC * 3;
					spawnRandom = .96;
					spawnDouble = .7;
					spawnTriple = .5;
				break;
				// Bomber
				case 21:
					enemiesRemaining = 16;

					// enable projectiles and regions
					spawnLoc[HELI] = [R_LEFT_CENTER];
					spawnLoc[BOMBER] = [R_LEFT_LASM];

					// set spawn probabilities
					spawnType[HELI] = 1;
					spawnType[BOMBER] = 3;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 2;
					spawnMax = -SEC * 6;
					spawnRandom = .98;
					spawnDouble = .5;
					spawnTriple = 0;
				break;
				// assault
				case 22:
					enemiesRemaining = 60;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_LEFT_CENTER];
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_CORNER_SMALL];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[HELI] = [R_LEFT_CENTER];

					// set spawn probabilities
					spawnType[MISSILE] = 8;
					spawnType[ARTY] = 4;
					spawnType[FAST] = 3;
					spawnType[BIG] = 3;
					spawnType[CLUSTER] = 2;
					spawnType[LASM] = 2;
					spawnType[BOMBER] = 1;
					spawnType[HELI] = 1;
					spawnType[POPUP] = 3;
					spawnType[PLANE] = 1;
					
					spawnDelay = SEC * 4;
					spawnMin = SEC * .25;
					spawnMax = -SEC * 2.5;
					spawnRandom = .99;
					spawnDouble = .8;
					spawnTriple = .4;
				break;
				// Sudden Death
				case 23:
					enemiesRemaining = 20;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_CORNER_SMALL, R_TOP_LEFT, R_TOP_CENTER];

					// set spawn probabilities
					spawnType[MISSILE] = 1;
					
					spawnDelay = SEC * .5;
					spawnMin = SEC * .1;
					spawnMax = -SEC * 2;
					spawnRandom = .90;
					spawnDouble = .85;
					spawnTriple = .5;
				break;
				// Satellite
				case 24:
					enemiesRemaining = 6;

					// enable projectiles and regions
					spawnLoc[SAT] = [R_LEFT_TOP];

					// set spawn probabilities
					spawnType[SAT] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 3;
					spawnMax = -SEC * 6;
					spawnRandom = .94;
					spawnDouble = .25;
					spawnTriple = 0;
				break;
				// Low and High
				case 25:
					enemiesRemaining = 25;

					// enable projectiles and regions
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[POPUP] = [R_SEA];

					// set spawn probabilities
					spawnType[LASM] = 1;
					spawnType[POPUP] = 1;
					
					spawnDelay = SEC * 1;
					spawnMin = SEC * .5;
					spawnMax = -SEC * 2;
					spawnRandom = .91;
					spawnDouble = .6;
					spawnTriple = .3;
				break;
				// assault
				case 26:
					enemiesRemaining = 75;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_LEFT_CENTER];
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_CORNER_SMALL];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[BOMBER] = [R_LEFT_LASM];
					spawnLoc[HELI] = [R_LEFT_CENTER];
					spawnLoc[SAT] = [R_LEFT_TOP];
					spawnLoc[POPUP] = [R_SEA];
					spawnLoc[PLANE] = [R_RIGHT_LASM];

					// set spawn probabilities
					spawnType[MISSILE] = 8;
					spawnType[ARTY] = 5;
					spawnType[FAST] = 5;
					spawnType[BIG] = 5;
					spawnType[CLUSTER] = 4;
					spawnType[LASM] = 4;
					spawnType[BOMBER] = 2;
					spawnType[HELI] = 2;
					spawnType[SAT] = 1;
					spawnType[POPUP] = 4;
					spawnType[PLANE] = 1;
					
					spawnDelay = SEC * 4;
					spawnMin = SEC * .2;
					spawnMax = -SEC * 2;
					spawnRandom = .97;
					spawnDouble = .85;
					spawnTriple = .4;
				break;
				// Vehicles
				case 27:
					enemiesRemaining = 25;

					// enable projectiles and regions
					spawnLoc[BOMBER] = [R_LEFT_LASM];
					spawnLoc[HELI] = [R_LEFT_CENTER];
					spawnLoc[SAT] = [R_LEFT_TOP];
					spawnLoc[PLANE] = [R_RIGHT_LASM];

					// set spawn probabilities
					spawnType[BOMBER] = 4;
					spawnType[HELI] = 6;
					spawnType[SAT] = 1;
					spawnType[PLANE] = 2;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 3;
					spawnRandom = .94;
					spawnDouble = .5;
					spawnTriple = 0;
				break;
				// Missile Blitz
				case 28:
					enemiesRemaining = 35;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_LARGE];
					spawnLoc[BIG] = [R_CORNER_LARGE, R_LEFT_CENTER];
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_CORNER_SMALL];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[POPUP] = [R_SEA];

					// set spawn probabilities
					spawnType[MISSILE] = 2;
					spawnType[FAST] = 1;
					spawnType[BIG] = 1;
					spawnType[CLUSTER] = 1;
					spawnType[LASM] = 1;
					spawnType[POPUP] = 1;
					
					spawnDelay = SEC * 2;
					spawnMin = SEC * .5;
					spawnMax = -SEC * 1.5;
					spawnRandom = .94;
					spawnDouble = .8;
					spawnTriple = .4;
				break;
				// Satellite Mayhem
				case 29:
					enemiesRemaining = 12;

					// enable projectiles and regions
					spawnLoc[SAT] = [R_LEFT_TOP];

					// set spawn probabilities
					spawnType[SAT] = 1;
					
					spawnDelay = SEC * 3;
					spawnMin = SEC * 1;
					spawnMax = -SEC * 4;
					spawnRandom = .91;
					spawnDouble = .5;
					spawnTriple = 0;
				break;
				// The Final Day
				// case 30:
				default:
					enemiesRemaining = 100;

					// enable projectiles and regions
					spawnLoc[MISSILE] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[ARTY] = [R_ARTY_NORM];
					spawnLoc[FAST] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_SMALL, R_TOP_LEFT];
					spawnLoc[BIG] = [R_LEFT_TOP, R_LEFT_CENTER, R_CORNER_LARGE, R_TOP_LEFT, R_TOP_CENTER];
					spawnLoc[CLUSTER] = [R_LEFT_TOP, R_LEFT_CENTER, R_LEFT_BOT, R_CORNER_SMALL];
					spawnLoc[LASM] = [R_LEFT_LASM];
					spawnLoc[POPUP] = [R_SEA];
					spawnLoc[HELI] = [R_LEFT_CENTER];
					spawnLoc[BOMBER] = [R_LEFT_LASM];
					spawnLoc[SAT] = [R_LEFT_TOP];
					spawnLoc[PLANE] = [R_RIGHT_LASM];

					// set spawn probabilities
					spawnType[MISSILE] = 8;
					spawnType[ARTY] = 5;
					spawnType[FAST] = 5;
					spawnType[BIG] = 5;
					spawnType[CLUSTER] = 4;
					spawnType[LASM] = 4;
					spawnType[BOMBER] = 2;
					spawnType[HELI] = 2;
					spawnType[SAT] = 1;
					spawnType[POPUP] = 4;
					spawnType[PLANE] = 1;
					
					spawnDelay = SEC * 5;
					spawnMin = SEC * .1;
					spawnMax = -SEC * 1.5;
					spawnRandom = .97;
					spawnDouble = .9;
					spawnTriple = .6;
				//break;
			}
			
			sunsetAmt = int(enemiesRemaining * .3);
			nightAmt = int(enemiesRemaining * .15);
		}

		// no argument	instantaneous day
		// "day"		day -> sunset
		// "sunset"		sunset -> night
		public function advanceTime(t:String = null):void
		{
			if (!t)
			{
				cg.game.bg.sky.gotoAndStop(1);
				cg.game.bg.ocean.gotoAndStop(1);
				return;
			}
			cg.game.bg.sky.gotoAndPlay(t);
			cg.game.bg.ocean.gotoAndPlay(t);
		}

		public function endWave():void
		{
			waveActive = false;
			wave++;
			cg.endWave();
		}

		override public function step():void
		{
			if (!waveActive) return;
			
			// if no more spawning, check if no enemy threats are left
			if (enemiesRemaining == 0)
			{
				var done:Boolean = true;
				
				var proj:Array = cg.getProjectileArray();
				var p:ABST_Missile;
				for (var i:int = proj.length - 1; i >= 0; i--)
				{
					p = proj[i];
					if (p.type == 0)
					{
						done = false;
						break;
					}
				}
				if (done)
					endWave();
				return;
			}

			spawnDelay--;

			var spawnAmt:int = 1;
			if (Math.random() < spawnTriple)
				spawnAmt = 3;
			else if (Math.random() < spawnDouble)
				spawnAmt = 2;
				
			for (var t:int = 0; t < spawnAmt; t++)		
				if ((spawnDelay <= 0 && Math.random() > spawnRandom) || spawnDelay < spawnMax)
				{
					switch (choose(spawnType))
					{
						case MISSILE:
							manMiss.spawnProjectile("standard", getSpawnLocation(MISSILE), getTarget());
						break;
						case ARTY:
							manArty.spawnProjectile("artillery", getSpawnLocation(ARTY), getTarget());
						break;
						case FAST:
							manMiss.spawnProjectile("fast", getSpawnLocation(FAST), getTarget(), 0, P_FAST);
						break;
						case BIG:
							manMiss.spawnProjectile("big", getSpawnLocation(BIG), getTarget(), 0, P_BIG);
						break;
						case CLUSTER:
							manMiss.spawnProjectile("cluster", getSpawnLocation(CLUSTER), getTarget());
						break;
						case LASM:
							manMiss.spawnProjectile("LASM", getSpawnLocation(LASM), getTarget());
						break;
						case POPUP:
							manMiss.spawnProjectile("pop", getSpawnLocation(POPUP), getTarget());
						break;
						case BOMBER:
							manMiss.spawnProjectile("bomber", getSpawnLocation(BOMBER), getTarget());
						break;
						case SAT:
							manMiss.spawnProjectile("satellite", getSpawnLocation(SAT), getTarget());
						break;
						case HELI:
							manMiss.spawnProjectile("helicopter", getSpawnLocation(HELI), new Point(getRand(-75, 75), getRand(-50, 50)));
						break;
						case PLANE:
							manMiss.spawnProjectile("plane", getSpawnLocation(PLANE), new Point(-500, 0));
						break;
						case SHIP:
							manMiss.spawnProjectile("ship", getSpawnLocation(SHIP), new Point(300, 0));
						break;
						default:
							trace("WARN! Didn't spawn anything...");
					}								  
					
					spawnDelay = spawnMin;
					enemiesRemaining--;
		
					// advance time from day to sunset
					if (dayFlag == 0 && enemiesRemaining == sunsetAmt)
					{
						dayFlag++;
						advanceTime("day");
					}
					// advance time from sunset to night
					else if (dayFlag == 1 && enemiesRemaining == nightAmt)
					{
						if (cg.game.bg.ocean.currentFrame < 152)		// magic number :c
							return;
						cg.game.bg.cacheAsBitmap = false;
						
						var oldVals:Array = [cg.cursor.tf_pr.text, cg.cursor.tf_se.text, cg.cursor.tf_sp.text];
						
						cg.cursor.gotoAndStop(2);
						cg.cursor.tf_pr.text = oldVals[0];
						cg.cursor.tf_se.text = oldVals[1];
						cg.cursor.tf_sp.text = oldVals[2];
						
						dayFlag++;
						advanceTime("sunset");
					}
				}
			// end for
	
			if (spawnDelay < spawnMax)		// ???
				spawnDelay = spawnMin;
		}
		
		/**	Determines random spawn location based on rules for a given projectile type
		 * 
		 * @param	type	name of projectile to spawn (ex "missile")
		 * @return			restricted randomly-generated spawn point
		 */
		private function getSpawnLocation(type:int):Point
		{
			var regions:Array = spawnLoc[type];
			if (!regions) return null;
			
			var region:Array = regions[int(getRand(0, regions.length - 1))];
			return new Point(getRand(region[0].x, region[1].x), getRand(region[0].y, region[1].y));
		}
		
		private function getTarget():Point
		{
			return new Point(targetX + getRand(-targetVarianceX, targetVarianceX),
							 targetY + getRand(-targetVarianceY, targetVarianceY));
		}
		
		// picks 1 index given an array of choices with elements as weights
		private function choose(choiceWeights:Array):int
		{
			var sum:int = 0;
			var i:int;
			for (i = 0; i < choiceWeights.length; i++)
				sum += choiceWeights[i];
				
			var rand:int = getRand(0, sum-1);
			for (i = 0; i < choiceWeights.length; i++)
			{
				if (rand < choiceWeights[i])
					return i;
				rand -= choiceWeights[i];
			}
			
			trace("WARN! choose() returning -1...");
			return -1;
		}
		
		private function resetSpawnType():void
		{
			if (spawnType)
			{
				for (var i:int = 0; i < spawnType.length; i++)
					spawnType[i] = 0;
			}
			else
				spawnType = [];
		}
	}
}