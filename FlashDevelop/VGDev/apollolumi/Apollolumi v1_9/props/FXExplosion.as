package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;

	public class FXExplosion extends Floater
	{			
		public function FXExplosion(_cg:MovieClip, _xP:int, _yP:int)
		{
			super(_cg, _xP, _yP, 0, 0, 0);
			collidable = false;
			if (cg.minimap.contains(mapIcon))
				cg.minimap.removeChild(mapIcon);
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			if (currentFrame == totalFrames)
			{
				stop();
				destroy();
				return true;
			}
			return false;
		}
	}
}
