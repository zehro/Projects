package scenarios
{
	import props.*;
	
	public class ScHard1 extends Scenario
	{		
		public function ScHard1(_cg:ContainerGame)
		{
			super(_cg);
			
			stageTime = 180000; //3:00  // length of each stage in ms (and number of stages) 150000
			stageBG = 1;				// index of frame of BG to use
			stageLen = 2500;
			stageHig = 2500;
			diffMod = 1.2;				// difficulty multiplier
			stageNext = "hard2";
			
			// -- asteroid settings
			astSaturationMin = 5;		// spawn asteroids only if current asteroid
			astSaturationMax = 14;		// 		count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 4;				// number of starting asteroids
			astVelMin = .5;				// min velocity
			astVelMax = 1.5;				// max velocity
			astRotMin = .3;				// min rotation speed
			astRotMax = 1;				// max rotation speed
			
			// -- enemy settings
			eneWaveInit = 1800; //1:00 mins		// starting wave delay of first enemy wave in frames
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in frames
			eneWaveSpace = 600;	//20 secs		// delay between waves
			eneWaveMod = -30;	//1 sec			// change in delay between waves
			eneWaveMin = 300;	//10 secs		// min delay between waves
			eneDiff = 0;						// current tier of enemies (-1)
			maxDiff = 8;						// max tier of enemies
			
			tipArr = [stageTime - 3000, stageTime - 14000, stageTime - 25000, stageTime - 36000,
					  stageTime - 60000,  stageTime - 80000, stageTime - 100000, 0];
		}
		
		override public function spawnStructures():void
		{
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
			var num:int;
			if (eneDiff >= 3 && eneDiff <= 5)
				cg.guiMan.warningWarn("New enemy approaching!", 0xFFFF00, true);
			else
				num += cg.spawnKami(3 + getRand(0, eneDiff ));
			if (eneDiff > 6)
				num += cg.spawnKami(4 + getRand(0, eneDiff));
			if (eneDiff == 3 || (eneDiff > 3 && waveCounter % 3 == 0))
				num += cg.spawnDragger(1);
			if (eneDiff == 4 || (eneDiff > 4 && waveCounter % 2 == 0))
				num += cg.spawnShooter(1 + (eneDiff > 3 ? eneDiff - 2 : 0));
			if (eneDiff == 5 || (eneDiff > 5 && waveCounter % 3 == 1))
				num += cg.spawnInterf(1);
			return num;
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