package managers
{
	import flash.display.MovieClip;
	import flash.text.TextField;
	import flash.geom.ColorTransform;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	import utils.UIArrow;
	import props.Floater;

	public class GUIManager extends Manager
	{
		public var guiLeft:MovieClip, guiMid:MovieClip, guiRight:MovieClip, guiWarning:MovieClip, guiEnemy:MovieClip, guiDeploy:MovieClip;
		private var minBars:Vector.<MovieClip> = new Vector.<MovieClip>(4, true);
		private var minText:Vector.<TextField> = new Vector.<TextField>(4, true);
		private var arrVec:Vector.<MovieClip> = new Vector.<MovieClip>();

		public function GUIManager(_par:ContainerGame, l:MovieClip, m:MovieClip, r:MovieClip, w:MovieClip, e:MovieClip, d:MovieClip)
		{
			super(_par);
			guiLeft = l;
			guiMid = m;
			guiRight = r;
			guiWarning = w;
			guiEnemy = e;
			guiDeploy = d;
			
			// -- add mineral bars and text fields to vectors
			minBars[0] = (guiRight.bar_r);	minText[0] = (guiRight.tf_r);
			minBars[1] = (guiRight.bar_y);	minText[1] = (guiRight.tf_y);
			minBars[2] = (guiRight.bar_g);	minText[2] = (guiRight.tf_g);
			minBars[3] = (guiRight.bar_b);	minText[3] = (guiRight.tf_b);
			
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
			
			// -- initialize bars
			for (var i:int = 0; i < 4; i++)
			{
				updateMineral(i, 0);
				updateMineralO(i, 0);
			}
				
			// -- alpha if mouse over GUI
			guiLeft.mouseChildren = false;
			guiRight.mouseChildren = false;
			guiLeft.addEventListener(MouseEvent.ROLL_OVER, overLeft);
			guiLeft.addEventListener(MouseEvent.ROLL_OUT, outLeft);
			guiRight.addEventListener(MouseEvent.ROLL_OVER, overRight);
			guiRight.addEventListener(MouseEvent.ROLL_OUT, outRight);
			
			guiMid.btn_tech.addEventListener(MouseEvent.CLICK, onTech);
			guiMid.btn_next.addEventListener(MouseEvent.CLICK, onNext);
			guiMid.btn_build.addEventListener(MouseEvent.CLICK, onBuild);		
			
			guiMid.btn_next.visible = false;
			
			guiDeploy.visible = false;
			
			guiLeft.minimap.graphics.lineStyle(1, 0xFFFFFF, 1);
			guiLeft.minimap.scaleX = guiLeft.minimap.scaleY = .08;
		}
		
		// "TECH" button pressed
		public function onTech(e:MouseEvent):void
		{
			par.GUI_tech.visible = !par.GUI_tech.visible;
			par.GUI_build.visible = false;
			if (!par.GUI_tech.visible)
				par.togglePause(2);
			else
				par.togglePause(1);
			if (!e)
			{
				par.isDeploying = false;
				par.guiMan.guiDeploy.visible = false;
			}
		}

		// "NEXT" button pressed
		public function onNext(e:MouseEvent):void
		{
			trace("onNext!");
			par.GUI_tech.visible = false;
			par.endStage();
			par.isDeploying = false;
			par.guiMan.guiDeploy.visible = false;
			//par.togglePause(1);
		}
		
		// "BUILD" button pressed
		public function onBuild(e:MouseEvent):void
		{
			par.GUI_build.visible = !par.GUI_build.visible;
			if (!par.GUI_build.visible)
				par.togglePause(2);
			else
			{
				par.togglePause(1);
				par.GUI_build.tf_time.text = par.getTime();
			}
			par.GUI_tech.visible = false;
			if (!e)
			{
				par.isDeploying = false;
				par.guiMan.guiDeploy.visible = false;
			}
			//par.togglePause(1);
		}
		
		// make a GUI arrow that follows a target "t"
		public function addArrow(t:Floater, col:uint):void
		{
			var a:UIArrow = new UIArrow(this, par.game.gui, par.game, t, col);
			arrVec.push(a);
			par.game.gui.addChild(a);
		}
		
		override public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			var i:int;
			for (i = arrVec.length - 1; i >= 0; i--)
				if (arrVec[i].step())
					arrVec.splice(i, 1);
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return arrVec;
		}
		
		public function updateScore(s:int):void
		{
			var str:String = "";
			for (var i:int = 5; i >= 0; i--)
				if (s < Math.pow(10, i))
					str += "0";
			str += s.toString();
			guiLeft.tf_score.text = str;
		}
		
		//	updates a mineral's bar and textfield
		//	ind: which mineral		amt: 0-100
		public function updateMineral(ind:int, amt:int):void
		{
			minBars[ind].bar.width = amt;
			minText[ind].text = (amt < 100 ? "0" : "") + (amt < 10 ? "0" : "") + amt.toString();
		}
		
		//	updates a mineral's objective bar
		//	ind: which mineral		amt: 0-100
		public function updateMineralO(ind:int, amt:int):void
		{
			minBars[ind].barO.width = amt;
		}
		
		public function updateHealth(hp:Number):void
		{
			guiLeft.health.gotoAndStop(Math.ceil(-.3 * hp + 31));
		}
		
		public function updateLaser(n:Number, col:ColorTransform):void
		{
			guiLeft.laser.rotation = -1.46 * n + 146;
			guiLeft.laser.transform.colorTransform = col;
		}
		
		public function updateTime(s:String):void
		{
			guiRight.tf_time.text = s;
		}
		
		// display incoming enemy message
		public function enemyWarn(s:String):void
		{
			guiEnemy.base.tf_text.text = s;
			guiEnemy.gotoAndPlay(2);		// -- TODO handle if current frame isn't 1
		}
		
		// display warning message message
		// interrupt TRUE to override existing message
		public function warningWarn(s:String, col:uint = 0x000000, interrupt:Boolean = false):void
		{
			if (!interrupt && guiWarning.currentFrame != 1)
				return;
			if (guiWarning.base.tf_text.text == s)
				return;
			guiWarning.base.tf_text.text = s;
			guiWarning.base.tf_text.textColor = col;
			guiWarning.gotoAndPlay(2);		// -- TODO handle if current frame isn't 1
		}
		
		// make UI transparent if mouse goes over it
		private function overLeft(e:MouseEvent):void
		{
			guiLeft.alpha = .5;
		}

		private function outLeft(e:MouseEvent):void
		{
			guiLeft.alpha = 1;
		}

		private function overRight(e:MouseEvent):void
		{
			guiRight.alpha = .5;
		}

		private function outRight(e:MouseEvent):void
		{
			guiRight.alpha = 1;
		}
		
		public function isOverUI(p:Point):Boolean
		{
			return par.GUI_tech.visible ||
				   par.GUI_build.visible ||
				   guiMid.hitTestPoint(p.x, p.y, true);
		}
		
		public function clearArrows():void
		{
			var mc:MovieClip;
			for each (mc in arrVec)
				if (par.game.gui.contains(mc))
					par.game.gui.removeChild(mc);
			arrVec = new Vector.<MovieClip>();
		}

		override public function destroy():void
		{
			guiLeft.removeEventListener(MouseEvent.ROLL_OVER, overLeft);
			guiLeft.removeEventListener(MouseEvent.ROLL_OUT, outLeft);
			guiRight.removeEventListener(MouseEvent.ROLL_OVER, overRight);
			guiRight.removeEventListener(MouseEvent.ROLL_OUT, outRight);
			guiMid.btn_tech.removeEventListener(MouseEvent.CLICK, onTech);
			guiMid.btn_next.removeEventListener(MouseEvent.CLICK, onNext);
			guiMid.btn_build.removeEventListener(MouseEvent.CLICK, onBuild);
			clearArrows();
			guiMid.btn_next.visible = false;
		}
	}
}