package scenarios
{
	public class ScTutorial06 extends Scenario			// IDENTIFIER: "Putting It Together"
	{
		public function ScTutorial06()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("tutorial06");
				
				turrMan.placeSatellite(0, -220, "home", 4);
				turrMan.placeTurret("turretNorm", -220, -50, "west", 0, 1, 1, 90);
				turrMan.placeTurret("turretNorm", -60, 60, "middle", 0, 2, 1, 60);
				turret = turrMan.placeTurret("turretNorm", 190, 65, "east", 0, 1, 2, 300);
				turret.setAttribute("rof", 60);
				turret.setAttribute("range", 125);
				turret.setAttribute("rotSpdIdle", .8);
				turret.setAttribute("rotSpdTrack", 9);
				turrMan.placeTurret("turretNorm", 300, -200, "useless", 0, 1, 1, 330);
				
				nodeMan.addEdge("home", "west");
				nodeMan.addEdge("home", "middle");
				nodeMan.addEdge("home", "east");
				nodeMan.addEdge("home", "useless");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -370, -240, 45, makeWaypoints([-140, -70,  0, 40,  350, 200,  700, 400]));
			}
		}
	}
}