package scenarios
{
	import props.Turret;
	
	public class ScSpring05 extends Scenario			// IDENTIFIER: "Server Overload"
	{
		public function ScSpring05()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring05");
				
				turrMan.placeSatellite(-300, -260, "home", 2);
				turrMan.placeServer("serverNorm", -100, -50, "serverC", 2, 2, 0, true);
				turrMan.placeServer("serverNorm", 60, 125, "serverE", 3, 2, 0, true);
				turrMan.placeServer("serverNorm", -270, 125, "serverW", 3, 1, 0, true);
				turrMan.placeServer("serverNorm", -135, 230, "serverS", 2, 2, 0, true);
				turrMan.placeServer("serverNorm", 60, 230, "serverSE", 2, 1, 0, true);
				turrMan.placeTurret("turretNorm", 180, 50, "turret", 0, 6, 2, 0);
				
				nodeMan.addEdge("home", "serverC");
				nodeMan.addEdge("home", "serverW");
				nodeMan.addEdge("serverC", "serverW");
				nodeMan.addEdge("serverC", "serverSE");
				nodeMan.addEdge("serverC", "turret");
				nodeMan.addEdge("turret", "serverE");
				nodeMan.addEdge("serverE", "serverSE");
				nodeMan.addEdge("serverSE", "serverS");
				nodeMan.addEdge("serverS", "serverW");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("cargo", -400, -180, 50, makeWaypoints([-332, -100,  -141, 16,  60, 65,
																		  300, 150,  600, 250]));
			}
		}
	}
}