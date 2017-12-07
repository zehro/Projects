package scenarios
{
	public class ScTutorial01 extends Scenario			// IDENTIFIER: "Teach Me How to Hack"
	{
		public function ScTutorial01()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("tutorial01");
				
				turrMan.placeSatellite(-250, -200, "home", 1);
				turrMan.placeTurret("turretNorm", -190, 0, "turret", 0, 1, 1, 45);
				
				nodeMan.addEdge("home", "turret");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -290, 190, -45, makeWaypoints([-200, 100,  -100, 0,  60, -100,  500, -500]));
			}
		}
	}
}