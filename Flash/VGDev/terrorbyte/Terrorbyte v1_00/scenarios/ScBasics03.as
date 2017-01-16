package scenarios
{
	public class ScBasics03 extends Scenario			// IDENTIFIER: "Fork It"
	{
		public function ScBasics03()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("basics03");
				
				turrMan.placeSatellite(0, -210, "home", 1);
				turrMan.placeServer("serverNorm", 0, 25, "serverC", 2, 1, 1, true);
				turrMan.placeServer("serverNorm", 260, -60, "serverE", 2, 1, 2, true);
				turrMan.placeTurret("turretNorm", 170, 120, "turretE", 0, 2, 1, 90);
				turrMan.placeTurret("turretNorm", -240, 100, "turretW", 0, 1, 1, 0);
				
				nodeMan.addEdge("home", "serverC");
				nodeMan.addEdge("serverC", "serverE");
				nodeMan.addEdge("serverC", "turretE");
				nodeMan.addEdge("serverC", "turretW");
				nodeMan.addEdge("serverE", "turretE");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -309, -270, 90, makeWaypoints([-303, -106,  -293, -61,  -244, 29,  -182, 73,  -112, 85,
																			  6, 81,  115, 60,  230, 47,  311, 47,  370, 51,  600, 50]));
			}
		}
	}
}