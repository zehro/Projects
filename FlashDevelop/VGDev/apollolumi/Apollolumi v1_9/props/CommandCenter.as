package props
{
	import flash.display.MovieClip;
	import managers.StructureManager;
	import managers.Manager;
	import flash.geom.Point;

	public class CommandCenter extends Structure
	{
		public function CommandCenter(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			SPID = "hq";
			TITLE = "Command Center";
			mass = 10;
			
			var tip:MovieClip = new tooltipDefend();		// make a "DEFEND" label
			cg.game.addChild(tip);
			tip.y -= 120;
			
			setMapIcon(0x3F9359, "cc");
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			handleTractor();
			
			rotation -= .5;
			gfx.rotation += .5;	// orientate tractor layer
			return hp == 0;
		}
		
		override public function collide(f:Floater):void
		{
			if (f.ID != "structure")
			{
				cg.changeHP(-5, f);		// -- temporary	
				trace("hit (CC): " + f);
			}
			handleCollision(this, f);
		}
		
		override public function destroy():void
		{
			hp = 0;
			cg.ccDestroyed();
		}
	}
}
