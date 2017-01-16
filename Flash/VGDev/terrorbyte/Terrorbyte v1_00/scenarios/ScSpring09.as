package scenarios
{
	import props.Turret;
	
	public class ScSpring09 extends Scenario			// IDENTIFIER: "Seeing Double"
	{
		public function ScSpring09()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring09");
				
				turrMan.placeSatellite(0, -240, "home", 2);
				turrMan.placeServer("serverNorm", 0, 0, "server1", 6, 2, 1);
				turrMan.placeServer("serverNorm", 0, 150, "server2", 4, 2, 0);
				turrMan.placeTurret("turretNorm", -200, 0, "turretW1", 0, 2, 1, 0);
				turret = turrMan.placeTurret("turretNorm", 200, 0, "turretE1", 0, 2, 1, -180);
				turret.setAttribute("rotDir", -1);
				turrMan.placeTurret("turretNorm", -140, 150, "turretW2", 0, 2, 1, 0);
				turret = turrMan.placeTurret("turretNorm", 140, 150, "turretE2", 1, 2, 1, -180);
				turret.setAttribute("rotDir", -1);
				
				nodeMan.addEdge("home", "server1");
				nodeMan.addEdge("server1", "turretW1");
				nodeMan.addEdge("server1", "turretE1");
				nodeMan.addEdge("server1", "server2");
				nodeMan.addEdge("server2", "turretW2");
				nodeMan.addEdge("server2", "turretE2");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -275, -270, 60, makeWaypoints([-45, 225,  0, 600]));	
				planeMan.addPlane("passenger", 275, -270, 120, makeWaypoints([45, 225,  0, 600]));	
			}
		}
	}
}