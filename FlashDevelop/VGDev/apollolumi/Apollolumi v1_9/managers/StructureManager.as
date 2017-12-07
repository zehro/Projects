package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import props.Structure;

	public class StructureManager extends Manager
	{
		public var strVec:Vector.<MovieClip> = new Vector.<MovieClip>();

		public function StructureManager(_par:MovieClip)
		{
			super(_par);
		}
		
		override public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			var i:int;
			if (strVec.length == 0) return;
			for (i = strVec.length - 1; i >= 0; i--)
				if (strVec[i].step(colVec, p))
				{
					strVec[i].destroy();
					strVec.splice(i, 1);
				}
		}
		
		public function addStruct(s:Structure):void
		{
			par.game.cont.addChild(s);
			strVec.push(s);
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return strVec;
		}
		
		override public function destroy():void
		{
			for each (var s:Structure in strVec)
			{
				s.destroy();
				if (par.game.cont.contains(s))
					par.game.cont.removeChild(s);
			}
			strVec = new Vector.<MovieClip>();
		}
	}
}
