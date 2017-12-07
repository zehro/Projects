package utils
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.MouseEvent;
	
	public class EpisodeSelector extends MovieClip
	{
		private var X_ANCHOR:int = -191;
		private var Y_ANCHOR:int = -130;
		private var LOWER_Y:int;
		private var Y_SPACING:int = 80;
		private	var MOVE_SPEED:int = 10;
		private	var objArray:Array = [];
		private	var ep:EpisodeGUI;
		private	var moveState:int = 0;
		private	var totalEps:uint;
		private	var i:uint;
		private var obj_scrollUp:Scroller
		private var obj_scrollDn:Scroller
		private var par:MovieClip;
		private var eng:MovieClip;
		
		public function EpisodeSelector()
		{				
			par = MovieClip(parent);
			eng = MovieClip(par.parent);
		
			totalEps = eng.epTitle.length;
			var entry:Object;
			//var readData:Boolean = (eng.saveData.data.sd_isValid != null);
			var sd:Object;
			
			for (i = 0; i < totalEps; i++)
			{
				//if (readData)
				//{
					try
					{
						sd = eng.saveData.data.sd_sectArr[i];
						entry = sd.epEntry;
						
						//isNew = (sd.epEntry == null)
						trace("At [" + i + "], entry is " + entry);
					}
					catch (e:Error)
					{
						trace("At [" + i + "], we fucked up. " + e.getStackTrace());
					}
				//}
				makeEpisode(eng.epID[i], eng.epTitle[i], entry, i);
			}

			obj_scrollDn = new Scroller();
			obj_scrollDn.y = -145;
			addChild(obj_scrollDn);
			obj_scrollUp = new Scroller();
			obj_scrollUp.y = 145;
			addChild(obj_scrollUp);
			
			addEventListener(Event.ENTER_FRAME, step);
			obj_scrollUp.addEventListener(MouseEvent.MOUSE_OVER, scrollUp);
			obj_scrollUp.addEventListener(MouseEvent.MOUSE_OUT, btnOut);
			obj_scrollDn.addEventListener(MouseEvent.MOUSE_OVER, scrollDn);
			obj_scrollDn.addEventListener(MouseEvent.MOUSE_OUT, btnOut);
		}

		private function step(e:Event):void
		{			
			if (moveState == 0)
				return;
			if (moveState == -1 && objArray[1].y > -320)	//-520 make more negative to add
			{
				for (i = 0; i < totalEps; i++)
					objArray[i].y -= MOVE_SPEED;
			}
			else if (moveState == 1 && objArray[0].y < Y_ANCHOR)
			{
				for (i = 0; i < totalEps; i++)
					objArray[i].y += MOVE_SPEED;
				if (objArray[1].y && objArray[0].y >= Y_ANCHOR)
					obj_scrollDn.visible = false;
			}
		}
		
		public function buttonPressed(s:String):void
		{
			par.obj_episodeSelected.visible = true;
			par.obj_episodeSelected.gotoAndPlay("fadeIn")
			i = determineIndex(s);
			par.episode = s;
			ep = objArray[i];
			par.obj_episodeSelected.cont.tNumber.text = s;
			par.obj_episodeSelected.cont.tTitle.text = ep.tTitle.text;
			par.obj_episodeSelected.cont.tDesc.text = eng.epDesc[i];
			
			trace("Pressed: " + s + " @ " + i);
			if (ep.entry == null)
				par.obj_episodeSelected.cont.board.visible = false;
			else if (eng.saveData.data.sd_isValid != null && ep.entry != null)
				ep.feedData(par.obj_episodeSelected.cont);
		}
		
		private function scrollUp(event:MouseEvent)
		{
			moveState = -1;
			obj_scrollDn.visible = true;
		}
		
		private function scrollDn(event:MouseEvent)
		{
			moveState = 1;
		}
		
		private function btnOut(event:MouseEvent)
		{
			moveState = 0;
		}
	
		private function makeEpisode(n:String, t:String, entry:Object, yP:int):void
		{
			ep = new EpisodeGUI(entry, n);
			eng.saveData.data.sd_sectArr[i].epEntry = ep.entry;
			if (n.indexOf("S-") == -1)
			{
				trace(n + " not a playable sector.");
				ep.entry = null;
				ep.tName.text = "";
				ep.tScore.text = "";
				ep.tBest.text = "";
			}
			else
			{
				trace(n + " playable sector.");
				ep.tName.text = ep.entry["n1"];
				ep.tScore.text = ep.entry["s1"].toString();
			}
			ep.tNumber.text = n;
			ep.tTitle.text = t;
			ep.x = X_ANCHOR;
			ep.y = Y_ANCHOR + Y_SPACING * yP + (eng.menuLocation == 0 ? 0 : eng.menuLocation);
			objArray.push(ep);
			base.addChild(ep);
		}
		
		public function getLocation():Number
		{
			return objArray[0].y - Y_ANCHOR;
		}
		
		private function determineIndex(s:String):int
		{
			return eng.epID.indexOf(s);
		}
	}
}
