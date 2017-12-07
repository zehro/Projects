package scenarios
{
	public class ScBasics04 extends Scenario			// IDENTIFIER: "Leapfrog"
	{
		public function ScBasics04()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("basics04");
				
				turrMan.placeSatellite(-250, -100, "home", 1);
				turrMan.placeTurret("turretNorm", -100, 50, "turretW", 1, 1, 1, 270);
				turrMan.placeServer("serverNorm", 50, 35, "server", 1, 1, 1, true);
				turrMan.placeServer("serverNorm", 35, -20, "serverTroll", 0, 1, 2, true);
				turrMan.placeTurret("turretNorm", 200, 20, "turretE", 0, 1, 1, 180);
				
				nodeMan.addEdge("home", "turretW");
				nodeMan.addEdge("turretW", "server");
				nodeMan.addEdge("turretW", "serverTroll");
				nodeMan.addEdge("server", "turretE");
				nodeMan.addEdge("serverTroll", "turretE");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -370, 130, -15, makeWaypoints([-250, 121,  30, 90,  250, 47,  370, 15,  600, -15]));
			}
		}
	}
}