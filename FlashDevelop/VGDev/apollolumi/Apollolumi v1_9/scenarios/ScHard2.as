package scenarios
{
	import props.*;
	
	public class ScHard2 extends Scenario
	{
		public function ScHard2(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 180000; //3:00  // length of each stage in ms (and number of stages)
			stageBG = 2;				// index of frame of BG to use
			stageLen = 2500;
			stageHig = 2500;
			diffMod = 1.6;				// difficulty multiplier
			stageNext = "hard2";

			// -- asteroid settings
			astSaturationMin = 7;		// spawn asteroids only if current asteroid
			astSaturationMax = 13;		// count is between these values
			astSize = 3;				// asteroid size to spawn
			astStart = 5;
			astVelMin = .5;				// min velocity
			astVelMax = 2;			// max velocity
			astRotMin = .5;				// min rotation speed
			astRotMax = 2;				// max rotation speed

			// -- enemy settings
			eneWaveInit = 600;	//0:20 			// starting wave delay of first enemy wave in ms
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in ms
			eneWaveSpace = 600;	//0:20			// delay between waves
			eneWaveMod = -30;	//0:01			// change in delay between waves
			eneWaveMin = 450;	//0:15			// min delay between waves
			eneDiff = 4;						// current tier of enemies (-1)
			maxDiff = 12;						// max tier of enemies
		}
		
		override protected function childWave():int
		{
			var num:int;
			num += cg.spawnKami(3 + getRand(eneDiff, eneDiff * 2));
			if (eneDiff > 8)
				for (var i:int = 0; i < int(eneDiff * .5); i++)
					num += cg.spawnKami(3 + getRand(0, 3));
			if (waveCounter % 3 == 1)
				num += cg.spawnDragger(1);
			if (waveCounter % 2 == 0)
				num += cg.spawnShooter(1 + (eneDiff > 3 ? eneDiff - 2 : 0));
			if (waveCounter % 3 == 2)
				num += cg.spawnInterf(1);
			return num;
		}
		
		override public function spawnStructures():void
		{
			// spawn 1 of each color
			cg.placeAsteroid(-1100, -1100, .1, getRand(-2, 2, false), 3, 0);
			cg.placeAsteroid(1100, -1100, .1, getRand(-2, 2, false), 3, 1);
			cg.placeAsteroid(-1100, 1100, .1, getRand(-2, 2, false), 3, 2);
			cg.placeAsteroid(1100, 1100, .1, getRand(-2, 2, false), 3, 3);
			
			// place planets			
			var pp:PlanetParallax = new PlanetParallax();
			pp.filters = [f_blur];
			pp.gotoAndStop(1);
			cg.addParallax(pp, .2, 800, -600);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.filters = [f_blur];
			pp.gotoAndStop(2);
			cg.addParallax(pp, .2, -1100, 200);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.gotoAndStop(4);
			cg.addParallax(pp, .3, 200, -100);
		}
	}
}