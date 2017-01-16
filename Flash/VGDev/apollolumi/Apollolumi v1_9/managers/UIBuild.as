package managers
{
	import flash.display.MovieClip;
	import flash.text.TextField;
	import flash.geom.ColorTransform;
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.display.DisplayObject;
	//import flash.utils.getQualifiedClassName;
	import props.UIIcon;

	public class UIBuild extends MovieClip
	{
		private var cg:MovieClip;
		private var gui:GUIManager;
		
		public var ui:UIIcon;				// currently selected icon
		public var partCost:Vector.<int>;	// mineral cost for part (array length 4)
		
		private var minBars:Vector.<MovieClip> = new Vector.<MovieClip>(4, true);
		private var minCosts:Vector.<MovieClip> = new Vector.<MovieClip>(4, true);
		private var minText:Vector.<TextField> = new Vector.<TextField>(4, true);
		
		public var tier:Number = .5;	// cost multiplier
		private var prevPart:int;		// previous part
		
		private var costNeeded:ColorTransform;
		private var costExtra:ColorTransform;
		
		public function UIBuild()
		{					
			defineStructure(build_fdt, "fdt",
							"Force Defense Turret",
							"Repels all nearby asteroids and enemies.",
							0, 10, 10, 25);
							
			defineStructure(build_mt, "mt",
							"Missile Turret",
							"Attacks farthest enemy with a high-explosive missile.",
							40, 10, 30, 10);
							
			defineStructure(build_pdl, "pdl",
							"Point Defense Laser",
							"Attacks nearest enemy with a high-energy laser.",
							30, 15, 10, 0);
							
			defineStructure(build_int, "int",
							"Interceptor Turret",
							"Shoots down incoming enemy projectiles.",
							0, 10, 30, 20);
							
			defineStructure(build_ebt, "ebt",
							"Energy Blade Turret",
							"Greatly damages objects that touch it. Very heavy and hard to move.\n(Ignores other towers.)",
							15, 40, 0, 10);
							
			defineStructure(build_amc, "amc",
							"Auto-Mineral Collector",
							"Gathers nearby minerals.",
							10, 10, 15, 15);
							
			defineStructure(build_rail, "rail",
							"Railgun",
							"Powerful but slow-firing railgun.\nTargets furthest enemy.",
							60, 20, 60, 30);
			
			costNeeded = new ColorTransform();
			costNeeded.color = 0xFF0000;
			costNeeded.alphaMultiplier = .25;
			
			costExtra = new ColorTransform();
			costExtra.color = 0x00FF00;
			costExtra.alphaMultiplier = .15;
			
			minWarn.visible = false;

			var i:int, j:int = numChildren;
			var obj:DisplayObject;
			for (i = 0; i < j; i++)
			{
				obj = getChildAt(i);
				if (obj is UIIcon)
				{
					obj.addEventListener(MouseEvent.ROLL_OVER, onOvr);
					obj.addEventListener(MouseEvent.ROLL_OUT, onOut);
				}
			}
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			stage.addEventListener(MouseEvent.MOUSE_UP, onDown);
			cg = MovieClip(parent);
			gui = cg.guiMan;
			
			choosePart();
			motherBought.buttonMode = false;
			
			minBars[0] = bar_r;	minText[0] = tf_r;
			minBars[1] = bar_y;	minText[1] = tf_y;
			minBars[2] = bar_g;	minText[2] = tf_g;
			minBars[3] = bar_b;	minText[3] = tf_b;
			
			// -- color each bar
			var ct:ColorTransform = new ColorTransform();
			ct.color = 0xE52727;
			minBars[0].bar.transform.colorTransform = ct;
			ct.color = 0xDCE527;
			minBars[1].bar.transform.colorTransform = ct;
			ct.color = 0x1E913B;
			minBars[2].bar.transform.colorTransform = ct;
			ct.color = 0x2791E5;
			minBars[3].bar.transform.colorTransform = ct;
			
			base.addEventListener(MouseEvent.MOUSE_DOWN, onBase);
			btn_return.addEventListener(MouseEvent.MOUSE_DOWN, onBase);

			clearBars();
			tf_name.text = "Deploy structures";
			tf_des.text = "Mouseover an icon to see stats; click an icon to deploy";
		}
		
		public function choosePart():void
		{
			motherBought.visible = false;
			motherBought.mouseEnabled = false;
			
			var currPart:int;
			do
			{
				currPart = getRand(2, 5);
			} while (prevPart != 0 && prevPart == currPart)
			prevPart = currPart;
			switch (currPart)
			{
				case 2:
					defineStructure(build_mother, "cpu",
									"Central Processing Unit",
									"Vital component.\nControls ship's systems.",
									10 * tier, 10 * tier, 10 * tier, 10 * tier);
				break;
				case 3:
					defineStructure(build_mother, "engine",
									"Engine",
									"Vital component.\nPrimary means of propulsion.",
									5 * tier, 15 * tier, 10 * tier, 10 * tier);
				break;
				case 4:
					defineStructure(build_mother, "wing",
									"Nanocarbon Wing",
									"Vital component.\nAssists movement in environments with an atmosphere.",
									0, 5 * tier, 15 * tier, 20 * tier);
				break;
				case 5:
					defineStructure(build_mother, "life",
									"Life Support",
									"Vital component.\nSustains life aboard the mothership.",
									0, 20 * tier, 0, 20 * tier);
				break;
			}
			// set mineral goal bars in UI Right
			for (var i:int = 0; i < 4; i++)
				cg.guiMan.updateMineralO(i, build_mother.costs[i]);
			cg.objective.objIcon.gotoAndStop(build_mother.currentFrame);
			cg.objective.tf_name.text = build_mother.iconName;
		}
		
		//	updates a mineral's bar and textfield
		//	ind: which mineral		amt: 0-100
		public function updateMineral(ind:int, amt:int):void
		{
			if (amt == 0)
			{
				minBars[ind].bar.width = cg.mineralVec[ind];
				minBars[ind].barO.width = 0;
				minText[ind].text = "000";
				return;
			}

			var cost:int;
			cost = minBars[ind].bar.width = cg.mineralVec[ind];
			cost -= amt;
			minBars[ind].barO.x = -50 + minBars[ind].bar.width;
			if (cost == 0)
				minBars[ind].barO.width = 0;
			else if (cost < 0)
			{
				minBars[ind].barO.transform.colorTransform = costNeeded;
				minBars[ind].barO.width = cost * -1;
			}
			else if (cost > 0)
			{
				minBars[ind].barO.transform.colorTransform = costExtra;
				if (minBars[ind].bar.width + cost > 100)
					minBars[ind].barO.width = 100 - minBars[ind].bar.width;
				else
					minBars[ind].barO.width = cost;
			}

			minText[ind].text = (amt < 100 ? "0" : "") + (amt < 10 ? "0" : "") + amt.toString();
		}
		
		public function clearBars():void
		{
			for (var i:int = 0; i < 4; i++)
				updateMineral(i, 0);
			tf_name.text = "";
			tf_des.text = "";
		}
		
		// button pressed
		public function onDown(e:MouseEvent):void
		{
			if (!visible) return;
			var tgt:String = e.target.parent.name;
			//trace("onDown: " + getQualifiedClassName(tgt));
			if (tgt.substring(0, 5) != "build")
				return;
			if (motherBought.visible && tgt == "build_mother")
				return;
			if (minWarn.visible)
			{
				minWarnB.play();
				return;
			}
			if (tgt == "build_mother")
			{
				spawnStructure();
				for (var i:int = 0; i < 4; i++)			// clear objective bars once bought
					cg.guiMan.updateMineralO(i, 0);
				return;
			}
			cg.isDeploying = cg.guiMan.guiDeploy.visible = true;
			onBase(e);
		}
		
		public function spawnStructure():void
		{
			if (!ui)
				return;
			
			for (var i:int = 0; i < 4; i++)
				cg.changeMineral(i, -ui.costs[i]);

			//trace("Pressed " + tgt);
			var str:String;
			switch (ui.name)
			{
				case "build_mother":
					if (motherBought.visible)
						return;
					if (!cg.isFinalStage)
						cg.guiMan.guiMid.btn_next.visible = true;
					motherBought.visible = true;
					bTime.visible = false;
					tf_time.visible = false;
					for (i = 0; i < 4; i++)		// clear build requirements
						cg.guiMan.updateMineralO(i, 0);
				return;
				case "build_fdt": str = "fdt"; break;
				case "build_mt": str = "mt"; break;
				case "build_int": str = "int"; break;
				case "build_pdl": str = "pdl"; break;
				case "build_ebt": str = "ebt"; break;
				case "build_amc": str = "amc"; break;
				case "build_rail": str = "rail"; break;
			}

			onBase(null);		// hide build menu
			cg.spawnStructure(str);
		}
		
		// area behind build menu pressed (hide the build screen)
		public function onBase(e:MouseEvent):void
		{
			if (!visible) return;
			cg.guiMan.onBuild(e);
		}
		
		public function onOvr(e:MouseEvent):void
		{
			if (!visible) return;
			var tgt:String = e.target.name;
			trace("onOvr: " + tgt);
			if (tgt.substring(0, 5) != "build")
				return;
			ui = e.target as UIIcon;
			tf_name.text = ui.iconName;
			tf_des.text = ui.iconDes;
			preview.gotoAndStop(ui.currentFrame);
			for (var i:int = 0; i < 4; i++)
				updateMineral(i, ui.costs[i]);
			minWarn.visible = (ui.costs[0] > cg.mineralVec[0] || 
							   ui.costs[1] > cg.mineralVec[1] || 
							   ui.costs[2] > cg.mineralVec[2] || 
							   ui.costs[3] > cg.mineralVec[3]);
		}
		
		public function onOut(e:MouseEvent):void
		{
			if (!visible) return;
			clearBars();
			preview.gotoAndStop(1);
			minWarn.visible = false;
			tf_name.text = "Deploy structures";
			tf_des.text = "Mouseover an icon to see stats; click an icon to deploy";
		}
		
		private function defineStructure(ui:MovieClip, _lab:String, _title:String, _desc:String,
										 _rC:Number, _yC:Number, _gC:Number, _cC:Number):void
		{
			if (_rC > 100) _rC = 100;
			if (_yC > 100) _yC = 100;
			if (_gC > 100) _gC = 100;
			if (_cC > 100) _cC = 100;
			ui.gotoAndStop(_lab);
			ui.setInfo(_title, _desc);
			ui.setCosts(int(_rC), int(_yC), int(_gC), int(_cC));
		}
		
		public function destroy():void
		{
			stage.removeEventListener(MouseEvent.MOUSE_UP, onDown);
			base.removeEventListener(MouseEvent.MOUSE_DOWN, onBase);
			btn_return.removeEventListener(MouseEvent.MOUSE_DOWN, onBase);
			for (i = 0; i < j; i++)
			{
				obj = getChildAt(i);
				var i:int, j:int = numChildren;
				var obj:DisplayObject;
				if (obj is UIIcon)
				{
					obj.removeEventListener(MouseEvent.ROLL_OVER, onOvr);
					obj.removeEventListener(MouseEvent.ROLL_OUT, onOut);
				}
			}
		}
		
		private function getRand(min:int, max:int):int   
		{
			return (int(Math.random() * (max - min + 1)) + min);
		} 
	}
}