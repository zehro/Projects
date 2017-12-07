// Manages all Container objects

package 
{
	import flash.display.MovieClip;
	import flash.ui.Mouse;
	import flash.events.Event;
	import flash.media.Sound;
	import flash.media.SoundTransform;
	import flash.media.SoundChannel;
	import flash.media.SoundMixer;
	import flash.events.KeyboardEvent;
	import flash.display.Stage;
	import flash.display.StageQuality;
	import flash.net.SharedObject;

	public class Engine extends MovieClip
	{
		public var gameState:int = 1;		// controls which Container to create next
		public var container:Container;		// currently active Container
		
		public var epTitle:Array = ["Standard Sector", "Easy Sector", "Hard Sector", "Training", "Mineral Rush",
									"Credits"];
		public var epID:Array = ["S-T", "S-E", "S-H", "TR","S-R",
								 "CR"];
		public var epDesc:Array = ["Normal difficulty. 3 Sectors.\nAbout 7:30 playtime. +50% mineral bonus.",
								   "Easy difficulty. 3 Sectors.\nAbout 7:00 playtime. +50% mineral bonus.",
								   "Hard difficulty. 3 Sectors.\nAbout 9:00 playtime.",
								   "Learn how to control your fleet.",
								   "Unlimited waves of increasing difficulty.\nGather as many minerals as you can.",
								   
								   "See who helped to make this game."];
		/*public var epTitle:Array = ["Standard Sector", "Easy Sector", "Hard Sector", "Training", "Survival", "Mineral Rush",
									"Hangar", "Cinema", "Sound Booth", "Credits"];
		public var epID:Array = ["S-T", "S-E", "S-H", "TR", "S-V", "S-R",
								 "HG", "CN", "SD", "CR"];
		public var epDesc:Array = ["Normal difficulty. 3 Sectors.\nAbout 7:30 playtime.",
								   "Easy difficulty. 3 Sectors.\nAbout 7:00 playtime.",
								   "Hard difficulty. 3 Sectors.\nAbout 9:00 playtime.",
								   "Learn how to control your fleet.",
								   "Unlimited waves of increasing difficulty. Hold out as long as you can.",
								   "Unlimited waves of increasing difficulty. Gather as many minerals as you can.",
								   "View defensive structures and ships.",
								   "View cutscenes and other movies.",
								   "Listen to the game's music.",
								   "See who helped to make this game."];*/
		public var episode:String;
		public var difficulty:int;
		public var returnState:int;
		public var menuLocation:Number = 0;
		public var soundVolume:Number = 1;
		public var autosave:Boolean;
		public var SAVE_DATA:String = "APOLUMI";
		public var saveData:SharedObject = SharedObject.getLocal(SAVE_DATA, "/");
		
		public var muteMenuMusic:Boolean = true;
		
		public var ws:String = "quit";
		
		public var bgmMenu:Sound = new BGM_Menu();
		public var bgmDarkSpace:Sound = new BGM_DarkSpace();
		public var bgmChannel:SoundChannel = new SoundChannel();
		
		public function Engine()
		{
			container = new ContainerMenu();
			bgmChannel = bgmMenu.play(0, int.MAX_VALUE);		//SoundMixer.stopAll();
			
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			addChild(container);
			loadGame();
			addEventListener(Event.ENTER_FRAME, step);
			addEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
			addEventListener(KeyboardEvent.KEY_DOWN, onKeyPress);
		}
		
		public function step(e:Event):void
		{
			if (!container.step())		// step the container and check if it is finished
				return;
			removeChild(container);
			switch (gameState)
			{
				case 0:
					container = new ContainerMenu();
					gameState++;
				break;
				case 1:
					var special:String = null;
					switch (episode)
					{
						case "HG":
							container = new ContainerHangar();
							gameState = 0;
						break;
						case "CR":
							container = new ContainerCredits();
							gameState = 0;
						break;
						case "TR":
							special = "end";
						default:
							container = new ContainerIntro(special);
							SoundMixer.stopAll();
							bgmChannel.stop();
							gameState++;
					}
				break;
				case 2:
					Mouse.hide();
					switch (episode)
					{
						case "S-T":
							container = new ContainerGame("vanilla1");
						break;
						case "S-E":
							container = new ContainerGame("easy1");
						break;
						case "S-H":
							container = new ContainerGame("hard1");
						break;
						case "S-R":
							container = new ContainerGame("mineral");
						break;
						case "TR":
							container = new ContainerGame("tutorial");
						break;
					}
					
					bgmChannel = bgmDarkSpace.play(0, int.MAX_VALUE);
					gameState++;
				break;
				case 3:
					Mouse.show();
					//trace(ws);
					container = new ContainerOutro(ws);
					SoundMixer.stopAll();
					bgmChannel.stop();
					gameState++;
				break;
				case 4:
					gameState = 1;
					container = new ContainerMenu();
					bgmChannel = bgmMenu.play(0, int.MAX_VALUE);
				break;
			}
			addChild(container);		
		}

		private function onAddedToStage(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyPress);
		}
		
		private function onKeyPress(e:KeyboardEvent):void
		{
			if (e.keyCode == 49)		// -- 1
				stage.quality = StageQuality.LOW;
			else if (e.keyCode == 50)	// -- 2
				stage.quality = StageQuality.MEDIUM;
			else if (e.keyCode == 51)	// -- 3
				stage.quality = StageQuality.HIGH;
		}
		
		function loadGame():void
		{
			if (saveData.data.sd_isValid == null)
			{
				//trace("No saved game detected. Making a new one!");
				newSaveGame();
				return;
			}
			//trace("Okay, trying to load this SOB.");
			var sd:Object;
			for (var i:int = 0; i < epID.length; i++)
			{
				try
				{
					sd = saveData.data.sd_sectArr[i];
					//trace("Sector at index " + i);
					//trace("Score: " + (sd.score == null ? "0" : sd.score));
					//trace("Rank:  " + (sd.rank == null ? "-" : sd.rank));
				}
				catch (e:Error)
				{
					//trace(">Error at index " + i);
					//trace(e.getStackTrace())
					//trace("Resolving by making a new episode...");
					saveData.data.sd_sectArr.push(new Object());
				}
			}
		}

		public function newSaveGame():void
		{
			saveData.clear();
			saveData.data.sd_sectArr = [];
			for (var i:int = 0; i < epID.length; i++)
				saveData.data.sd_sectArr.push(new Object());
			saveData.data.sd_isValid = true;
			saveData.flush();
		}
	}
}
