package scenarios
{
	import flash.filters.BlurFilter;

	public class Scenario
	{
		protected var cg:ContainerGame;

		// -- stage settings
		public var isEndless:Boolean;
		public var stageTime:int;				// length of stage
		public var stageBG:int;					// index of frame of BG to use
		public var stageLen:int;
		public var stageHig:int;
		public var diffMod:Number = 1;			// difficulty multiplier
		public var stageNext:String;			// next stage
		
		// -- asteroid settings
		public var astSaturationMin:int;		// asteroid forced to spawn if below
		public var astSaturationMax:int;		// asteroids never spawn if above
		public var astSpawn:Number;				// chance to not spawn per tick (e.g: .999)
		public var astSize:int;					// asteroid size to spawn
		public var astStart:int;				// number of starting asteroids
		public var astVelMin:Number;			// min velocity
		public var astVelMax:Number;			// max velocity
		public var astRotMin:Number;			// min rotation speed
		public var astRotMax:Number;			// max rotation speed

		// -- enemy settings
		public var eneWave:int;					// wave delay for enemy waves
		public var eneWaveInit:int;				// starting wave delay of first enemy wave in ms
		public var eneWaveSpace:int;			// delay between waves
		public var eneWaveMod:int;				// change in delay between waves
		public var eneWaveMin:int;				// min delay between waves
		public var eneDiff:int;					// current tier of enemies
		public var maxDiff:int;					// max tier of enemies
		
		public var multHP:Number = 1;			// % of base enemy HP
		//public var multSpd:Number = 1;			// % of base enemy max speed
		//public var multRange:Number = 1;		// % of base enemy range
		public var multMod:Number = 0;			// % to add to multipliers per wave
		public var multMax:Number = 1;
		
		protected var waveCounter:int;			// use modulus to control special waves, etc.
		
		protected var tipArr:Array = [];
		protected var tipInd:int = 0;
		
		protected var f_blur:BlurFilter = new BlurFilter(11, 11);

		public function Scenario(_cg:ContainerGame)
		{
			cg = _cg;
		}
		
		public function spawnStructures():void
		{
			// -- override
		}
		
		public function scenarioStep(timeLeft:int):void
		{
			// -- override
		}
		
		public function wave():int
		{
			waveCounter++;
			if (eneDiff < maxDiff)
				eneDiff++;
			eneWaveSpace -= eneWaveMod;		// modify wave time
			if (eneWaveSpace < eneWaveMin)
				eneWaveSpace = eneWaveMin;
			multHP += multMod;				// modify multipliers
			if (multHP > eneWaveMin)
				multHP = multMax;
			return childWave();
		}
		
		protected function childWave():int
		{
			// -- override
			return 0;
		}
		
		protected function getRand(min:Number, max:Number, useInt:Boolean = true):Number   
		{  
			if (useInt)
				return (int(Math.random() * (max - min + 1)) + min);
			return Math.random() * max + min;
		} 
	}
}