package scenarios
{
	import props.*;
	
	public class ScEasy3 extends Scenario
	{
		public function ScEasy3(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 120000; //2:00  // length of each stage in ms (and number of stages)
			stageBG = 2;				// index of frame of BG to use
			stageLen = 2300;
			stageHig = 2300;
			diffMod = 1;				// difficulty multiplier
			stageNext = "END";

			// -- asteroid settings
			astSaturationMin = 6;		// spawn asteroids only if current asteroid
			astSaturationMax = 8;		// count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 4;
			astVelMin = .3;				// min velocity
			astVelMax = 1.3;			// max velocity
			astRotMin = .4;				// min rotation speed
			astRotMax = 1				// max rotation speed

			// -- enemy settings
			eneWaveInit = 900;	//0:30 			// starting wave delay of first enemy wave in ms
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in ms
			eneWaveSpace = 600;	//0:20			// delay between waves
			eneWaveMod = -30;	//0:01			// change in delay between waves
			eneWaveMin = 450;	//0:15			// min delay between waves
			eneDiff = 2;						// current tier of enemies (-1)
			maxDiff = 6;						// max tier of enemies
		}
		
		override protected function childWave():int
		{
			var num:int;
			num += cg.spawnKami(3 + getRand(2, int(eneDiff * .5) * 2));	// make hordes
			num += cg.spawnKami(3 + getRand(2, int(eneDiff * .5) * 2));
			num += cg.spawnKami(3 + getRand(2, int(eneDiff * .5) * 2));
			if (waveCounter % 4 == 0)
				num += cg.spawnDragger(1);
			if (waveCounter % 4 == 1)
				num += cg.spawnInterf(1);
			if (waveCounter % 3 == 0)
			{
				num += cg.spawnShooter(1 + getRand(0, 1));
				num += cg.spawnShooter(1 + getRand(0, 1));
			}
			return num;
		}
		
		override public function spawnStructures():void
		{
			// spawn 1 of each color
			cg.placeAsteroid(-900, -900, .1, getRand(-2, 2, false), 2, 0);
			cg.placeAsteroid(900, -900, .1, getRand(-2, 2, false), 2, 1);
			cg.placeAsteroid(-900, 900, .1, getRand(-2, 2, false), 2, 2);
			cg.placeAsteroid(900, 900, .1, getRand(-2, 2, false), 2, 3);
			
			// place planets			
			var pp:PlanetParallax = new PlanetParallax();
			pp.filters = [f_blur];
			pp.gotoAndStop(4);
			cg.addParallax(pp, .2, 200, 600);
		}
	}
}