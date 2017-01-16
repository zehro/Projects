package props
{
	import flash.display.MovieClip;
	import managers.Manager;
	import flash.geom.Point;

	public class DebrisFX extends Floater
	{		
		private var type:int;
	
		public function DebrisFX(_cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _type:int)
		{
			super(_cg, _xP, _yP, _dX, _dY, 0);
			type = _type;
			 gotoAndStop(type);

			hp = Math.floor(Math.random() * 300) + 120
			collidable = false;

			var rot:Number = Math.random() * 360;
			rotation = rot;
			
			if (cg.minimap.contains(mapIcon))
				cg.minimap.removeChild(mapIcon);
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			if (--hp == 0)
			{
				destroy();
				return true;
			}
			if (hp <= 60)
				alpha = hp * .016666;
			
			rotation += 4;
			
			return false;
		}
	}
}
