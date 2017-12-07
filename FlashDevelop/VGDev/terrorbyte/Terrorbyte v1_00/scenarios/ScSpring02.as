package scenarios
{
	import props.Turret;
	
	public class ScSpring02 extends Scenario			// IDENTIFIER: "Taking the Long Way"
	{
		public function ScSpring02()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring02");
				
				var turrets:Array = [];
				
				turrMan.placeSatellite(-200, -140, "home", 1);
				turrMan.placeServer("serverNorm", -76, -50, "serverN", 1, 1, 2);
				turrMan.placeServer("serverNorm", -70, 170, "serverS", 2, 1, 1, true);
				turrets.push(turrMan.placeTurret("turretNorm", -160, 40, "turretW", 1, 1, 1, 50));
				turrets.push(turrMan.placeTurret("turretNorm", 200, -40, "turretE", 0, 1, 0, 45));
				turrets.push(turrMan.placeTurret("turretNorm", 50, 50, "turretS", 1, 2, 1, 205));
				
				for each (var t:Turret in turrets)
					t.setAttribute("stunBase", 6);		// default 4
				turrets[1].setAttribute("range", 75);
				turrets[1].setAttribute("rotSpdIdle", .2);
				
				nodeMan.addEdge("home", "serverN");
				nodeMan.addEdge("home", "turretW");
				nodeMan.addEdge("serverN", "turretW");
				nodeMan.addEdge("serverN", "turretE");
				nodeMan.addEdge("serverN", "turretS");
				nodeMan.addEdge("serverS", "turretW");
				nodeMan.addEdge("serverS", "turretS");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("cargo", -360, 150, -20, makeWaypoints([366, -50,  600, -170]));
			}
		}
	}
}