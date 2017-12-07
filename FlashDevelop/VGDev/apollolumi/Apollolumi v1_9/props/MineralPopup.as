package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;

	public class MineralPopup extends Floater
	{			
		public function MineralPopup(_cg:MovieClip, _xP:int, _yP:int, amt:uint, type:int)
		{
			super(_cg, _xP, _yP, 0, 0, 0);
			collidable = false;
			if (cg.minimap.contains(mapIcon))
				cg.minimap.removeChild(mapIcon);
				
			switch (type)
			{
				case 0: base.num.textColor = 0xE52727; break;
				case 1: base.num.textColor = 0xCCCD23; break;
				case 2: base.num.textColor = 0x1E8834; break;
				case 3: base.num.textColor = 0x2790CD; break;
			}
			
			base.num.text = "+" + amt.toString();

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
