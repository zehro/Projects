package scenarios
{
	import props.*;
	
	public class ScEasy1 extends Scenario
	{		
		public function ScEasy1(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 150000; //2:30  // length of each stage in ms (and number of stages) 150000
			stageBG = 1;				// index of frame of BG to use
			stageLen = 2300;
			stageHig = 1900;
			diffMod = .5;				// difficulty multiplier
			stageNext = "easy2";
			
			// -- asteroid settings
			astSaturationMin = 4;		// spawn asteroids only if current asteroid
			astSaturationMax = 10;		// 		count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 2;				// number of starting asteroids
			astVelMin = .2;				// min velocity
			astVelMax = .8;				// max velocity
			astRotMin = .1;				// min rotation speed
			astRotMax = .3;				// max rotation speed
			
			// -- enemy settings
			eneWaveInit = 2250; //1:15 mins		// starting wave delay of first enemy wave in frames
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in frames
			eneWaveSpace = 700;	//30 secs		// delay between waves
			eneWaveMod = 0;						// change in delay between waves
			eneWaveMin = 300;					// min delay between waves
			eneDiff = 0;						// current tier of enemies (-1)
			maxDiff = 3;						// max tier of enemies
			
			tipArr = [stageTime - 3000, stageTime - 14000, stageTime - 25000, stageTime - 36000,
					  stageTime - 60000,  stageTime - 80000, stageTime - 100000, 0];
		}
		
		override public function spawnStructures():void
		{
			// create Force Defense Turrets
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, 200, 0, 0, 0, 50, 3));
			cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, -200, 0, 0, 0, 50, 3));	
			
			// create Point Defense Lasers
			cg.strMan.addStruct(new PointDefenseLaser(cg.strMan, cg, 0, 200, 0, 0, 50, 3));
			cg.strMan.addStruct(new PointDefenseLaser(cg.strMan, cg, 0, -200, 0, 0, 50, 3));
			
			// spawn 1 of each color
			cg.placeAsteroid(-700, -700, .05, getRand(-1, 1, false), astSize, 0);
			cg.placeAsteroid(700, -700, .05, getRand(-1, 1, false), astSize, 1);
			cg.placeAsteroid(-700, 700, .05, getRand(-1, 1, false), astSize, 2);
			cg.placeAsteroid(700, 700, .05, getRand(-1, 1, false), astSize, 3);
			
			for (var i:int = 0; i < 4; i++)
				cg.changeMineral(i, 15);
			
			// place planets			
			var pp:PlanetParallax = new PlanetParallax();
			pp.filters = [f_blur];
			pp.gotoAndStop(1);
			cg.addParallax(pp, .2, 900, 500);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.filters = [f_blur];
			pp.gotoAndStop(2);
			cg.addParallax(pp, .2, -1000, 20);
		}
		
		override protected function childWave():int
		{
			// spawn kami enemies
			return cg.spawnKami(1 + getRand(eneDiff, eneDiff + 1));
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