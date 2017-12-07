package scenarios
{
	public class ScBasics01 extends Scenario			// IDENTIFIER: "Serve Me"
	{
		public function ScBasics01()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("basics01");
				
				turrMan.placeSatellite(-200, -240, "home", 1);
				turrMan.placeServer("serverNorm", -80, -90, "server", 1, 1, 1, true);
				turrMan.placeTurret("turretNorm", -20, 110, "turret", 0, 1, 1, 0);
				
				nodeMan.addEdge("home", "server");
				nodeMan.addEdge("turret", "server");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -370, -240, 60, makeWaypoints([-290, -120,  -180, -20,  -100, 120,  10, 270,  150, 600]));
			}
		}
	}
}