package props
{
	import flash.display.MovieClip
	import flash.events.MouseEvent;
	import scenarios.Scenario;
	import flash.events.Event;

	public class UITutorial extends MovieClip
	{
		private var par:MovieClip;
		private var scen:Scenario;
		private var wavesStarted:Boolean;
		private var spawnDelay:int;
		
		public function UITutorial(_par:MovieClip, _scen:Scenario)
		{
			par = _par;
			scen = _scen;
			addEventListener(Event.ENTER_FRAME, step);
			addEventListener(Event.REMOVED_FROM_STAGE, destroy);
		}
		
		private function step(e:Event):void
		{
			if (spawnDelay > 0)
				spawnDelay--;
		}
		
		public function spawnAst(e:MouseEvent):void
		{
			if (spawnDelay > 0) return;
			par.placeAsteroid(0, -400, 0, .1, 2, 0);
			spawnDelay = 30;
		}
		
		public function spawnEnemy(e:MouseEvent):void
		{
			if (spawnDelay > 0) return;
			par.spawnKami(3);
			par.guiMan.enemyWarn("3 enemies inbound!");
			spawnDelay = 30;
		}
		
		public function startWaves():void
		{
			if (wavesStarted)
				return;
			par.guiMan.enemyWarn(scen.wave() + " enemies inbound!");
			par.enWave = scen.eneWaveSpace;
			wavesStarted = true;
		}
		
		private function destroy():void
		{
			removeEventListener(Event.ENTER_FRAME, step);
			removeEventListener(Event.REMOVED_FROM_STAGE, destroy);
		}
	}
}
