package 
{
	import managers.SoundManager;
	import scenarios.*;
	import flash.display.MovieClip;
	import flash.ui.Mouse;
	import flash.events.Event;
	import flash.net.SharedObject;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.FileReference;
	import flash.utils.ByteArray;

	public class Engine extends MovieClip
	{
		private var gameState:int;
		private var container:Container;
		public var chosenScenario:String;
		
		public var soundMan:SoundManager;
		
		public var returnState:int;
		public var pageTu:Array = ["Teach Me How to Hack", "Timing is Everything", "Tiering Up", "Double Threat", "One Two Punch", "Putting It Together"];
		public var pageSp:Array = ["Serve Me", "Dual-pronged", "Fork It", "Leapfrog"];
		public var pageSu:Array = ["Slipping Away", "Pass it Along", "Taking the Long Way",  "Make a Withdrawal"];
		public var pageFa:Array = ["Server Overload", "Double Back", "Seeing Double", "Sacrifice"];
		public var pageWi:Array = ["Running the Gauntlet", "Ring of Death", "Instant", "Double Triple", "Around the Bend"];
		public var pageCustom:Array = ["Custom"];
		public var levelPages:Array = [pageTu, pageSp, pageSu, pageFa, pageWi];//, pageCustom];
		public var levelTitles:Array = ["TUTORIALS", "SPRING", "SUMMER", "FALL", "WINTER", "CUSTOM"];
		
		public var cleared:Array;		// unlocked stages		0 - locked, 1 - unlocked, 2 - completed
		public var clearPag:int = 0;	// index of last unlocked page
		public var clearInd:int = 0;	// index of level to unlock next
		public var willUnlock:Boolean;	// used with ContainerMenu to deterine if clearing this level unlocks more
		public var unlockAllHack:Boolean;
		
		public var prevPage:int = 0;	// current level's page
		public var prevLevel:int = 0;	// current level's index
		public var playNext:Boolean = false;		// set TRUE by Stage Clear NEXT button
		
		private var loader:URLLoader;
		
		public var SAVE_DATA:String = "TERRORBYTE";
		public var saveData:SharedObject = SharedObject.getLocal(SAVE_DATA, "/");
		
		private var containerCount:int;	// debugging
		
		public function Engine()
		{
			container = new ContainerIntro();
			soundMan = new SoundManager(this);
		
			initCustom();
			
			if (saveData.data.isValid)		// load data
			{
				trace("ENG: Loading data.");
				cleared = saveData.data.cleared;
				clearPag = saveData.data.clearPag;
				clearInd = saveData.data.clearInd;
				unlockAllHack = saveData.data.unlockAllHack;
			}
			else							// new data
			{
				trace("ENG: Creating new data for the first time.");
				newSave();
			}
			
			addChild(container);
			addEventListener(Event.ENTER_FRAME, step);
		}
		
		public function newSave():void
		{
			trace("ENG: Creating new save.");
			saveData.clear();
			cleared = [];
			for (var i:int = 0; i < levelPages.length; i++)
			{
				var locks:Array = [];
				for (var j:int = 0; j < levelPages[i].length; j++)
					locks.push(0);
				cleared.push(locks);
			}
			saveData.data.cleared = cleared;
			saveData.data.clearPag = clearPag = 0;
			saveData.data.clearInd = clearInd = 0;
			saveData.data.unlockAllHack = unlockAllHack = false;
			for (var k:int = 0; k < 2; k++)
				unlockLevel();
			saveData.data.isValid = true;
			saveData.flush();
		}
		
		public function unlockAll():void
		{
			trace("Unlocking ALL levels.");
			while (unlockLevel(false) < levelPages.length)
			{}
			saveData.data.unlockAllHack = unlockAllHack = true;
			saveData.flush();
		}
		
		public function unlockLevel(save:Boolean = true):int
		{
			if (unlockAllHack) 
			{
				trace("unlockAllHack is true, not unlocking anything.");
				return clearPag;
			}
			if (clearPag < levelPages.length)
				cleared[clearPag][clearInd] = (cleared[clearPag][clearInd] == 2 ? 2 : 1);
			clearInd++;
			trace("clearPag: " + clearPag + " levelPages: " + levelPages);
			if (clearInd >= levelPages[clearPag].length)
			{
				clearInd = 0;
				clearPag++;
			}
			saveData.data.cleared = cleared;
			saveData.data.clearPag = clearPag;
			saveData.data.clearInd = clearInd;
			trace("ENG: Level unlocked. Next page/level to unlock: " + clearPag + "/" + clearInd);
			if (save)
				saveData.flush();
			return clearPag;
		}
		
		private function initCustom():void
		{
			loader = new URLLoader();
			try
			{
				loader.load(new URLRequest("levels.txt"));
				loader.addEventListener(Event.COMPLETE, onFileLoaded);
			} catch (e:Error)
			{
				trace("Failed to open levels.txt");
			}
		}
		
		private function onFileLoaded(e:Event):void
		{
			//trace("Loaded levels.txt.");
			try
			{
				loader.removeEventListener(Event.COMPLETE, onFileLoaded);
				//var fileReference:FileReference = event.target as FileReference;
				//trace(event.target.data);
				var levelArr:Array = e.target.data.split("\n");
				//trace(levelArr.toString());
				for (var s:String in levelArr)
				{
					try
					{
						//trace(s);
						pageCustom.push(s);
					} catch (e:Error)
					{
						
					}
				}
			} catch (e:Error)
			{
				trace("lol nope");
			}
		}
		
		private function loadGame():void
		{
			var scenario:Scenario;
			switch (chosenScenario)
			{
				case "Custom": 					scenario = new ScCustom();			break;
				case "Teach Me How to Hack": 	scenario = new ScTutorial01();		break;
				case "Timing is Everything": 	scenario = new ScTutorial02();		break;
				case "Tiering Up": 				scenario = new ScTutorial03();		break;
				case "Double Threat": 			scenario = new ScTutorial04();		break;
				case "One Two Punch": 			scenario = new ScTutorial05();		break;
				case "Putting It Together": 	scenario = new ScTutorial06();		break;
				case "Serve Me": 				scenario = new ScBasics01();		break;
				case "Dual-pronged": 			scenario = new ScBasics02();		break;
				case "Fork It": 				scenario = new ScBasics03();		break;
				case "Leapfrog": 				scenario = new ScBasics04();		break;
				case "Double Triple": 			scenario = new ScBasics05();		break;
				case "Running the Gauntlet": 	scenario = new ScBasics06();		break;
				case "Slipping Away":		 	scenario = new ScSpring01();		break;
				case "Taking the Long Way":		scenario = new ScSpring02();		break;
				case "Pass it Along":			scenario = new ScSpring03();		break;
				case "Make a Withdrawal":		scenario = new ScSpring04();		break;
				case "Server Overload":			scenario = new ScSpring05();		break;
				case "Double Back":				scenario = new ScSpring06();		break;
				case "Sacrifice":				scenario = new ScSpring07();		break;
				case "Ring of Death":			scenario = new ScSpring08();		break;
				case "Seeing Double":			scenario = new ScSpring09();		break;
				case "Instant":					scenario = new ScHard01();			break;
				case "Around the Bend":			scenario = new ScHard02();			break;
				
				
				//case "Basics": 					scenario = new ScTutSimple();		break;
				//case "The Long Way": 			scenario = new ScRiverMountain();	break;

				default: scenario = new ScTutSimple(); trace(">>WARNING - scenario not found");
			}
			
			container = new ContainerGame(this, scenario);
			(container as ContainerGame).levelName = chosenScenario;
			scenario.setCG(container as ContainerGame);
			
			(container as ContainerGame).cnt = containerCount++;
		}
		
		public function setScenario(s:String):void
		{
			chosenScenario = s;
			prevLevel = levelPages[prevPage].indexOf(s);
		}
		
		public function step(e:Event):void
		{
			if (!container.step())
				return;
			removeChild(container);
			container = null;
			switch (gameState)
			{
				case 0:
					//soundMan.playBGM("BGM_main");			// -- TODO reenable
					container = new ContainerMenu(this);
					gameState++;
				break;
				case 1:					
					loadGame();					
					gameState++;
				break;
				case 2:
					if (playNext)
					{
						trace("PLAY NEXT");
						prevLevel++;
						// go to next page
						if (prevLevel == levelPages[prevPage].length)
						{
							prevLevel = 0;
							prevPage++;
						}
						// no more levels
						if (prevPage == levelPages.length)
						{
							trace("No more levels...");
							prevPage = prevLevel = 0;
							gameState = 1;
							container = new ContainerMenu(this, 0);
						}
						// play next level
						else
						{
							gameState = 2;
							willUnlock = cleared[prevPage][prevLevel] == 1;
							setScenario(levelPages[prevPage][prevLevel]);
							loadGame();
						}
					}
					else
					{
						if (returnState == 1)
						{
							gameState = 2;
							loadGame();
						}
						else
						{
							gameState = 1;
							container = new ContainerMenu(this, 0);
						}
						returnState = 0;
					}
					playNext = false;
				break;
			}
			addChild(container);		
		}
	}
}
