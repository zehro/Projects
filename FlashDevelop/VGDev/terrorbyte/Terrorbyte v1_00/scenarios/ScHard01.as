package scenarios
{
	import props.Turret;
	
	public class ScHard01 extends Scenario			// IDENTIFIER: "Instant"
	{
		public function ScHard01()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("bayside");
				
				turrMan.placeSatellite(-160, 130, "home", 5);
				turrMan.placeServer("serverNorm", 150, 90, "beach1", 3, 2, 0, true);
				turrMan.placeServer("serverNorm", -300, -75, "beach2", 2, 2, 1, true);
				
				turrMan.placeTurret("laser", -7, -130, "laserL", 1, 1, 1, 0);
				turret = turrMan.placeTurret("laser", 140, 0, "laserR", 0, 1, 2, 0);
					turret.setAttribute("rof", 45);
					turret.setAttribute("range", 150);
					
				turrMan.placeTurret("turretNorm", -150, -230, "turretW", 1, 1, 1, 180);
				turrMan.placeTurret("turretNorm", 313, 120, "turretE", 1, 1, 1, 0);
				
				nodeMan.addEdge("home", "beach1");
				nodeMan.addEdge("home", "beach2");
				nodeMan.addEdge("beach2", "turretW");
				nodeMan.addEdge("beach1", "laserR");
				nodeMan.addEdge("laserL", "laserR");
				nodeMan.addEdge("laserL", "turretW");
				nodeMan.addEdge("beach2", "laserL");
				nodeMan.addEdge("beach1", "turretE");
				nodeMan.addEdge("turretE", "laserR");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -377, 255, -45, makeWaypoints([50, -70,  370, -270,  600, -500]));	
				planeMan.addPlane("fighter", -372, 264, -30, makeWaypoints([95, 170,  253, 95,  330, -40,  380, -250,  600, -400]));	
			}
		}
	}
}