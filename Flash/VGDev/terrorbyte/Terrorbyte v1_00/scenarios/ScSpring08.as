package scenarios
{
	import props.Turret;
	
	public class ScSpring08 extends Scenario			// IDENTIFIER: "Ring of Death"
	{
		public function ScSpring08()
		{
		}
		
		override public function loadScenario():void
		{
			with (cg)
			{
				terrain.gotoAndStop("spring08");
				
				turrMan.placeSatellite(-250, -200, "home", 8);
				turrMan.placeTurret("turretNorm", -155, -54, "turret1", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", -64, -128, "turret2", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", 64, -128, "turret3", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", 155, -54, "turret4", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", 155, 54, "turret5", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", 64, 128, "turret6", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", -64, 128, "turret7", 0, 1, 0, 0);
				turrMan.placeTurret("turretNorm", -155, 54, "turret8", 0, 1, 0, 0);
				
				for (var i:int = turrMan.turrVec.length - 1; i >= 1; i--)
					turrMan.turrVec[i].setAttribute("stunBase", 12);
				
				nodeMan.addEdge("home", "turret1");
				nodeMan.addEdge("turret1", "turret2");
				nodeMan.addEdge("turret2", "turret3");
				nodeMan.addEdge("turret3", "turret4");
				nodeMan.addEdge("turret4", "turret5");
				nodeMan.addEdge("turret5", "turret6");
				nodeMan.addEdge("turret6", "turret7");
				nodeMan.addEdge("turret7", "turret8");
				nodeMan.addEdge("turret8", "turret1");
				nodeMan.setOrigin("home");
				
				planeMan.addPlane("cargo", -393, 261, -55, makeWaypoints([-393, 261,  -313, 109,  -243, 30,
																			  -115, 0,  60, 0,  177, -27,  275, -80,
																			   338, -160,  460, -600]));
																			
			}
		}
	}
}