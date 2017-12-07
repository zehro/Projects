package scenarios
{
	import props.*;
	
	// vanilla2
	public class ScVanilla3 extends Scenario
	{
		public function ScVanilla3(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 150000; //2:30  // length of each stage in ms (and number of stages)
			stageBG = 2;				// index of frame of BG to use
			stageLen = 2700;
			stageHig = 2700;
			diffMod = 1.5;				// difficulty multiplier
			stageNext = "END";

			// -- asteroid settings
			astSaturationMin = 6;		// spawn asteroids only if current asteroid
			astSaturationMax = 8;		// count is between these values
			astSize = 3;				// asteroid size to spawn
			astStart = 2;
			astVelMin = .3;				// min velocity
			astVelMax = 2.2;			// max velocity
			astRotMin = .4;				// min rotation speed
			astRotMax = 2.6				// max rotation speed

			// -- enemy settings
			eneWaveInit = 900;	//0:30 			// starting wave delay of first enemy wave in ms
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in ms
			eneWaveSpace = 450;	//0:15			// delay between waves
			eneWaveMod = -60;	//0:02			// change in delay between waves
			eneWaveMin = 300;	//0:10			// min delay between waves
			eneDiff = 2;						// current tier of enemies (-1)
			maxDiff = 6;						// max tier of enemies
		}
		
		override protected function childWave():int
		{
			var num:int;
			num += cg.spawnKami(3 + getRand(2, int(eneDiff * .5) * 2));	// make hordes
			num += cg.spawnKami(3 + getRand(2, int(eneDiff * .5) * 2));
			num += cg.spawnKami(3 + getRand(2, int(eneDiff * .5) * 2));
			if (waveCounter % 3 == 0)
				num += cg.spawnDragger(1);
			if (waveCounter % 3 == 1)
				num += cg.spawnInterf(1);
			if (waveCounter % 2 == 1)
			{
				num += cg.spawnShooter(1 + getRand(0, 1));
				num += cg.spawnShooter(1 + getRand(0, 1));
			}
			return num;
		}
		
		override public function spawnStructures():void
		{
			// spawn 1 of each color
			cg.placeAsteroid(-1100, -1100, .1, getRand(-2, 2, false), astSize, 0);
			cg.placeAsteroid(1100, -1100, .1, getRand(-2, 2, false), astSize, 1);
			cg.placeAsteroid(-1100, 1100, .1, getRand(-2, 2, false), astSize, 2);
			cg.placeAsteroid(1100, 1100, .1, getRand(-2, 2, false), astSize, 3);
			
			// place planets			
			var pp:PlanetParallax = new PlanetParallax();
			pp.filters = [f_blur];
			pp.gotoAndStop(3);
			cg.addParallax(pp, .2, 200, 600);
		}
	}
}