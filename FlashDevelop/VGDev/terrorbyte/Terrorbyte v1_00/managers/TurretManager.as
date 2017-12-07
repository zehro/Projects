package managers
{
	import flash.display.MovieClip;
	import managers.*;
	import props.*;
	import flash.geom.Point;
	import flash.events.MouseEvent;

	public class TurretManager extends Manager
	{
		public var turrVec:Vector.<MovieClip> = new Vector.<MovieClip>();
		
		public var gameMan:GameManager;
		public var nodeMan:NodeManager;
		public var planeMan:PlaneManager;
		public var missMan:MissileManager;

		public function TurretManager(_cg:MovieClip, _gameMan:GameManager, _nodeMan:NodeManager, _planeMan:PlaneManager, _missMan:MissileManager)
		{
			super(_cg);
			gameMan = _gameMan;
			nodeMan = _nodeMan;
			planeMan = _planeMan;
			missMan = _missMan;
		}
		
		override public function step(ld:Boolean, rd:Boolean, p:Point):void
		{
			var i:int;
			for (i = turrVec.length - 1; i >= 0; i--)
				if (turrVec[i].step())
					turrVec.splice(i, 1);
				else if (ld)
					turrVec[i].leftMouse();
				else if (rd)
					turrVec[i].rightMouse();
		}
		
		public function setRunning(run:Boolean):void
		{
			for each (var m:MovieClip in turrVec)
			{
				m.isRunning = run;
				if (!run)
					m.restart();
			}
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return turrVec;
		}
		
		public function placeTurret(type:String, locX:int, locY:int, id:String, tb:int, resist:int, tier:int, rot:Number):MovieClip
		{
			var turret:MovieClip;
			if (type == "laser")
				turret = new TurretLaser(this, nodeMan, locX, locY, id, type, tb, resist, tier, rot);
			else
				turret = new Turret(this, nodeMan, locX, locY, id, type, tb, resist, tier, rot);
			addNode(turret);
			return turret;
		}
		
		public function placeServer(type:String, locX:int, locY:int, id:String, tb:int, resist:int, tier:int, perma:Boolean = false):Server
		{
			var server:Server = new Server(this, nodeMan, locX, locY, id, type, tb, resist, tier, perma);
			addNode(server);
			return server;
		}
		
		public function placeSatellite(locX:int, locY:int, id:String, tb:int):void
		{
			var server:Server = new Server(this, nodeMan, locX, locY, id, "", tb, 0, 0, true);
			//trace("Placing home with " + tb + " Terrorbytes.");
			server.setAsHome();
			addNode(server);
		}
		
		private function addNode(n:MovieClip):void
		{
			n.hitbox.addEventListener(MouseEvent.MOUSE_OVER, overNode);
			n.hitbox.addEventListener(MouseEvent.MOUSE_OUT, outNode);
			n.hitbox.addEventListener(MouseEvent.CLICK, onNode);
			nodeMan.includeNode(n as Node);
			cg.cont.addChild(n);
			turrVec.push(n);
		}
		
		private function overNode(e:MouseEvent):void
		{
			var node:MovieClip = e.target.parent;
			//trace("\nRolled over node " + node);
			switch (node.showWhichBox)
			{
				case ("info"):
					node.infobox.visible = true;
				break;
				case ("generic"):
					node.genericbox.visible = true;
				break;
			}
			if (node.tb == 0 && (node.nodeState == "controlled" || node.nodeState == "offline"))
				node.outbox.visible = true;
			gameMan.nodeOver(node);
		}
		
		private function outNode(e:MouseEvent):void
		{
			var node:MovieClip = e.target.parent;
			//trace("Rolled out node " + node);
			node.infobox.visible = false;
			node.outbox.visible = false;
			if (node.genericBox)
				node.genericBox.visible = false;
			gameMan.nodeOut(node);
		}
		
		private function onNode(e:MouseEvent):void
		{
			//trace("\tClicked over node " + e.target.parent.id);
			gameMan.nodeClicked(e.target.parent);
		}
	}
}
