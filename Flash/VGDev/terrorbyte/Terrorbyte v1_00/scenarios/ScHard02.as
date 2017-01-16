package scenarios
{
	import props.Turret;
	
	public class ScHard02 extends Scenario			// IDENTIFIER: "Around the Bend"
	{
		public function ScHard02()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("riverMountain");
				
				turrMan.placeSatellite(-200, -250, "home", 4);
				turrMan.placeServer("serverNorm", -182, -118, "serverNW", 3, 2, 0, true);
				turrMan.placeServer("serverNorm", 80, 135, "serverC", 4, 2, 0, true);
				turrMan.placeServer("serverNorm", 215, -160, "serverM", 6, 2, 3);		
				
				turrMan.placeTurret("laser", -54, 267, "laserS", 1, 1, 1, 0);
				turret = turrMan.placeTurret("laser", 290, 145, "laserSE", 0, 1, 0, 0);
					turret.setAttribute("range", 50);
					
				turrMan.placeTurret("turretNorm", -257, 50, "turretW", 0, 1, 1, 180);
				turrMan.placeTurret("turretNorm", -66, -122, "turretC", 1, 1, 1, 180);
				turrMan.placeTurret("turretNorm", 260, 70, "turretE", 0, 2, 1, 0);
				
				nodeMan.addEdge("home", "turretC");
				nodeMan.addEdge("home", "serverNW");
				nodeMan.addEdge("serverNW", "turretC");
				nodeMan.addEdge("serverNW", "serverC");
				nodeMan.addEdge("serverNW", "turretW");
				nodeMan.addEdge("serverM", "turretC");
				nodeMan.addEdge("serverM", "turretE");
				nodeMan.addEdge("serverC", "turretE");
				nodeMan.addEdge("serverC", "laserS");
				nodeMan.addEdge("serverC", "laserSE");
				nodeMan.addEdge("turretE", "laserSE");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -370, -250, 60, makeWaypoints([-175, 61,  -80, 180,  80, 230,  208, 220,  332, 135, 381, 4,  600, -100]));	
				planeMan.addPlane("swept", -222, -227, 60, makeWaypoints([-162, -156,  -112, 0,  -44, 125,  60, 166,  176, 166,  264, 116,  360, -25,  600, -125]));	
			}
		}
	}
}