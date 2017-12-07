package props
{
	import flash.display.MovieClip
	import managers.NodeManager;
	import managers.Manager;
	import flash.text.TextField;

	public class Node extends Prop
	{
		private var nodeMan:NodeManager;
		
		public var id:String;
		public var tb:int;		// Terrorbytes available/at this node
		public var resist:int;	// Terrorbytes needed to take over this node
		public var tier:int;	// level of this node
		
		public var nodeState:String = "normal";

		public var clockTime:int;		// ms on the clock for the hack clock
		private var MAX_TIME:int = 10 * 1000;
		
		public var timeHackBase:Number = 3;		// time to hack Tier 1
		public var timeHackTier:Number = 1.5;	// time to hack each +1 Tier
		public var timeStunBase:Number = 4;		// time to hack Tier 1
		public var timeStunTier:Number = 1.5;	// time to hack each +1 Tier
		
		public var beingHacked:Boolean;			// for hacking FX
		
		//public var playerControl:Boolean;
		public var conquerable:Boolean;

		public var edgeArr:Array = [];
		public var edgeLen:int;
		private var edge:Edge;
		private var i:int, j:int;

		public var edgeBook:Object = new Object();
		public var active:Boolean;
		
		public var showWhichBox:String = "info";

		// pointers to corresponding parts of this node (needed for "abstraction")
		public var timeBox:MovieClip;
		public var infoBox:MovieClip;
		public var genericBox:MovieClip;
		public var tbQueue:MovieClip;
		
		public var tfTb:TextField;
		public var tfResist:TextField;
		public var tfTier:TextField;

		public function Node(man:Manager, _nodeMan:NodeManager, xP:int, yP:int, _id:String, _tb:int, _resist:int, _tier:int)
		{
			super(man, xP, yP);
			nodeMan = _nodeMan;
			
			tb = _tb;
			resist = _resist;
			tier = _tier;
			
			id = _id;
		}
		
		protected function drawRange():void
		{
			// -- override this function
		}
		
		protected function setup():void
		{
			//trace("\t\tSetup " + id + " with " + tb + " " + resist + " " + tier);
			filters = [glowR];
			
			tfTb.text = tb.toString();
			tfResist.text = resist.toString();
			tfTier.text = tier.toString();
			
			//trace("\t\tSetup " + id + " with Tb x " + tfTb.text + " : " + tb);
			
			timeBox.visible = false;
			tbQueue.visible = false;
			
			if (genericBox)
				genericBox.visible = false;
			
			infoBox.tf_hack.text = (timeHackBase + (tier-1) * timeHackTier).toFixed(1) + " sec";
			infoBox.tf_stun.text = (timeStunBase + (tier-1) * timeStunTier).toFixed(1) + " sec";
			infoBox.visible = false;
			
			// show only HACK, not HACK/STUN
			if (conquerable)
			{
				infoBox.gotoAndStop("stun");
				infoBox.tf_hack.text = (timeHackBase + (tier-1) * timeHackTier).toFixed(1) + " sec";
			}
		}

		public function addEdge(e:Edge):void
		{
			edgeArr.push(e);
			if (e.n1() == id)
				edgeBook[e.n2()] = e;
			else
				edgeBook[e.n1()] = e;
			edgeLen++;
		}
		
		public function isValidConnection(str:String):Boolean
		{
			return edgeBook[str] != null;
		}
		
		public function isAdjacent(n:Node):Boolean
		{
			for (var i:int = 0; i < edgeLen; i++)
				if (edgeArr[i].n1() == n.id || edgeArr[i].n2() == n.id)
					return true;
			return false;
		}
		
		public function isEdgeActive(str:String):Boolean
		{
			return edgeBook[str].active;
		}
		
		public function getEdge(str:String):Edge
		{
			return edgeBook[str];
		}
		
		public function changeEdge(str:String, b:Boolean):void
		{
			/*edge = edgeBook[str];
			edge.active = b;
			edge.getNode1().setOffline(true);
			edge.getNode2().setOffline(true);*/
		}
		
		public function setOffline(isOffline:Boolean, perma:Boolean = false):void
		{
			if (isOffline)
			{
				filters = [];
				for (j = 0; j < edgeLen; j++)
					edgeArr[j].connections += perma ? 2 : 1;						
			}
			else
			{
				filters = [glowR];
				for (j = 0; j < edgeLen; j++)
					edgeArr[j].connections--;			
			}
			nodeMan.updateGFX();
		}
		
		public function setClock(time:int = 0):void
		{
			if (time != 0)
				clockTime = time;
			timeBox.clock.ring.rotation = 180 * ((clockTime > MAX_TIME ? MAX_TIME : clockTime) / MAX_TIME);
		}
				
		public function updateTime(tick:int):void
		{
			//trace(this + " updating time: " + clockTime);
			clockTime -= tick;
			if (clockTime < 1)
			{
				clockTime = 0;
				if (beingHacked)
					nodeMan.cg.eng.soundMan.playSound("SFX_nodeOffline");
				updateState();
			}
				
			if (timeBox.visible)
			{
				timeBox.clock.needle.rotation = 180 * ((clockTime > MAX_TIME ? MAX_TIME : clockTime) / MAX_TIME);
				timeBox.tf_time.text = getTime();
			}
			
			if (beingHacked)
			{
				manager.cg.addHacking(this);
				//manager.cg.eng.soundMan.playSound("SFX_hacking");
			}
		}
		
		public function updateState(newTime:int = 0, addOrSet:Boolean = false, newState:String = ""):void
		{
			if (newTime != 0)
			{
				if (addOrSet)
					clockTime = newTime;
				else
					clockTime += newTime;
			}
			if (newState != "")
				nodeState = newState;
			switch (nodeState)
			{				
				case "hackStart":
					nodeState = "hacking";
					setClock();
					showWhichBox = "none";
					beingHacked = true;
					infoBox.visible = false;
					timeBox.visible = true;			
					timeBox.tf_operation.text = "HACKING";
				break;
				case "hacking":
					nodeState = "offline";
					beingHacked = false;
					manager.cg.addHacked(this);
					if (!conquerable)
					{
						setOffline(true);
						setClock((timeStunBase + (tier-1) * timeStunTier) * 1000);
						timeBox.tf_operation.text = "OFFLINE";
					}
					else
					{
						setOffline(true, true);
						nodeState = "controlled";
						genericBox.tf_generic.text = "Controlled";
						glowB.quality = 2;
						glowB.alpha = .5;
						filters = [glowB];
						this.transform.colorTransform = colorW;
						timeBox.visible = false;
						showWhichBox = "generic";
					}
					drawRange();
				break;
				case "offline":
					nodeState = "normal";
					nodeMan.cg.eng.soundMan.playSound("SFX_nodeOnline");
					changeResist(1);
					setOffline(false);
					timeBox.visible = false;
					showWhichBox = "info";
					if (manager.cg.gameMan.nodeOrigin == this)
						manager.cg.gameMan.deselectNode();
					drawRange();
				break;
			}
		}
		
		protected function getTime():String
		{
			var timeSec:int = int(clockTime * .001);
			var timeMSec:int = int((clockTime - (timeSec * 1000)) * .01);
			return timeSec + "." + timeMSec;
		}
		
		public function changeTb(amt:int):void
		{
			tb += amt;
			tfTb.text = tb.toString();
		}
		
		public function changeResist(amt:int):void
		{
			resist += amt;
			tfResist.text = resist.toString();
		}
		
		override public function step():Boolean
		{
			if (active) return false;
			return false;
		}
		
		public function kill():void
		{
			active = false;
		}
		
		public function destNode():void
		{
			kill();
			for (i = 0; i < edgeLen; i++)
			{
				edge = edgeArr[i];
				edge.connections = 0;
				edge.getNode1().setOffline(false);
				edge.getNode2().setOffline(false);
			}
		}
	}
}
