package 
{
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.display.MovieClip;

	public class ContainerMenu extends Container
	{
		public var eng:Engine;
		private var counter:int = 60;
		private var consoleState:String = "idle";
		private var consoleIndex:int = 0;
		private var consoleMessages:Array;
		private var CONSOLE_SPEED:int = 2;

		private var tgtString:String = "";
		private var tgtIndex:int = 0;
		private var currIndex:int = 0;
		
		private var levelBtns:Array = [];
		private var levelPage:int = 0;			// page of levels
		
		public function ContainerMenu(_eng:Engine, page:int = -1)
		{
			super();
			eng = _eng;
			consoleMessages = ["TERRORBYTE\nDeveloped by VGDev\n\nSpring 2014\nVideo Game Development Club\n\nTeam Lead\nAlexander Huynh\n\nBackground Music\nDhruv Karunakaran\n\nBackground Music\nAndrew Han\n\nPlaytesting\nPriscilla Han",
							   "AD 2064 - Summer\nCobaltric Industries develops the world's first completely automated, interconnected ground defense network. Despite criticism, testing begins on a small island isolated from the world. Preliminary testing is completed, and the system is officially dubbed the \"Advanced Autonomous Anti-Air Defense Network\", or 4ADefNet for short. Some countries begin to express interest in the system.",
					 		   "AD 2065 - Winter\nThe first country officially incorporates 4ADefNet into its border defenses. Tensions arise from the increased millitary presence, but many others begin to consider it as well. Countries around the world bolster their millitary in a preemptive defensive move.",
					 		   "AD 2066 - Winter\nThe first casualty occurs when an unarmed cargo plane accidentally strays into airspace covered by 4ADefNet. Rather than increased criticism of the system, more and more countries plan to adopt the system into their own defenses. More than 80% of all countries worldwide integrate 4ADefNet systems into their network. Cobaltric Industries works on expanding the network to include all aspects of millitary control.",
					 		   "AD 2067 - Summer\nAn erroneous missile strike from a 4ADefNet turret shoots down six passenger planes on the same day worldwide. At the same time, a small country manages to override the 4ADefNet systems of its neighbors, yielding complete air superiority. Panic ensues.",
					 		   "AD 2067 - Fall\nWar breaks out worldwide, stemming from the purported defects in the 4ADefNet systems as well as the rapid hacking of systems. The global infrastructure becomes shattered. Cobaltric Industries collapses.",
					 		   "AD 2068 - Winter\nThe world's nations are reduced to a shadow of their former selves. Humanity loses control of 4ADefNet after a failed attempt to incorporate a powerful war-planning AI unit. Ground defenses, no longer controllable by humans, go haywire, destroying anything in range. War slows down as all the world's nations become even more crippled. Casualites are high. The defense system, once meant to protect, is given the moniker of the \"Death Net\".",
					 		   "AD 2069 - Spring\nA minor nation develops a possible counter to the uncontrollable 4ADefNet system. Dubbed the 'Terrorbyte', it is a computer virus capable of temporarily shutting down or permanently controlling nearly any device connected to the 4ADefNet. A plan is formed to transfer remaining survivors and supplies to a central location, from which order can be begin to be restored. Extensive testing begins yet again, this time, to determine the exact usages of Terrorbytes on various Death Net locations to allow vital aircraft to reach the haven.\nIt is you who will conduct these scenarios and save humanity."];

			if (page == -1)
				gui_titles.visible = false;
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			addEventListener(Event.REMOVED_FROM_STAGE, destroy);
			btn_play.addEventListener(MouseEvent.CLICK, onPlay);
			btn_clear.addEventListener(MouseEvent.CLICK, onClear);
			btn_advance.addEventListener(MouseEvent.CLICK, onAdvance);
			btn_unlockData.addEventListener(MouseEvent.CLICK, onUnlock);
			
			levelPage = eng.prevPage;
			
			levelBtns = [gui_titles.l0, gui_titles.l1, gui_titles.l2, gui_titles.l3,
						 gui_titles.l4, gui_titles.l5, gui_titles.l6, gui_titles.l7,
						 gui_titles.l8, gui_titles.l9, gui_titles.l10, gui_titles.l11];
			
			updateLevelListing();
			
			gui_titles.btn_title.addEventListener(MouseEvent.CLICK, onTitle);
			gui_titles.btn_next.addEventListener(MouseEvent.CLICK, onNext);
			gui_titles.btn_prev.addEventListener(MouseEvent.CLICK, onPrev);
			gui_titles.btn_tutorial.addEventListener(MouseEvent.CLICK, onTut);
			gui_titles.btn_prev.visible = levelPage != 0;
			gui_titles.btn_next.visible = levelPage != eng.levelPages.length - 1;
			tutorial.visible = false;
		}
		
		private function onTitle(e:MouseEvent)
		{
			gui_titles.visible = false;
		}
		
		private function onTut(e:MouseEvent)
		{
			tutorial.visible = true;
		}
		
		private function onNext(e:MouseEvent)
		{
			levelPage++;
			gui_titles.btn_prev.visible = true;
			if (levelPage == eng.levelPages.length - 1)
				gui_titles.btn_next.visible = false;
			updateLevelListing();
		}
		
		private function onPrev(e:MouseEvent)
		{
			levelPage--;
			gui_titles.btn_next.visible = true;
			if (levelPage == 0)
				gui_titles.btn_prev.visible = false;
			updateLevelListing();
		}
		
		public function updateLevelListing():void
		{
			//trace("Updating level listing...");
			var i:uint = 1;
			trace(eng.cleared);
			for each (var lvl:MovieClip in levelBtns)
			{				
				//trace("Visiting " + eng.levelPages[levelPage][i-1] + "\tStatus: " + eng.cleared[levelPage][i-1]);
				lvl.addEventListener(MouseEvent.ROLL_OVER, overLevel);
				lvl.addEventListener(MouseEvent.ROLL_OUT, outLevel);
				lvl.addEventListener(MouseEvent.CLICK, onLevel);
				lvl.restricted.mouseEnabled = false;
				lvl.restricted.buttonMode = false;
				lvl.cleared.mouseEnabled = false;
				lvl.cleared.buttonMode = false;
				lvl.tf_level.mouseEnabled = false;
				if (i < eng.levelPages[levelPage].length + 1)
				{
					lvl.visible = true;
					lvl.tf_level.text = eng.levelPages[levelPage][i-1];
					lvl.restricted.visible = eng.cleared[levelPage][i-1] == 0;
					lvl.cleared.visible = eng.cleared[levelPage][i-1] == 2;
					//trace("Updating. Restricted/cleared " + lvl.restricted.visible + "/" + lvl.cleared.visible);
				}
				else
					lvl.visible = false;
				i++;
				lvl.folder.gotoAndStop(1);
				lvl.restricted.gotoAndStop(1);
				gui_titles.tf_page.text = (levelPage+1) + "/" + eng.levelPages.length;
				gui_titles.tf_title.text = eng.levelTitles[levelPage];
			}
		}
	
		private function overLevel(e:MouseEvent):void
		{
			e.target.folder.gotoAndStop(2);
			e.target.restricted.gotoAndStop(2);
			eng.soundMan.playSound("SFX_beepSingle");
		}
	
		private function outLevel(e:MouseEvent):void
		{
			e.target.folder.gotoAndStop(1);
			e.target.restricted.gotoAndStop(1);
		}
	
		private function onLevel(e:MouseEvent):void
		{
			try
			{
				var par:MovieClip = e.target.parent;
				if (par.restricted.visible)
				{
					eng.soundMan.playSound("SFX_beepLow2");
					return;
				}
				eng.soundMan.playSound("SFX_menuBeep");
				eng.prevPage = levelPage;
				eng.prevLevel = eng.levelPages[levelPage][eng.levelPages.indexOf(par.tf_level.text)];
				trace("Selected: " + eng.prevPage + "/" + eng.prevLevel);
				eng.setScenario(par.tf_level.text);
				eng.willUnlock = eng.cleared[eng.prevPage][eng.prevLevel] == 1;
				trace("\nwillUnlock = " + eng.willUnlock);
				play();
			}
			catch (err:Error)
			{
				trace("Can't do something with " + e.target);
			}
		}
		
		private function onUnlock(e:MouseEvent):void
		{
			eng.unlockAll();
			updateLevelListing();
			dataUnlocked.gotoAndPlay(2);
		}
		
		private function onPlay(e:MouseEvent):void
		{
			gui_titles.visible = true;
		}
		
		private function onAdvance(e:MouseEvent):void
		{
			consoleState = "idle";
			counter = 0;
		}
		
		private function onClear(e:MouseEvent):void
		{
			clearConfirm.visible = true;
		}
		
		override public function step():Boolean
		{
			if (counter > 0)
				counter--;
				
			switch (consoleState)
			{
				case "idle":
					if (counter == 0)
					{
						consoleState = "stream";
						currIndex = 0;
						tgtString = consoleMessages[consoleIndex];
						consoleIndex++;
						story.gotoAndStop(consoleIndex);
						if (consoleIndex == consoleMessages.length)
							consoleIndex = 0;
						tgtIndex = tgtString.length;
						tf_console.text = "";
					}
				break;
				case "stream":
					if (currIndex == tgtIndex)
					{
						consoleState = "reset";
						counter = 150;
					}
					else
					{
						for (var i:int = 0; i < CONSOLE_SPEED; i++)
						{
							if (currIndex < tgtIndex)
							{
								tf_console.appendText(tgtString.charAt(currIndex));
								currIndex++;
							}
							else
								break;
						}
					}
				break;
				case "reset":
					if (counter == 0)
					{
						consoleState = "idle";
						tf_console.text = "";
						counter = 60;
					}
				break;
			}
			return completed;
		}
		
		private function destroy(e:Event):void
		{
			removeEventListener(Event.REMOVED_FROM_STAGE, destroy);
			btn_play.removeEventListener(MouseEvent.CLICK, onPlay);
			btn_advance.removeEventListener(MouseEvent.CLICK, onAdvance);
			btn_unlockData.removeEventListener(MouseEvent.CLICK, onUnlock);
			btn_clear.removeEventListener(MouseEvent.CLICK, onClear);
		}
	}
}
