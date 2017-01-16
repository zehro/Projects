package managers
{
	import flash.display.MovieClip;
	import flash.text.TextField;
	import flash.geom.ColorTransform;
	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.display.DisplayObject;
	import props.UIIcon;

	public class UIUpgrade extends MovieClip
	{
		private var cg:MovieClip;
		private var gui:GUIManager;
		
		public var ui:UIIcon;				// currently selected icon
		public var techCost:Vector.<int>;	// mineral cost for part (array length 4)
		
		private var minBars:Vector.<MovieClip> = new Vector.<MovieClip>(4, true);
		private var minCosts:Vector.<MovieClip> = new Vector.<MovieClip>(4, true);
		private var minText:Vector.<TextField> = new Vector.<TextField>(4, true);
	
		private var costNeeded:ColorTransform;
		private var costExtra:ColorTransform;
		
		// upgrade lists
		private var laserDamage:Vector.<Boolean> = new Vector.<Boolean>(3, true);
		private var laserCharge:Vector.<Boolean> = new Vector.<Boolean>(2, true);
		private var tractCharge:Vector.<Boolean> = new Vector.<Boolean>(2, true);
		private var tractDist:Vector.<Boolean> = new Vector.<Boolean>(2, true);
		private var motherMove:Vector.<Boolean> = new Vector.<Boolean>(2, true);
		private var motherRadar:Vector.<Boolean> = new Vector.<Boolean>(2, true);
		private var motherSynth:Boolean;
		
		public function UIUpgrade()
		{					
			defineTech(tech_dmg1, "lasDmg1",
							"Focusing Lens",
							"Increases laser damage.",
							40, 15, 20, 0, false);
							
			defineTech(tech_dmg2, "lasDmg2",
							"Energy Amplifier",
							"Greatly increases laser damage.",
							65, 35, 30, 10);
							
			defineTech(tech_lChrg1, "lasChrg1",
							"Mark I Charger",
							"Increases laser recharge rate.",
							20, 20, 5, 0, false);
							
			defineTech(tech_tChrg1, "traChg1",
							"Tractor Energizer",
							"Increases tractor recharge rate.",
							0, 5, 15, 20, false);
							
			defineTech(tech_tractD1, "traDist1",
							"Tractor Stabilizer",
							"Improves responsiveness of objects being towed by the tractor beam.",
							0, 10, 5, 15, false);
							
			defineTech(tech_tractS, "traStab",
							"Gamma Centrifuge",
							"Slows and stops the rotation of tractored asteroids.\nUses more tractor energy.",
							0, 10, 15, 30);
							
			defineTech(tech_move1, "move1",
							"Viewport Servos",
							"Increases movement speed.",
							0, 15, 25, 0, false);
							
			defineTech(tech_radar1, "mm1",
							"Minimap EXT-B",
							"Increases range of minimap.",
							10, 10, 10, 10, false);
							
			defineTech(tech_synth, "synth",
							"Mineral Synthesizer",
							"Periodically generates the mineral you have the least of.\n(Takes about 2:10 to pay for itself.)",
							50, 50, 50, 50, false);

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
			tf_name.text = "Buy upgrades";
			tf_des.text = "Mouseover an icon to see stats; click an icon to purchase";
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
			if (tgt.substring(0, 4) != "tech")
				return;
			if (minWarn.visible)
			{
				minWarnB.play();
				return;
			}
			if (!ui)
				return;
			if (ui.bought.visible || ui.isLocked.visible)
				return;

			for (var i:int = 0; i < 4; i++)
				cg.changeMineral(i, -ui.costs[i]);

			ui.bought.visible = true;
			onOut(e);		// reset bars

			//trace("Pressed " + ui.name);
			var str:String;
			switch (ui.name)
			{
				case "tech_dmg1":
					laserDamage[0] = true;
					tech_dmg2.isLocked.visible = false;
					cg.laserDmg = 4;
				break;
				case "tech_dmg2":
					laserDamage[1] = true;
					cg.laserDmg = 6;
				break;
				case "tech_lChrg1":
					laserCharge[0] = true;
					cg.laserRecharge = 1.2;
					cg.laserDepleteCharge = 1.7;
				break;
				case "tech_tChrg1":
					tractCharge[0] = true;
					cg.tractorRecharge = 1.2;
					cg.tractorDepleteCharge = 1.7;
				break;
				case "tech_tractD1":
					tractDist[0] = true;
					cg.tractDist = 250;
					tech_tractS.isLocked.visible = false;
				break;
				case "tech_tractS":
					tractCharge[1] = true;
					cg.tractorRotRed = .95;
				break;
				case "tech_radar1":
					motherRadar[0] = true;
					cg.minimap.scaleX = cg.minimap.scaleY = .06;
					cg.guiMan.guiLeft.rangefinder.scaleX = cg.guiMan.guiLeft.rangefinder.scaleY = .75;					
				case "tech_move1":
					motherMove[0] = true;
					cg.moveSpd = 1.25;
					cg.maxSpd = 3.5;
					cg.friction = .97;
				break;
				case "tech_synth":
					motherSynth = true;
					cg.synth = true;
				break;
			}
		}
		// area behind build menu pressed (hide the build screen)
		public function onBase(e:MouseEvent):void
		{
			if (!visible) return;
			cg.guiMan.onTech(null);
		}
		
		public function onOvr(e:MouseEvent):void
		{
			if (!visible) return;
			var tgt:String = e.target.name;
			if (tgt.substring(0, 4) != "tech")
				return;
			ui = e.target as UIIcon;
			tf_name.text = ui.iconName;
			tf_des.text = ui.iconDes;
			preview.gotoAndStop(ui.currentFrame);
			if (ui.bought.visible)
				return;
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
			tf_name.text = "Buy upgrades";
			tf_des.text = "Mouseover an icon to see stats; click an icon to purchase";
		}
		
		private function defineTech(ui:MovieClip, _lab:String, _title:String, _desc:String,
										 _rC:Number, _yC:Number, _gC:Number, _cC:Number,
										 _isLocked:Boolean = true):void
		{
			if (_rC > 100) _rC = 100;
			if (_yC > 100) _yC = 100;
			if (_gC > 100) _gC = 100;
			if (_cC > 100) _cC = 100;
			ui.gotoAndStop(_lab);
			ui.setInfo(_title, _desc);
			ui.setCosts(int(_rC), int(_yC), int(_gC), int(_cC));
			ui.isLocked.visible = _isLocked;
			trace("UI: " + ui + " " + ui.currentFrame + "/" + ui.currentLabel);
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
	}
}