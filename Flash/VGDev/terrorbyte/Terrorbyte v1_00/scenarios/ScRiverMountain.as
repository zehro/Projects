package scenarios
{
	public class ScRiverMountain extends Scenario			// IDENTIFIER: "The Long Way"
	{
		public function ScRiverMountain()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("riverMountain");
				
				
				turrMan.placeSatellite(-300, -140, "starting", 4);
				turrMan.placeTurret("turretNorm", -110, -130, "closest", 1, 3, 1, getRand(0, 360));
				turrMan.placeServer("serverNorm", 40, 50, "mountains", 3, 1, 1);
				turrMan.placeTurret("turretNorm", -320, 30, "western", 0, 1, 1, getRand(0, 360));
				turrMan.placeTurret("turretNorm", 40, 220, "southern", 0, 2, 1, getRand(0, 360));
				turrMan.placeTurret("turretNorm", 315, 190, "eastern", 1, 1, 2, getRand(0, 360));
				
				nodeMan.addEdge("starting", "closest");
				nodeMan.addEdge("closest", "mountains");
				nodeMan.addEdge("western", "starting");
				nodeMan.addEdge("mountains", "southern");
				nodeMan.addEdge("mountains", "eastern");
				
				nodeMan.setOrigin("starting");
				nodeMan.updateGFX();
							
				//planeMan.addPlane("ultraTest", -360, -270, 45);
				//planeMan.addPlane("swept", -330, -270, 45);
				planeMan.addPlane("passenger", -310, -250, 45, makeWaypoints([-100, 50,  0, 75,  310, 270,  600, 500]));
				//planeMan.addPlane("swept", -270, -270, 45);
			}
		}
	}
}