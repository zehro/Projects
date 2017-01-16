package scenarios
{
	import props.*;
	
	public class ScEasy2 extends Scenario
	{
		public function ScEasy2(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 150000; //2:30  // length of each stage in ms (and number of stages)
			stageBG = 1;				// index of frame of BG to use
			stageLen = 2300;
			stageHig = 2000;
			diffMod = .7;				// difficulty multiplier
			stageNext = "easy3";

			// -- asteroid settings
			astSaturationMin = 7;		// spawn asteroids only if current asteroid
			astSaturationMax = 12;		// count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 5;
			astVelMin = .3;				// min velocity
			astVelMax = .9;			// max velocity
			astRotMin = .2;				// min rotation speed
			astRotMax = .7				// max rotation speed

			// -- enemy settings
			eneWaveInit = 900;	//0:30 			// starting wave delay of first enemy wave in ms
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in ms
			eneWaveSpace = 600;	//0:20			// delay between waves
			eneWaveMod = -30;	//0:01			// change in delay between waves
			eneWaveMin = 300;	//0:10			// min delay between waves
			eneDiff = 2;						// current tier of enemies (-1)
			maxDiff = 7;						// max tier of enemies
		}
		
		override protected function childWave():int
		{
			var num:int;
			if (eneDiff <= 5)
				cg.guiMan.warningWarn("New enemy approaching!", 0xFFFF00, true);
			else
				num += cg.spawnKami(1 + getRand(0, eneDiff));
			if (eneDiff > 3)
				num += cg.spawnKami(2 + getRand(0, eneDiff));
			if (eneDiff == 3 || (eneDiff > 3 && waveCounter % 3 == 0))
				num += cg.spawnDragger(1);
			if (eneDiff == 4 || (eneDiff > 4 && waveCounter % 2 == 0))
				num += cg.spawnShooter(1);
			if (eneDiff == 5 || (eneDiff > 5 && waveCounter % 3 == 1))
				num += cg.spawnInterf(1);
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
			pp.gotoAndStop(1);
			cg.addParallax(pp, .2, 800, -600);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.filters = [f_blur];
			pp.gotoAndStop(2);
			cg.addParallax(pp, .2, -900, 200);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.gotoAndStop(4);
			cg.addParallax(pp, .3, 200, -100);
		}
	}
}