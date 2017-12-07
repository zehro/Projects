package scenarios
{
	public class ScTutorial04 extends Scenario			// IDENTIFIER: "Double Threat"
	{
		public function ScTutorial04()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("tutorial04");
				
				turrMan.placeSatellite(-250, -150, "home", 2);
				turrMan.placeTurret("turretNorm", -25, 65, "turret1", 0, 1, 1, 250);
				turret = turrMan.placeTurret("turretNorm", -80, -85, "turret2", 0, 1, 2, 0);
				turret.setAttribute("rof", 60);
				turret.setAttribute("range", 125);
				
				nodeMan.addEdge("home", "turret1");
				nodeMan.addEdge("home", "turret2");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", 0, 260, 270, makeWaypoints([0, 0,  15, -240,  15, -640]));
			}
		}
	}
}