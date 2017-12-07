package scenarios
{
	public class ScTutorial02 extends Scenario			// IDENTIFIER: "Timing is Everything"
	{
		public function ScTutorial02()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("tutorial02");
				
				turrMan.placeSatellite(-150, -140, "home", 2);
				turrMan.placeTurret("turretNorm", -180, 60, "turret1", 0, 1, 1, 45);
				turrMan.placeTurret("turretNorm", 145, -30, "turret2", 0, 1, 1, 260);
				
				nodeMan.addEdge("home", "turret1");
				nodeMan.addEdge("home", "turret2");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -320, 220, -45, makeWaypoints([0, 0,  250, -160,  640, -440]));
			}
		}
	}
}