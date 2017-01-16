package scenarios
{
	import props.*;
	
	// vanilla1
	public class ScVanilla1 extends Scenario
	{		
		public function ScVanilla1(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 150000; //2:30  // length of each stage in ms (and number of stages) 150000
			stageBG = 1;				// index of frame of BG to use
			stageLen = 2500;
			stageHig = 2000;
			diffMod = .7;				// difficulty multiplier
			stageNext = "vanilla2";
			
			// -- asteroid settings
			astSaturationMin = 4;		// spawn asteroids only if current asteroid
			astSaturationMax = 10;		// 		count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 2;				// number of starting asteroids
			astVelMin = .2;				// min velocity
			astVelMax = 1;				// max velocity
			astRotMin = .2;				// min rotation speed
			astRotMax = .5;				// max rotation speed
			
			// -- enemy settings
			eneWaveInit = 1800; //1:00 mins		// starting wave delay of first enemy wave in frames
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in frames
			eneWaveSpace = 900;	//30 secs		// delay between waves
			eneWaveMod = 0;						// change in delay between waves
			eneWaveMin = 300;					// min delay between waves
			eneDiff = 0;						// current tier of enemies (-1)
			maxDiff = 4;						// max tier of enemies
			
			tipArr = [stageTime - 3000, stageTime - 14000, stageTime - 25000, stageTime - 36000,
					  stageTime - 60000,  stageTime - 80000, stageTime - 100000, 0];
		}
		
		override public function spawnStructures():void
		{
			// create Point Defense Lasers
			cg.strMan.addStruct(new PointDefenseLaser(cg.strMan, cg, 0, -150, 0, 0, 50, 3));
			
			// create Force Defense Turrets
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, 200, 0, 0, 0, 50, 3));
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, -200, 0, 0, 0, 50, 3));
			
			// spawn 1 of each color
			cg.placeAsteroid(-900, -900, .05, getRand(-1, 1, false), astSize, 0);
			cg.placeAsteroid(900, -900, .05, getRand(-1, 1, false), astSize, 1);
			cg.placeAsteroid(-900, 900, .05, getRand(-1, 1, false), astSize, 2);
			cg.placeAsteroid(900, 900, .05, getRand(-1, 1, false), astSize, 3);
			
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
		}
		
		override protected function childWave():int
		{
			// spawn kami enemies
			return cg.spawnKami(2 + getRand(eneDiff, eneDiff * 2));
		}
		
		override public function scenarioStep(timeLeft:int):void
		{
			if (timeLeft < tipArr[tipInd])
			{
				switch (tipInd)
				{
					case 0:	//0:03 elapsed
						cg.setTip("WASD to move\nviewpoint");
					break;
					case 1:	//0:14
						cg.setTip("Laser: Left click\nTractor: Right click");
					break;
					case 2:	//0:25
						cg.setTip("Trace asteroid seams with laser\nCollect minerals with tractor");
					break;
					case 3:	//0:36
						cg.setTip("Buy mothership part\nbefore time expires (B)");
					break;
					case 4:	//1:00
						cg.setTip("Defend against attacking\nenemies! Use laser and turrets!");
					break;
					case 5:	//1:20
						cg.setTip("Tow asteroids back to base\nfor easier mining and defending");
					break;
					case 6:	//1:40
						cg.setTip("After buying ship part,\npress NEXT to end stage early");
					break;
				}
				tipInd++;
			}
		}
	}
}