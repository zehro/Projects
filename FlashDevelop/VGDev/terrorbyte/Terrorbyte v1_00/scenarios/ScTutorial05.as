package scenarios
{
	public class ScTutorial05 extends Scenario			// IDENTIFIER: "One Two Punch"
	{
		public function ScTutorial05()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("tutorial05");
				
				turrMan.placeSatellite(-300, -220, "home", 2);
				turrMan.placeTurret("turretNorm", 0, -50, "turret", 0, 2, 1, 45);
				
				nodeMan.addEdge("home", "turret");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -350, 200, -45, makeWaypoints([-200, 70,  -100, 25,  -50, 5,  0, 0,  50, 5,
																			  100, 25,  175, 80, 600, 480]));
			}
		}
	}
}