package scenarios
{
	public class ScBasics05 extends Scenario			// IDENTIFIER: "Double Triple"
	{
		public function ScBasics05()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("basics05");
				
				turrMan.placeSatellite(-325, -230, "home", 1);
				turrMan.placeTurret("turretNorm", -255, 90, "turretW", 1, 1, 0, 270);
				turrMan.placeTurret("turretNorm", 0, -90, "turretC", 0, 2, 0, 145);
				turrMan.placeTurret("turretNorm", 260, -102, "turretE", 0, 1, 0, 180);
				turrMan.placeServer("serverNorm", -150, -25, "serverNW", 2, 1, 0, true);
				turrMan.placeServer("serverNorm", 202, 9, "serverNE", 3, 2, 0, true);
				turrMan.placeServer("serverNorm", 46, 180, "serverS", 2, 2, 0, true);
				
				nodeMan.addEdge("home", "serverNW");
				nodeMan.addEdge("serverNW", "turretW");
				nodeMan.addEdge("serverNW", "turretC");
				nodeMan.addEdge("serverNW", "serverS");
				nodeMan.addEdge("serverS", "turretW");
				nodeMan.addEdge("serverS", "serverNE");
				nodeMan.addEdge("serverNE", "turretC");
				nodeMan.addEdge("serverNE", "turretE");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -370, 280, -45, makeWaypoints([-126, 66,  -9, -27,  61, -52,  132, -64,
																			  212, -64,  282, -47,  388, 20,  600, 60]));
			}
		}
	}
}