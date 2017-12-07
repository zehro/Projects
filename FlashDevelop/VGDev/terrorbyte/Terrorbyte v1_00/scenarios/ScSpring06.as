package scenarios
{
	import props.Turret;
	
	public class ScSpring06 extends Scenario			// IDENTIFIER: "Double BackDouble BackDouble BackDouble Back"
	{
		public function ScSpring06()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring06");
				
				turrMan.placeSatellite(-100, -200, "home", 1);
				turrMan.placeServer("serverNorm", 100, -87, "serverC", 1, 1, 1, true);
				turrMan.placeServer("serverNorm", 300, 50, "serverE", 0, 1, 1, true);
				turrMan.placeTurret("turretNorm", 23, 122, "turret", 1, 1, 1, 45);
				
				nodeMan.addEdge("serverC", "home");
				nodeMan.addEdge("serverC", "serverE");
				nodeMan.addEdge("serverC", "turret");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("cargo", -384, -288, 45, makeWaypoints([240, 175,  480, 325]));
				planeMan.addPlane("passenger", -384, 0, 5, makeWaypoints([-123, 22,  84, 102,
																		  234, 200,  468, 400]));			
			}
		}
	}
}