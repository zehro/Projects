package scenarios
{
	import props.Turret;
	
	public class ScSpring07 extends Scenario			// IDENTIFIER: "Sacrifice"
	{
		public function ScSpring07()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring07");
				
				turrMan.placeSatellite(-330, -116, "home", 1);
				turrMan.placeServer("serverNorm", -100, 105, "server", 1, 1, 1, true);
				turrMan.placeTurret("turretNorm", 95, -116, "turretN", 0, 1, 1, 45);
				turrMan.placeTurret("turretNorm", 217, 38, "turretE", 1, 1, 1, 45);
				
				nodeMan.addEdge("server", "home");
				nodeMan.addEdge("server", "turretN");
				nodeMan.addEdge("server", "turretE");
				nodeMan.addEdge("turretN", "turretE");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -375, -200, 0, makeWaypoints([0, -200,  50, -190,
																	     170, -156,  240, -60,
																	     280, 116,  280, 170,  280, 400]));			
				planeMan.addPlane("drone", -375, 120, -45, makeWaypoints([-142, -107,  -15, -205,
																			 161, -194,  287, -115,
																			 315, 100,  315, 170,  315, 400]),
																			 false);
																			
																			
																			
			}
		}
	}
}