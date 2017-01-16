package scenarios
{
	public class ScTutorial03 extends Scenario			// IDENTIFIER: "Tiering Up"
	{
		public function ScTutorial03()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("tutorial03");
				
				turrMan.placeSatellite(-30, -180, "home", 1);
				turrMan.placeTurret("turretNorm", 240, -205, "useless", 0, 1, 1, 0);
				turret = turrMan.placeTurret("turretNorm", 70, 10, "turret", 0, 1, 2, 180);
				turret.setAttribute("rof", 60);
				turret.setAttribute("range", 125);
				turret.setAttribute("rotSpdIdle", .8);
				turret.setAttribute("rotSpdTrack", 9);
				
				nodeMan.addEdge("home", "turret");
				nodeMan.addEdge("home", "useless");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -320, -220, 40, makeWaypoints([0, 0,  250, 160,  640, 440]));
			}
		}
	}
}