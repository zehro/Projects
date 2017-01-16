package scenarios
{
	import props.Turret;
	
	public class ScBasics06 extends Scenario			// IDENTIFIER: "Running the Gauntlet"
	{
		public function ScBasics06()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("basics06");
				
				var turrets:Array = [];
				
				turrMan.placeSatellite(-320, 0, "home", 1);
				turrets.push(turrMan.placeTurret("turretNorm", -130, -70, "turret0", 1, 1, 0, 270));
				turrets.push(turrMan.placeTurret("turretNorm", -60, 70, "turret1", 1, 1, 0, 145));
				turrets.push(turrMan.placeTurret("turretNorm", 25, -70, "turret2", 1, 1, 0, 180));
				turrets.push(turrMan.placeTurret("turretNorm", 110, 70, "turret3", 1, 1, 0, 180));
				turrets.push(turrMan.placeTurret("turretNorm", 230, -70, "turret4", 0, 1, 0, 180));
				
				for each (var t:Turret in turrets)
					t.setAttribute("stunBase", 6);		// default 4
				
				nodeMan.addEdge("home", "turret0");
				nodeMan.addEdge("turret0", "turret1");
				nodeMan.addEdge("turret1", "turret2");
				nodeMan.addEdge("turret2", "turret3");
				nodeMan.addEdge("turret3", "turret4");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("passenger", -350, -100, 0, makeWaypoints([350, -100,  600, -100]));
				planeMan.addPlane("passenger", -350, 100, 0, makeWaypoints([350, 100,  600, 100]));
			}
		}
	}
}