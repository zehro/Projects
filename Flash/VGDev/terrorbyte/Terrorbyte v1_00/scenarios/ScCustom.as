package scenarios
{
	import flash.display.MovieClip;
	import flash.net.FileReference;
	import flash.events.Event;
	import flash.net.FileFilter;
	import flash.events.IOErrorEvent;
	import flash.utils.ByteArray;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import props.Turret;
	import props.Server;

	public class ScCustom extends Scenario			// IDENTIFIER: "Custom"
	{
		private var fileRef:FileReference;

		public function ScCustom()
		{
			/*var hax:URLLoader = new URLLoader();
			hax.addEventListener(Event.COMPLETE, onFileLoaded);
			hax.load(new URLRequest("custom.txt"));*/
			
			fileRef = new FileReference();
			fileRef.addEventListener(Event.SELECT, onFileSelected);
			var fileFilter:FileFilter = new FileFilter("Text Files", "*.txt");
			fileRef.browse([fileFilter]);
		}
		
		private function onFileSelected(event:Event):void
		{
			fileRef.addEventListener(Event.COMPLETE, onFileLoaded);
			fileRef.addEventListener(IOErrorEvent.IO_ERROR, onFileLoadError);
			fileRef.load();
			
			cg.cust.visible = true;
		}
				
		function onFileLoaded(event:Event):void
		{
			fileRef.removeEventListener(Event.COMPLETE, onFileLoaded);
			fileRef.removeEventListener(IOErrorEvent.IO_ERROR, onFileLoadError);
			
			var fileReference:FileReference = event.target as FileReference;
			var textData:ByteArray = fileReference["data"];
			cg.cust.tf_custom.appendText("\n\n\n");
			
			var gfx:MovieClip = new MovieClip();
			cg.terrain.addChild(gfx);
			
			var checkOnePlane:Boolean = false;
			var checkOneHome:Boolean = false;
			
			var code:Array;
			var i:uint;
			var pt:String;
			var turr:MovieClip;
			var serv:Server;
			try
			{
				code = textData.toString().split("\n");
				cg.cust.tf_custom.appendText("\n" + code);
				for each (var str:String in code)
				{
					var cmd:Array = str.split(" ");
					if (cmd[0].length < 2) continue;
					if (cmd[0].substring(0, 2) == "//") continue;
					switch (cmd[0])
					{
						case "SATELLITE":
							// turrMan.placeSatellite(-250, -200, "home", 1);
							// SATELLITE -250 -200 home 1
							cg.turrMan.placeSatellite(cmd[1], cmd[2], cmd[3], cmd[4]);
							checkOneHome = true;
						break;
						case "TURRET":
							// turrMan.placeTurret("turretNorm", -190, 0, "turret", 0, 1, 1, 45);
							// TURRET turretNorm -190 0 turret 0 1 1 45
							// TURRET turretNorm -190 0 modified 0 1 2 45 rof:5
							turr = cg.turrMan.placeTurret(cmd[1], cmd[2], cmd[3], cmd[4], cmd[5], cmd[6], cmd[7], cmd[8]);
							if (cmd.length > 9)
								for (i = 9; i < cmd.length; i++)
								{
									var modifier:Array = cmd[i].split(":");
									trace(modifier);
									turr.setAttribute(modifier[0], modifier[1]);
								}
						break;
						case "SERVER":
							// turrMan.placeServer("serverNorm", 40, 50, "mountains", 3, 1, 1);
							// SERVER serverNorm -190 0 server 0 1 1
							serv = cg.turrMan.placeServer(cmd[1], cmd[2], cmd[3], cmd[4], cmd[5], cmd[6], cmd[7]);
						break;
						case "EDGE":
							// nodeMan.addEdge("home", "turret");
							// EDGE home turret
							cmd[2] = cmd[2].substring(0, cmd[2].length - 1);		// remove trailing \n character
							cg.nodeMan.getNodes();
							cg.nodeMan.addEdge(cmd[1], cmd[2]);
						break;
						case "PLANE":
							// planeMan.addPlane("passenger", -290, 190, -45, makeWaypoints([-200, 100,  -100, 0,  60, -100,  500, -500]));
							// PLANE passenger -290 190 -45 (-200 100) (-100 0) (60 -100) (500 -500)
							var way:Array = [];
							stripParen(cmd, 5, cmd.length - 1);
							for (i = 5; i < cmd.length; i++)
								way.push(Number(cmd[i]));
							cg.planeMan.addPlane(cmd[1], cmd[2], cmd[3], cmd[4], makeWaypoints(way));
							checkOnePlane = true;
						break;
						case "TERRAIN":
							// TERRAIN (40 40) (60 60) ...
							stripParen(cmd, 1, cmd.length - 1);
							gfx.graphics.lineStyle(1, 0xFFFFFF, 1);
							gfx.graphics.moveTo(cmd[1], cmd[2]);
							for (i = 3; i < cmd.length; i += 2)
								gfx.graphics.lineTo(cmd[i], cmd[i+1]);
						break;
						case "TREE1":
							// TREE1 (0 0)
							addMC(cmd, new TerrainTree());
						break;
						case "TREE3":
							// TREE3 (0 0)
							addMC(cmd, new TerrainTree3());
						break;
						case "MOUNTAIN1":
							// MOUNTAIN1 (0 0)
							addMC(cmd, new TerrainMountain());
						break;
						case "MOUNTAIN3":
							// MOUNTAIN3 (0 0)
							addMC(cmd, new TerrainMountain3());
						break;
						case "SAFETITLE":
							addMC(cmd, new SafeZone());
						break;
						case "SAFEZONE":
							// SAFEZONE (0 0) (50 50) (25 25) ...
							stripParen(cmd, 1, cmd.length - 1);
							gfx.graphics.lineStyle(1, 0x00FF00, 1);
							gfx.graphics.beginFill(0x00FF00, .15);
							gfx.graphics.moveTo(cmd[1], cmd[2]);
							for (i = 3; i < cmd.length; i += 2)
							{
								gfx.graphics.lineTo(cmd[i], cmd[i+1]);
							}
							gfx.graphics.lineTo(cmd[1], cmd[2]);
							gfx.graphics.endFill();
						break;
						default:
							cg.cust.tf_custom.appendText("\n>>> Unknown code found. (" + cmd[0] + ")");
							return;
					}
				}
				cg.nodeMan.setOrigin("home");
				cg.cust.visible = false;
			}
			catch (e:Error)
			{
				cg.cust.tf_custom.text = "\>> Error reading file.\n" + e.toString() +
										 "\n\nAt command:\n" + cmd;
				return;
			}
			
			if (!checkOnePlane)
			{
				cg.cust.visible = true;
				cg.cust.tf_custom.text = "\n\n\n\n\t\tERROR!\n\t\tNeed at least one plane!";
			}
			if (!checkOneHome)
			{
				cg.cust.visible = true;
				cg.cust.tf_custom.text = "\n\n\n\n\t\tERROR!\n\t\tNeed at least one satellite!";
			}
		}
		
		private function stripParen(a:Array, s:int, e:int):void
		{
			for (var j:uint = s; j <= e; j++)
			{
				var lastLine:int = (j == a.length - 1 ? 2 : 1);
				var prt:String = a[j];
				if (prt.charAt(prt.length - lastLine) == ")")
					prt = prt.substring(0, prt.length - lastLine);
				if (prt.charAt(0) == "(")
					prt = prt.substring(1, prt.length);
				a[j] = prt;
			}			
		}
		
		private function addMC(c:Array, mc:MovieClip):void
		{
			stripParen(c, 1, 2);
			mc.x = c[1]; mc.y = c[2];
			cg.terrain.addChild(mc);
		}
		
		private function onFileLoadError(event:Event):void
		{
			// Hide progress bar
			fileRef.removeEventListener(Event.COMPLETE, onFileLoaded);
			fileRef.removeEventListener(IOErrorEvent.IO_ERROR, onFileLoadError);		
			cg.cust.tf_custom.text = "Error loading file.";
		}  
		
		override public function loadScenario():void
		{
			/*
			fileRef = new FileReference();
			fileRef.addEventListener(Event.SELECT, onFileSelected);
			var fileFilter:FileFilter = new FileFilter("Text Files", "*.txt");
			fileRef.browse(fileFilter);*/
		
			
			with (cg)
			{
				//terrain.gotoAndStop("tutorial01");
				//var gfx:MovieClip = new MovieClip();
				//terrain.addChild(gfx);
				
				
				
				//turrMan.placeSatellite(-250, -200, "home", 1);
				//turrMan.placeTurret("turretNorm", -190, 0, "turret", 0, 1, 1, 45);
				
				//nodeMan.addEdge("home", "turret");
				//nodeMan.setOrigin("home");
				
				//planeMan.addPlane("passenger", -290, 190, -45, makeWaypoints([-200, 100,  -100, 0,  60, -100,  500, -500]));
			}
		}
		

	}
}