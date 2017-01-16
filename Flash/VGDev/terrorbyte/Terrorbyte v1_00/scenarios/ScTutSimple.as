package scenarios
{
	public class ScTutSimple extends Scenario			// IDENTIFIER: "Basics"
	{
		public function ScTutSimple()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("demo");
				
				turrMan.placeSatellite(-260, -250, "satellite", 1);
				turrMan.placeServer("serverNorm", -180, -170, "top", 2, 1, 0, true);
				turrMan.placeServer("serverNorm", 160, 60, "right", 3, 2, 0, true);
				turrMan.placeServer("serverNorm", -80, 170, "bot", 2, 2, 0, true);
				
				turrMan.placeTurret("turretNorm", -220, 0, "west", 1, 1, 1, -20);
				turrMan.placeTurret("turretNorm", 25, -100, "high", 0, 1, 1, 0);
				turrMan.placeTurret("turretNorm", 210, -40, "far", 0, 2, 1, -45);

				nodeMan.addEdge("satellite", "top");
				nodeMan.addEdge("top", "bot");
				nodeMan.addEdge("top", "west");
				nodeMan.addEdge("top", "high");
				nodeMan.addEdge("west", "bot");
				nodeMan.addEdge("bot", "right");
				nodeMan.addEdge("right", "high");
				nodeMan.addEdge("right", "far");
				
				nodeMan.setOrigin("satellite");
							
				planeMan.addPlane("transport", -370, 270, -45, makeWaypoints([-200, 70,  -100, 25,  -30, 5,  25, -15,  65, -35,
																			   300, -180,  600, -480]));
			}
		}
	}
}