package cobaltric
{
	import deltabattery.SoundPlayer;
	import flash.display.MovieClip;
	import flash.display.StageQuality;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.net.SharedObject;
	
	/**
	 * Engine.as
	 * 
	 * Primary game loop event firer and state machine.
	 * 
	 * @author Alexander Huynh
	 */
	public class Engine extends MovieClip
	{
		private var gameState:int;				// 0:Intro, 1:Game, 2:Outro
		private var container:ABST_Container;	// the currently active container
		
		public var startWave:int = 1;
		public var scoreArr:Array;		// new score
		
		public var scoreData:Array;		// old scores
		public var allowHigh:Boolean;
		
		public const SAVE_DATA:String = "DELT_BATT";
		public var saveData:SharedObject = SharedObject.getLocal(SAVE_DATA, "/");
		
		public function Engine()
		{
			// try to load save data
			if (saveData.data.sd_isValid)
			{
				scoreData = [];
				for (var i:int = 0; i < 8; i++)
				{
					scoreData.push([saveData.data.sd_name[i],
									saveData.data.sd_day[i],
									saveData.data.sd_money[i]]);
				}
			}
			else
				newGame();
				
				
			gameState = 0;
			
			container = new ContainerIntro(this, scoreData);
			addChild(container);

			// center the container
			container.x = 0;//stage.width * .5;
			container.y = 0;//stage.height * .5;
			
			addEventListener(Event.ENTER_FRAME, step);
			addEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
			
			SoundPlayer.playBGM(true);
				
			//trace(scoreData[0] + " " + scoreData[1] + " " + scoreData[2]);
		}
		
		public function newGame():void
		{
			save();		// new data
			
			scoreData = [[" ", 1, 0], [" ", 1, 0], [" ", 1, 0], [" ", 1, 0],
						 [" ", 1, 0], [" ", 1, 0], [" ", 1, 0], [" ", 1, 0]];
		}
		
		public function step(e:Event):void
		{
			if (!container.step())
				return;

			removeChild(container);
			switch (gameState)
			{
				case 0:
					container = new ContainerGame(this, startWave);
					gameState++;
					SoundPlayer.stopBGM();
					allowHigh = startWave == 1;
				break;
				case 1:
					container = new ContainerIntro(this, scoreData, (allowHigh ? scoreArr : null));
					gameState = 0;
					if (!SoundPlayer.isBGMplaying())
						SoundPlayer.playBGM(true);	
				break;
			}
			
			addChild(container);
			if (gameState == 1)
			{
				container.x = stage.width * .5;
				container.y = stage.height * .5;
			}	
		}
		
		private function onAddedToStage(e:Event):void
		{
			container.removeEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
			container.stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyPress);
		}
		
		private function onKeyPress(e:KeyboardEvent):void
		{
			if (!container.stage)
				return;
			if (gameState == 0 && (container as ContainerIntro).menu.mc_high.visible)
				return;
			if (e.keyCode == 76)		// -- 1
				container.stage.quality = StageQuality.LOW;
			else if (e.keyCode == 77)	// -- 2
				container.stage.quality = StageQuality.MEDIUM;
			else if (e.keyCode == 72)	// -- 3
				container.stage.quality = StageQuality.HIGH;
		}
		
		public function save(scoreData:Array = null):void
		{ 
			saveData.clear();
			if (!scoreData)
				scoreData = [[" ", 1, 0], [" ", 1, 0], [" ", 1, 0], [" ", 1, 0],
							 [" ", 1, 0], [" ", 1, 0], [" ", 1, 0], [" ", 1, 0]];
			saveData.data.sd_name = [];
			saveData.data.sd_day = [];
			saveData.data.sd_money = [];
			for (var i:int = 0; i < 8; i++)
			{
				saveData.data.sd_name.push(scoreData[i][0]);
				saveData.data.sd_day.push(scoreData[i][1]);
				saveData.data.sd_money.push(scoreData[i][2]);
			}
			saveData.data.sd_isValid = true;
			saveData.flush();
			
			//trace("Saved...");
			//trace(saveData.data.sd_name);
			//trace(saveData.data.sd_day);
			//trace(saveData.data.sd_money);
		}
	}
	
}
