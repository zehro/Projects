package scenarios
{
	public class ScTutOld extends Scenario			// IDENTIFIER: "Basics"
	{
		public function ScTutOld()
		{
		}
		
		override public function loadScenario():void
		{
			promptText = "Disable all turrets.";
			with (cg)
			{
				terrain.gotoAndStop("bayRiver");
				
				turrMan.placeSatellite(-220, 250, "satellite", 2);
				turrMan.placeServer("serverNorm", -50, -120, "northCoast", 0, 1, 1, true);
				turrMan.placeServer("serverNorm", 140, 0, "upper", 1, 1, 1, true);
				turrMan.placeTurret("turretNorm", -280, -170, "west", 1, 1, 1, getRand(0, 360));
				turrMan.placeTurret("turretNorm", 275, -58, "east", 1, 1, 3, getRand(0, 360));
				turrMan.placeTurret("turretNorm", 170, 150, "lower", 0, 1, 2, getRand(0, 360));

				nodeMan.addEdge("satellite", "northCoast");
				nodeMan.addEdge("satellite", "upper");

				nodeMan.addEdge("west", "northCoast");
				nodeMan.addEdge("northCoast", "upper");
				nodeMan.addEdge("upper", "east");
				nodeMan.addEdge("lower", "east");
				nodeMan.addEdge("upper", "lower");
				
				nodeMan.setOrigin("satellite");
							
				planeMan.addPlane("fighter", -300, 100, -45, makeWaypoints([]));
				planeMan.addPlane("passenger", -300, 150, -45, makeWaypoints([]));
				planeMan.addPlane("swept", -300, 200, -45, makeWaypoints([]));
			}
		}
	}
}