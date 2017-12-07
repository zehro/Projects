package scenarios
{
	import props.*;
	
	// tutorial
	public class ScTutorial extends Scenario
	{
	
		public function ScTutorial(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 600000; //10:00 // length of each stage in ms (and number of stages) 150000
			stageBG = 1;				// index of frame of BG to use
			stageLen = 2400;
			stageHig = 2200;
			diffMod = 1;				// difficulty multiplier
			stageNext = "ENDSP";
			
			// -- asteroid settings
			astSaturationMin = 4;		// spawn asteroids only if current asteroid
			astSaturationMax = 6;		// 		count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 2;				// number of starting asteroids
			astVelMin = .2;				// min velocity
			astVelMax = 1;				// max velocity
			astRotMin = .2;				// min rotation speed
			astRotMax = 1;				// max rotation speed
			
			// -- enemy settings
			eneWaveInit = 18000; //10:00 mins	// starting wave delay of first enemy wave in frames
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in frames
			eneWaveSpace = 1200;	//0:40 min	// delay between waves
			eneWaveMod = -60;					// change in delay between waves
			eneWaveMin = 300;					// min delay between waves
			eneDiff = 0;						// current tier of enemies (-1)
			maxDiff = 6;						// max tier of enemies
		}
		
		override public function spawnStructures():void
		{
			// create Point Defense Lasers
			cg.strMan.addStruct(new PointDefenseLaser(cg.strMan, cg, 200, 0, 0, 0, 50, 3));
			cg.strMan.addStruct(new PointDefenseLaser(cg.strMan, cg, -200, 0, 0, 0, 50, 3));
			
			// create Auto-Mineral Collectors
			cg.strMan.addStruct(new AutoCollector(cg.strMan, cg, 0, -150, 0, 0, 35, 3));
			cg.strMan.addStruct(new AutoCollector(cg.strMan, cg, 0, 150, 0, 0, 35, 3));
			
			// create Force Defense Fields
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, 200, -100, 0, 0, 50, 3));
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, 200, 100, 0, 0, 50, 3));
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, -200, -100, 0, 0, 50, 3));
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, -200, 100, 0, 0, 50, 3));
			
			// spawn 1 of each color
			cg.placeAsteroid(-700, -700, .05, getRand(-1, 1, false), astSize, 0);
			cg.placeAsteroid(700, -700, .05, getRand(-1, 1, false), astSize, 1);
			cg.placeAsteroid(-700, 700, .05, getRand(-1, 1, false), astSize, 2);
			cg.placeAsteroid(700, 700, .05, getRand(-1, 1, false), astSize, 3);
			
			// place planets			
			var pp:PlanetParallax = new PlanetParallax();
			pp.filters = [f_blur];
			pp.gotoAndStop(1);
			cg.addParallax(pp, .2, 700, 500);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.filters = [f_blur];
			pp.gotoAndStop(2);
			cg.addParallax(pp, .2, -1000, 20);
			
			var tut:UITutorial = new UITutorial(cg, this);
			tut.x = -287.85;
			tut.y = -177;
			cg.tutorial.addChild(tut);
		}
		
		override protected function childWave():int
		{
			var num:int;
			num += cg.spawnKami(3 + getRand(eneDiff, eneDiff * 2));
			return num;
		}
	}
}