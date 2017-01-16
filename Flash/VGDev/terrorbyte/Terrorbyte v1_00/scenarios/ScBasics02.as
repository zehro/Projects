package scenarios
{
	public class ScBasics02 extends Scenario			// IDENTIFIER: "Dual-pronged"
	{
		public function ScBasics02()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("basics02");
				
				turrMan.placeSatellite(-220, -200, "home", 1);
				turrMan.placeServer("serverNorm", -70, -90, "server", 2, 1, 1, true);
				turrMan.placeServer("serverNorm", -15, -125, "serverTroll", 2, 1, 4, true);
				turrMan.placeTurret("turretNorm", 160, -50, "east", 0, 1, 1, 0);
				turrMan.placeTurret("turretNorm", 0, 50, "south", 0, 1, 1, 0);
				
				nodeMan.addEdge("home", "server");
				nodeMan.addEdge("home", "serverTroll");
				nodeMan.addEdge("east", "server");
				nodeMan.addEdge("south", "server");
				nodeMan.addEdge("east", "serverTroll");
				nodeMan.addEdge("south", "serverTroll");

				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -370, -25, 0, makeWaypoints([0, 0,  360, 20,  700, 25]));
			}
		}
	}
}