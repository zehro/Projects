package scenarios
{
	import props.*;

	public class ScMineral extends Scenario
	{		
		public function ScMineral(_cg:ContainerGame)
		{
			super(_cg);
			
			// handle endless stuff
			cg.scoreMode = "MINE";
			
			isEndless = true;
			stageBG = 1;				// index of frame of BG to use
			stageLen = 3000;
			stageHig = 3000;
			diffMod = 1;				// difficulty multiplier
			stageNext = "END";
			
			// -- asteroid settings
			astSaturationMin = 9;		// spawn asteroids only if current asteroid
			astSaturationMax = 24;		// 		count is between these values
			astSize = 2;				// asteroid size to spawn
			astStart = 10;				// number of starting asteroids
			astVelMin = .2;				// min velocity
			astVelMax = 1;				// max velocity
			astRotMin = .2;				// min rotation speed
			astRotMax = .5;				// max rotation speed
			
			multHP = 1;			// % of base enemy HP
			multMod = .02;		// % to add to multipliers per wave
			multMax = 999;
			
			// -- enemy settings
			eneWaveInit = 1800; //1:00 mins		// starting wave delay of first enemy wave in frames
			eneWave = eneWaveInit;				// starting wave delay of each stage's enemy wave in frames
			eneWaveSpace = 900;	//30 secs	// delay between waves
			eneWaveMod = -30;	//-1sec			// change in delay between waves
			eneWaveMin = 300;	//10secs		// min delay between waves
			eneDiff = 0;						// current tier of enemies (-1)
			maxDiff = 999;						// max tier of enemies
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
			cg.addParallax(pp, .2, -1500, 70);
			
			pp = new PlanetParallax();
			pp.filters = [f_blur];
			pp.gotoAndStop(3);
			cg.addParallax(pp, .3, -200, 100);
			
			pp = new PlanetParallax();
			pp.scaleX = pp.scaleY = .5;
			pp.gotoAndStop(4);
			cg.addParallax(pp, .2, 800, -600);
		}
		
		override protected function childWave():int
		{
			var num:int;
			num += cg.spawnKami(5 + getRand(0, eneDiff));
			if (eneDiff > 4)
				for (var i:int = 0; i < int(eneDiff * .3); i++)
					num += cg.spawnKami(3 + getRand(2, 3));
			if (eneDiff == 3 || (eneDiff > 4 && waveCounter % 3 == 1))
				num += cg.spawnDragger(1);
			if (eneDiff == 5 || (eneDiff > 5 && waveCounter % 2 == 0))
				num += cg.spawnShooter(1 + int(eneDiff * .25));
			if (eneDiff == 7 || (eneDiff > 7 && waveCounter % 3 == 0))
				num += cg.spawnInterf(1);
				
			if (eneDiff == 3 || eneDiff == 5 || eneDiff == 7)
				cg.guiMan.warningWarn("New enemy approaching!", 0xFFFF00, true);

			if (eneDiff % 4 == 2)
			{
				astVelMin += .2;				// min velocity
				astVelMax += .2;				// max velocity
				astRotMin += .2;				// min rotation speed
				astRotMax += .2;				// max rotation speed
				astSaturationMax += 1;
			}
			if (eneDiff == 10)
				astSize = 3;				// asteroid size to spawn
			return num;
		}
	}
}