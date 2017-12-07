package managers
{
	import flash.display.MovieClip;
	import flash.text.TextField;
	import flash.geom.ColorTransform;
	import flash.events.MouseEvent;
	import scenarios.Scenario;
	import props.Node;

	public class GameManager extends Manager
	{
		private var guiGFX:MovieClip;
		private var guiTop:MovieClip, guiBot:MovieClip;
		
		public var tbReserves:int;		// starting Terrorbytes
		public var started:Boolean;
		
		public var nodeOrigin:MovieClip;
		public var nodeTarget:MovieClip;
		public var nodeDest:MovieClip;
		private var isHoveringOverNode:Boolean;
		private var nodeReachable:Boolean;
		
		public var bonusDetected:Boolean = true;		// false when turret target acquired
		public var bonusLaunched:Boolean = true;		// false when projectile fired
		public var bonusTime:Number = 9000000;			// false if time is above this
		
		public var cursorStatus:String = "IDLE";
		
		private var SNAP_DIST:int = 75;  //35

		public function GameManager(_cg:MovieClip, _guiGFX:MovieClip, u:MovieClip, d:MovieClip)
		{
			super(_cg);
			guiGFX = _guiGFX;
			guiTop = u;
			guiBot = d;	
			
			guiBot.btn_start.addEventListener(MouseEvent.CLICK, onStart);
			guiBot.btn_reset.addEventListener(MouseEvent.CLICK, onReset);
			guiBot.btn_exit.addEventListener(MouseEvent.CLICK, onExit);
			
			guiBot.btn_reset.visible = false;
			
			guiGFX.mouseEnabled = false;
			guiGFX.buttonMode = false;
		}
		
		private function onStart(e:MouseEvent):void
		{
			cg.startScenario();
		}
		
		private function onReset(e:MouseEvent):void
		{
			cg.eng.returnState = 1;
			onExit(e);
		}
		
		private function onExit(e:MouseEvent):void
		{
			guiBot.btn_reset.removeEventListener(MouseEvent.CLICK, onStart);
			guiBot.btn_reset.removeEventListener(MouseEvent.CLICK, onReset);
			guiBot.btn_exit.removeEventListener(MouseEvent.CLICK, onExit);
			cg.destroy();
		}
		
		public function updateTime(s:String):void
		{
			guiTop.tf_time.text = s;
		}
		
		public function setupGame(scen:Scenario):void
		{
			//cg.levelIntro.base.tf_title.text = cg.eng.chosenScenario;
		}
		
		public function setPrompt(s:String):void
		{
			guiBot.textPrompt.base.tf_prompt.text = s;
		}
		
		public function updateGFX():void
		{
			if (nodeOrigin)
			{
				guiGFX.graphics.clear();
				switch (cursorStatus)
				{
					case "SEEK":		guiGFX.graphics.lineStyle(2, 0x449944, .7);	break;
					case "TARGETED":	guiGFX.graphics.lineStyle(2, 0x99FF99, 1);	break;
					case "UNREACHABLE":	guiGFX.graphics.lineStyle(2, 0xFF9999, 1);	break;
					case "IDLE":
				}
				guiGFX.graphics.moveTo(nodeOrigin.x, nodeOrigin.y);
				if (nodeTarget)
					guiGFX.graphics.lineTo(nodeTarget.x, nodeTarget.y);
				else
					guiGFX.graphics.lineTo(cg.mouseX, cg.mouseY);
			}
		}
		
		public function onMouse():void
		{
			//trace("GM: on mouse\t\t" + isHoveringOverNode + " " + nodeOrigin + " " + !nodeTarget);
			return;
			if (!isHoveringOverNode && nodeOrigin && !nodeTarget)
			{
				nodeOrigin = nodeTarget = null;
				cursorStatus = "IDLE";
				guiGFX.graphics.clear();
				cg.cursor.visible = false;
			}
		}
		
		public function onRightMouse():void
		{
			nodeOrigin = nodeTarget = null;
			cursorStatus = "IDLE";
			guiGFX.graphics.clear();
			cg.cursor.visible = false;
		}
		
		// fired from TurretManager overNode
		public function nodeOver(n:MovieClip):void
		{
			//trace("Hovering over " + n.id);
			if (!(n is Node)) return;
			cg.eng.soundMan.playSound("SFX_beepSingle");
			isHoveringOverNode = true;
			if (nodeOrigin && nodeOrigin != n && 
				!nodeTarget && Math.sqrt((n.x - cg.mouseX) * (n.x - cg.mouseX) + (n.y - cg.mouseY) * (n.y - cg.mouseY)) < SNAP_DIST)
			{
				//trace("Targeting " + n.id);
				
				nodeReachable = false;
				for (var j:uint = 0; j < n.edgeLen; j++)
					if (n.edgeArr[j].connections > 0 && n.isAdjacent(nodeOrigin))
					{
						nodeReachable = true;
						break;
					}
				cursorStatus = nodeReachable ? "TARGETED" : "UNREACHABLE";
				if (nodeReachable)
					nodeTarget = n;
			}
		}
		
		// fired from TurretManager outNode
		public function nodeOut(n:MovieClip):void
		{
			//trace("Leaving " + (nodeTarget ? nodeTarget.id : "nothing"));
			isHoveringOverNode = false;
			if (nodeOrigin)
				cursorStatus = "SEEK";
			else
				cursorStatus = "IDLE";
			nodeTarget = null;
			guiGFX.graphics.clear();
		}
		
		// fired from TurretManager onNode (CLICK)
		public function nodeClicked(n:MovieClip):void
		{
			//trace("Node " + n.id + " clicked!");
			if (!(n is Node)) return;
			
			// -- nothing -> selected
			if (!nodeOrigin && (n.nodeState == "controlled" || n.nodeState == "offline"))
			//if (!nodeOrigin && n.tb > 0 && (n.nodeState == "controlled" || n.nodeState == "offline"))
			{
				if (n.tb == 0)
				{
					cg.eng.soundMan.playSound("SFX_beepLow2");
					n.outbox.gotoAndPlay(2);
					return;
				}
				
				cg.eng.soundMan.playSound("SFX_nodeClicked");
				nodeOrigin = n as Node;
				cursorStatus = "SEEK";
				cg.cursor.visible = true;
				cg.cursor.tf_tb.text = n.tb;
				return;
			}
			
			if (!nodeOrigin) return;
			
			// -- selected -> same
			/*if (nodeOrigin == n)
			{
				cursorStatus = "IDLE";
				deselectNode();// trace("Deselect 2");
				return;
			}*/

			// -- selected -> not connected
			if (!nodeReachable)
			{
				//trace("Node not reachable!");
				cg.eng.soundMan.playSound("SFX_beepLow2");
				return;
			}

			// -- selected -> ran out
			if (nodeOrigin.tb == 0)
			{
				//trace("--GM: not enough Tb on " + n.id + " (x" + n.tb + ")");
				return;
			}

			if (!started)
			{
				started = true;
				cg.startScenario();
			}
			
			// -- hack
			if (n.nodeState == "normal")
			{
				nodeOrigin.changeTb(-1);
				nodeTarget.changeResist(-1);
				if (nodeTarget.resist == 0)
				{
					n.updateState((n.timeHackBase + (n.tier-1) * n.timeHackTier) * 1000, true, "hackStart");
					cg.eng.soundMan.playSound("SFX_nodeHacked");
					//trace("--- INITIATING HACK: " + nodeOrigin.id + "->" + n.id);
				}
				else
					cg.eng.soundMan.playSound("SFX_nodeSent");
				cg.addReduce(n);
				
				if (nodeOrigin.tb > 0)
				{
					cg.cursor.tf_tb.text = nodeOrigin.tb;
					return;
				}
			}
			// -- transfer
			else if (n.nodeState == "controlled" || n.nodeState == "offline")
			{
				nodeOrigin.changeTb(-1);
				n.changeTb(1);
				n.outbox.visible = false;
				cg.addTransferred(nodeOrigin);
				cg.addTransferred(n);
				cg.eng.soundMan.playSound("SFX_nodeTransferred");
				if (nodeOrigin.tb > 0)
				{
					cg.cursor.tf_tb.text = nodeOrigin.tb;
					return;
				}
			}
				
			//trace("Deselect 1");
			deselectNode();
		}
		
		public function deselectNode():void
		{
			//trace("Deselect A");
			nodeOrigin = null;
			cg.cursor.visible = false;
			guiGFX.graphics.clear();
		}
	}
}