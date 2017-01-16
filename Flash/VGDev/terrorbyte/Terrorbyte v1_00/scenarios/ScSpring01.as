package scenarios
{
	import props.Turret;
	
	public class ScSpring01 extends Scenario			// IDENTIFIER: "Slipping Away"
	{
		public function ScSpring01()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring01");
				
				turrMan.placeSatellite(-170, -240, "home", 1);
				turrMan.placeServer("serverNorm", -86, -83, "serverT", 2, 1, 1);
				turrMan.placeServer("serverNorm", 73, -139, "serverP", 1, 1, 1, true);
				turrMan.placeTurret("turretNorm", -100, 135, "turretW", 0, 1, 0, 270);
				turrMan.placeTurret("turretNorm", 170, -45, "turretE", 0, 1, 1, 15);
				turrMan.placeTurret("turretNorm", 90, -235, "turretTroll", 0, 1, 1, 45);
				
				nodeMan.addEdge("home", "serverT");
				nodeMan.addEdge("serverT", "turretW");
				nodeMan.addEdge("serverT", "serverP");
				nodeMan.addEdge("serverP", "turretE");
				nodeMan.addEdge("serverP", "turretTroll");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -387, 246, -20, makeWaypoints([-114, 160,  71, 75,  296, -64,  389, -153,  600, -400]));
			}
		}
	}
}