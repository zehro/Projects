package scenarios
{
	import props.*;
	
	// vanilla2
	public class ScVanilla2 extends Scenario
	{
		public function ScVanilla2(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 150000; //2:30  // length of each stage in ms (and number of stages)
			stageBG = 2;				// index of frame of BG to use
			stageLen = 2500;
			stageHig = 2500;
			diffMod = 1.2;				// difficulty multiplier
			stageNext = "vanilla3";

			// -- asteroid settings
			astSaturationMin = 7;		// spawn asteroids only if current asteroid
			astSaturationMax = 12;		// count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 5;
			astVelMin = .3;				// min velocity
			astVelMax = 1.6;			// max velocity
			astRotMin = .3;				// min rotation speed
			astRotMax = 1.4				// max rotation speed

			// -- enemy settings
			eneWaveInit = 600;	//0:20 			// starting wave delay of first enemy wave in ms
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in ms
			eneWaveSpace = 600;	//0:20			// delay between waves
			eneWaveMod = -30;	//0:01			// change in delay between waves
			eneWaveMin = 300;	//0:10			// min delay between waves
			eneDiff = 4;						// current tier of enemies (-1)
			maxDiff = 8;						// max tier of enemies
		}
		
		override protected function childWave():int
		{
			var num:int;
			if (eneDiff <= 7)
				cg.guiMan.warningWarn("New enemy approaching!", 0xFFFF00, true);
			else
				num += cg.spawnKami(2 + getRand(eneDiff, eneDiff * 2));
			if (eneDiff > 6)
				num += cg.spawnKami(3 + getRand(0, eneDiff));
			if (eneDiff == 5 || (eneDiff > 6 && waveCounter % 3 == 0))
				num += cg.spawnDragger(1);
			if (eneDiff == 6 || (eneDiff > 6 && waveCounter % 2 == 0))
				num += cg.spawnShooter(1);
			if (eneDiff == 7 || (eneDiff > 7 && waveCounter % 3 == 1))
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