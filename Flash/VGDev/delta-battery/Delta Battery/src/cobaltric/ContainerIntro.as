package cobaltric
{
	import flash.display.MovieClip;
	import flash.events.MouseEvent;
	import deltabattery.SoundPlayer;

	/**
	 * Main menu screen.
	 * 
	 * @author Alexander Huynh
	 */
	public class ContainerIntro extends ABST_Container
	{
		public var menu:ContainerMenu;
		public var lvl:uint;
		private var eng:Engine;
		
		private var arrName:Array;
		private var arrDay:Array;
		private var arrMoney:Array;
		
		private var displaceRank:int;
		private var resetFlag:Boolean = false;		// reset scores confirmation
		
		public function ContainerIntro(_eng:Engine, scoreArr:Array, newArr:Array = null)
		{
			super();
			eng = _eng;
			
			menu = new ContainerMenu();
			addChild(menu);
			
			menu.menu_credits.visible = false;
			menu.mc_high.confirm.visible = false;
			menu.menu_main.btn_level0.addEventListener(MouseEvent.CLICK, onLevel);
			menu.menu_main.btn_level1.addEventListener(MouseEvent.CLICK, onLevel);
			menu.menu_main.btn_level2.addEventListener(MouseEvent.CLICK, onLevel);
			menu.menu_main.btn_credits.addEventListener(MouseEvent.CLICK, onButton);
			menu.menu_main.btn_high.addEventListener(MouseEvent.CLICK, onHigh);
			menu.menu_credits.btn_back.addEventListener(MouseEvent.CLICK, onButton);
			
			menu.menu_main.btn_level0.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_level1.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_level2.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_level2.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_credits.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_high.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_credits.btn_back.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			menu.mc_high.btn_quit.addEventListener(MouseEvent.CLICK, onQuit);
			menu.mc_high.btn_quit.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.mc_high.btn_reset.addEventListener(MouseEvent.CLICK, onReset);
			
			menu.mc_high.newHigh.btn_ok.addEventListener(MouseEvent.CLICK, onOK);
			menu.mc_high.newHigh.btn_ok.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.mc_high.btn_reset.addEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			SoundPlayer.play("sfx_launch_voice");
			
			menu.mc_high.visible = false;
			menu.mc_high.newHigh.visible = false;
			
			var h:MovieClip = menu.mc_high
			arrName = [h.tf_name1, h.tf_name2, h.tf_name3, h.tf_name4, h.tf_name5, h.tf_name6, h.tf_name7, h.tf_name8];
			arrDay = [h.tf_day1, h.tf_day2, h.tf_day3, h.tf_day4, h.tf_day5, h.tf_day6, h.tf_day7, h.tf_day8];
			arrMoney = [h.tf_money1, h.tf_money2, h.tf_money3, h.tf_money4, h.tf_money5, h.tf_money6, h.tf_money7, h.tf_money8];

			if (newArr)		// [day, money]
			{
				//trace("Got new data: " + newArr);
				
				// day is 99 if Win
				
				var rank:int;
				for (rank = 9; rank > 1; rank--)
				{
					//trace("\tComparing: " + newArr[0] + "/$" + newArr[1] + "\t" + scoreArr[rank-2][1] + "/$" + scoreArr[rank-2][2]);
					if (newArr[0] < scoreArr[rank-2][1])
						break;
					else if (newArr[0] == scoreArr[rank-2][1] && newArr[1] <= scoreArr[rank-2][2])
						break;
				}
				
				//trace("Rank to be displaced is: " + rank);
				if (rank < 9)
				{
					for (var r:int = 8; r > rank; r--)
					{
						//trace("Displacing rank: " + r);
						scoreArr[r - 1][0] = scoreArr[r - 2][0];
						scoreArr[r - 1][1] = scoreArr[r - 2][1];
						scoreArr[r - 1][2] = scoreArr[r - 2][2];
					}
					
					scoreArr[rank - 1][0] = "Anonymous";
					scoreArr[rank - 1][1] = newArr[0];
					scoreArr[rank - 1][2] = newArr[1];
					
					var nth:String;
					switch (rank)
					{
						case 1: nth = "1st"; break;
						case 2: nth = "2nd"; break;
						case 3: nth = "3rd"; break;
						case 4: nth = "4th"; break;
						case 5: nth = "5th"; break;
						case 6: nth = "6th"; break;
						case 7: nth = "7th"; break;
						case 8: nth = "8th"; break;
					}
					menu.mc_high.newHigh.tf_rank.text = nth;
					
					menu.mc_high.newHigh.tf_name.text = "Anonymous";
					menu.mc_high.newHigh.tf_day.text = newArr[0] == 99 ? "Win" : newArr[0];
					menu.mc_high.newHigh.tf_money.text = "$" + newArr[1];
					displaceRank = rank - 1;
					
					menu.mc_high.visible = true;
					menu.mc_high.newHigh.visible = true;
				}
				
			}
			else
				SoundPlayer.playBGM(true);
			
			for (var i:int = 0; i < 8; i++)
			{
				arrName[i].text = scoreArr[i][0];
				arrDay[i].text = scoreArr[i][1] == 99 ? "Win" : scoreArr[i][1];
				arrMoney[i].text = "$" + scoreArr[i][2];
			}
		}
		
		private function onLevel(e:MouseEvent):void
		{			
			switch (e.target)
			{
				case menu.menu_main.btn_level0:
					lvl = 1;
				break;
				case menu.menu_main.btn_level1:
					lvl = 8;
				break;
				case menu.menu_main.btn_level2:
					lvl = 18;
				break;
			}

			onButton(e);
			
			menu.menu_main.btn_level0.removeEventListener(MouseEvent.CLICK, onLevel);
			menu.menu_main.btn_level1.removeEventListener(MouseEvent.CLICK, onLevel);
			menu.menu_main.btn_level2.removeEventListener(MouseEvent.CLICK, onLevel);
			menu.menu_main.btn_credits.removeEventListener(MouseEvent.CLICK, onButton);
			menu.menu_main.btn_high.removeEventListener(MouseEvent.CLICK, onButton);
			menu.menu_credits.btn_back.removeEventListener(MouseEvent.CLICK, onButton);
			
			menu.menu_main.btn_level0.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_level1.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_level2.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_level2.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_credits.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_main.btn_high.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.menu_credits.btn_back.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			menu.mc_high.btn_quit.removeEventListener(MouseEvent.CLICK, onQuit);
			menu.mc_high.btn_quit.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.mc_high.btn_reset.removeEventListener(MouseEvent.CLICK, onReset);
			
			menu.mc_high.newHigh.btn_ok.removeEventListener(MouseEvent.CLICK, onOK);
			menu.mc_high.newHigh.btn_ok.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			menu.mc_high.btn_reset.removeEventListener(MouseEvent.MOUSE_OVER, overButton);
			
			menu.destroy();
			eng.startWave = lvl;
			completed = true;
		}
		
		private function onHigh(e:MouseEvent):void
		{
			onButton(e);
			menu.mc_high.visible = true;
		}
		
		private function onQuit(e:MouseEvent):void
		{
			onButton(e);
			menu.mc_high.visible = false;
			
			resetFlag = false;
			menu.mc_high.confirm.visible = false;
			
			if (!SoundPlayer.bgm)
				SoundPlayer.playBGM(true);
		}
		
		private function onReset(e:MouseEvent):void
		{
			onButton(e);
			if (!resetFlag)
			{
				resetFlag = true;
				menu.mc_high.confirm.visible = true;
				return;
			}
			
			eng.newGame();
			
			var scoreArr:Array = eng.scoreData;
			for (var i:int = 0; i < 8; i++)
			{
				arrName[i].text = scoreArr[i][0];
				arrDay[i].text = scoreArr[i][1] == 99 ? "Win" : scoreArr[i][1];
				arrMoney[i].text = "$" + scoreArr[i][2];
			}
			
			resetFlag = false;
			menu.mc_high.confirm.visible = false;
		}
		
		private function onOK(e:MouseEvent):void
		{
			onButton(e);
			menu.mc_high.newHigh.visible = false;
			
			//trace("Name is: " + menu.mc_high.newHigh.tf_name.text);
			arrName[displaceRank].text = eng.scoreData[displaceRank][0] = menu.mc_high.newHigh.tf_name.text;
			eng.save(eng.scoreData);
		}
		
		private function onButton(e:MouseEvent):void
		{
			SoundPlayer.play("sfx_menu_blip");
		}
		
		private function overButton(e:MouseEvent):void
		{
			SoundPlayer.play("sfx_menu_blip_over");
		}
	}
}
