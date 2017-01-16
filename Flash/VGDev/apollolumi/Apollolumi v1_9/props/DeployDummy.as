// placeholder for deploying turrets

package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.StructureManager;
	import managers.Manager;

	public class DeployDummy extends Structure
	{	
		public function DeployDummy(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int)
		{
			super(_sm, _cg, _xP, _yP, 0, 0, 100000, 0);
			ID = "structure";
			TITLE = "Deployed Turret";
			mass = 999999;
			if (cg.minimap.contains(mapIcon))
				cg.minimap.removeChild(mapIcon);
			cg.sndMan.playSound("deploy");
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{			
			return false;
		}

		override public function collide(f:Floater):void
		{
			handleCollision(this, f);
		}
		
		override public function destroy():void
		{
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);
		}
	}
}
