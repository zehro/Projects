package scenarios
{
	import props.Turret;
	
	public class ScSpring03 extends Scenario			// IDENTIFIER: "Pass it Along"
	{
		public function ScSpring03()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring03");
				
				turrMan.placeSatellite(-345, -250, "home", 1);
				turrMan.placeServer("serverNorm", -215, -175, "server1", 2, 1, 1, true);
				turrMan.placeServer("serverNorm", -80, -70, "server2", 0, 1, 1);
				turrMan.placeServer("serverNorm", 40, -35, "server3", 2, 1, 1, true);
				turrMan.placeTurret("turretNorm", 220, 55, "turretE", 0, 1, 1, 45);
				turrMan.placeTurret("turretNorm", 140, 220, "turretS", 0, 1, 1, 205);
				
				nodeMan.addEdge("home", "server1");
				nodeMan.addEdge("server1", "server2");
				nodeMan.addEdge("server2", "server3");
				nodeMan.addEdge("server3", "turretE");
				nodeMan.addEdge("server3", "turretS");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("cargo", -380, 250, -12, makeWaypoints([350, 104,  600, 50]));
			}
		}
	}
}