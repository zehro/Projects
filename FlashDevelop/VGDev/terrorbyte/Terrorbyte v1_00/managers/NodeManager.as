package managers
{
	import props.Edge;
	import props.Node;
	import flash.display.MovieClip;
	import flash.geom.Point;

	public class NodeManager extends Manager
	{
		public var nodeBook:Object = new Object();
		public var nodeArr:Array = [];
		public var edgeArr:Array = [];
		private var numNodes:int;
		private var numEdges:int;
		
		private var origin:Node;
		private var gfx:MovieClip;
		
		private var i:int, j:int;
		private var edge:Edge;
		private var node:Node;
		private var tgt:String;
		private var targets:Array = [];
		private var nodeVisits:Object = new Object();
		//private var numNodes:int;
		
		public var isConnected:Boolean;
		
		public function NodeManager(_cg:MovieClip)
		{
			super(_cg);
			gfx = cg.nodeGFX;
			gfx.buttonMode = false;
			gfx.mouseEnabled = false;
		}
		
		public function includeNode(node:Node):Node
		{
			//trace("Adding " + node.id + " to the nodebook.");
			numNodes++;
			node.cacheAsBitmap = true;
			nodeBook[node.id] = node;
			nodeArr.push(node);
			updateGFX();
			return node;
		}
		
		public function setOrigin(n:String):void
		{
			origin = nodeBook[n];
			nodeBook[n].setAsHome();
			updateGFX();
		}
		
		public function setUploadable(n:String, b:Boolean):void
		{
			nodeBook[n].setUploadable(b);
		}
		
		public function addEdge(n1:String, n2:String):void
		{
			var connFX:MovieClip = new UIConnecting();
			
			connFX.x = (nodeBook[n1].x + nodeBook[n2].x) * .5;
			connFX.y = (nodeBook[n1].y + nodeBook[n2].y) * .5;
			cg.addChild(connFX);
			
			edge = new Edge(nodeBook[n1], nodeBook[n2], connFX);
			nodeBook[n1].addEdge(edge);
			nodeBook[n2].addEdge(edge);
			edgeArr.push(edge);
			numEdges++;
		}
				
		private function getConnection():Array
		{
			var connArr:Array = [];
			
			for (i = 0; i < numNodes; i++)
			{
				tgt = targets[i];
				for (j = 0; j < numNodes; j++)
				{
					//trace(j + " " + nodeArr[j]);
					nodeVisits[nodeArr[j].id] = false;
				}
				isConnected = connHelper(origin)
				connArr.push(isConnected);
			}
			return connArr;
		}
		
		private function connHelper(n:Node):Boolean
		{
			if (n.id == tgt)
				return true;
			if (nodeVisits[n.id])
				return false;
			nodeVisits[n.id] = true;
			for (var k:uint = 0; k < n.edgeLen; k++)
			{
				if (n.edgeArr[k].connections == 0) continue;
				if (connHelper(n.edgeArr[k].getNode1().id == n.id ? n.edgeArr[k].getNode2() : n.edgeArr[k].getNode1()))
					return true;
			}
			return false;
		}
		
		public function updateTime(tick:int):void
		{
			//trace("NodeManager tick.");
			var i:int;
			for (i = nodeArr.length - 1; i >= 0; i--)
				nodeArr[i].updateTime(tick);
			for (i = edgeArr.length - 1; i >= 0; i--)
				edgeArr[i].updateTime(tick);
		}
		
		public function reset():void
		{
			for (i = 0; i < numEdges; i++)
				edgeArr[i].connections = 0;
			for (i = 0; i < numNodes; i++)
			{
				nodeArr[i].kill();
				nodeArr[i].toggleState(false);
			}
			//updateGFX();
			isConnected = false;
		}

		public function updateGFX():Array
		{			
			if (numEdges == 0 || !origin) return [];
			gfx.graphics.clear();
			
			//trace("\n\nUpdating graph.");

			var connArr:Array = getConnection();
			
			for (i = 0; i < numEdges; i++)
			{
				edge = edgeArr[i];
				//trace("Edge between " + edge.n1() + " and " + edge.n2() + " has " + edge.connections + " connections.");
				if (edge.connections == 0)
					gfx.graphics.lineStyle(2, 0x888888, .3);
				else if (edge.connections == 1)
					gfx.graphics.lineStyle(2, 0xFFFF00, .5);
				else
					gfx.graphics.lineStyle(2, 0xFFFFFF, .5);
				gfx.graphics.moveTo(edge.n1x(), edge.n1y());
				gfx.graphics.lineTo(edge.n2x(), edge.n2y());
			}
			
			return connArr;
		}
		
		public function getNodes():void
		{
			var out:String = "Nodes in NodeManager: [ ";
			for each (var n:Node in nodeBook)
				out += n.id + " ";
			trace(out + "]");
		}
		
		/*private function getDistance(d:Point, e:Edge):Number
		{
			var projPt:Point = new Point(d.x - e.getNode1().x, d.y - e.getNode1().y);
			var dotProd:Number = projPt.x * e.normVect.x + projPt.y * e.normVect.y;
			
			projPt.x = e.normVect.x * dotProd + e.getNode1().x;
			projPt.y = e.normVect.y * dotProd + e.getNode1().y;
			
			return Math.sqrt(Math.pow(d.x - projPt.x,2) + Math.pow(d.y - projPt.y,2));
		}*/
		
		override public function destroy():void
		{
			for each (var n:Node in nodeBook)
			{
				
			}
		}
	}
}
