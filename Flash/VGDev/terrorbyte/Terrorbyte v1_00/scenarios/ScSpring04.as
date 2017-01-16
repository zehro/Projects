package scenarios
{
	import props.Turret;
	
	public class ScSpring04 extends Scenario			// IDENTIFIER: "Make a Withdrawal"
	{
		public function ScSpring04()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring04");
				
				turrMan.placeSatellite(-240, -200, "home", 1);
				turrMan.placeServer("serverNorm", -120, 30, "serverC", 2, 1, 1, true);
				turrMan.placeServer("serverNorm", -40, -120, "serverB", 4, 2, 2, true);
				turrMan.placeServer("serverNorm", -190, 90, "serverTroll", 6, 3, 3, false);
				turrMan.placeTurret("turretNorm", 100, 50, "turretN", 0, 2, 1, 270);
				turrMan.placeTurret("turretNorm", 100, 170, "turretS", 0, 2, 1, 90);
				
				nodeMan.addEdge("home", "serverC");
				nodeMan.addEdge("serverC", "serverB");
				nodeMan.addEdge("serverC", "serverTroll");
				nodeMan.addEdge("serverC", "turretN");
				nodeMan.addEdge("serverC", "turretS");
				nodeMan.addEdge("turretN", "turretS");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -350, -280, 75, makeWaypoints([-317, -125,  -260, 0,  -190, 60,  -115, 90,
																			  0, 100,  200, 100,  335, 94,  600, 80]));
			}
		}
	}
}