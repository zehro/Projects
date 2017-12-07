package scenarios
{
	import props.*;
	
	public class ScHard3 extends Scenario
	{
		public function ScHard3(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 180000; //3:00  // length of each stage in ms (and number of stages)
			stageBG = 2;				// index of frame of BG to use
			stageLen = 2600;
			stageHig = 2600;
			diffMod = 2;				// difficulty multiplier
			stageNext = "END";

			// -- asteroid settings
			astSaturationMin = 6;		// spawn asteroids only if current asteroid
			astSaturationMax = 8;		// count is between these values
			astSize = 3;				// asteroid size to spawn
			astStart = 3;
			astVelMin = .6;				// min velocity
			astVelMax = 3;			// max velocity
			astRotMin = 1;				// min rotation speed
			astRotMax = 3;				// max rotation speed

			// -- enemy settings
			eneWaveInit = 600;	//0:20 			// starting wave delay of first enemy wave in ms
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in ms
			eneWaveSpace = 450;	//0:15			// delay between waves
			eneWaveMod = -60;	//0:02			// change in delay between waves
			eneWaveMin = 300;	//0:10			// min delay between waves
			eneDiff = 10;						// current tier of enemies (-1)
			maxDiff = 999;					// max tier of enemies
		}
		
		override protected function childWave():int
		{
			var num:int;
			num += cg.spawnKami(7 + getRand(eneDiff, eneDiff * 2));
			if (eneDiff > 8)
				for (var i:int = 0; i < int(eneDiff * .5); i++)
					num += cg.spawnKami(3 + getRand(2, 6));
			if (waveCounter % 3 == 1)
				num += cg.spawnDragger(1);
			if (waveCounter % 2 == 0)
				num += cg.spawnShooter(1 + (eneDiff > 10 ? eneDiff - 9 : 0));
			if (waveCounter % 3 == 2)
				num += cg.spawnInterf(1);
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