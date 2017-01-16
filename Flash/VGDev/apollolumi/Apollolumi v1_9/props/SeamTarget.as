package props
{
	import flash.display.MovieClip;
	import flash.geom.ColorTransform;

	public class SeamTarget extends MovieClip
	{		
		private var par:MovieClip;
		public var hp:Number, hpMax:Number;
		private var stages:Array = [0, .35, .6, 1];		// public later to change?
		private var seamStage:int = 3;
		private var nextStage:Boolean = true;
		
		private var colGrn:ColorTransform = new ColorTransform();
		private var colYel:ColorTransform = new ColorTransform();
		private var colRed:ColorTransform = new ColorTransform();
	
		public function SeamTarget()
		{
			par = MovieClip(parent);
			hp = hpMax = 100;			// overridden in Asteroid
			
			colGrn.color = 0x00FF00;
			colYel.color = 0xFFFF00;
			colRed.color = 0xFF0000;
		}
		
		// returns -1 if destroyed, else seamStage
		public function damage(dmg:Number, ss:int, doDmg:Boolean):int
		{
			var hpPerc:Number = hp / hpMax;
			if (!nextStage || !doDmg)
				return seamStage;
			hp -= dmg * 10;
			hpPerc = hp / hpMax;
			if (nextStage && hpPerc < stages[ss])
			{
				seamStage--;
				nextStage = false;
				if (hp < 0)
				{
					destroy();
					return -1;
				}
				if (hpPerc < .35)
					transform.colorTransform = colRed;
				else if (hpPerc < .6)
					transform.colorTransform = colYel;
				else if (hpPerc < 1)
					transform.colorTransform = colGrn;
			}
			return seamStage;
		}
		
		public function reduceStage():void
		{
			nextStage = true;
		}
		
		public function destroy():void
		{
			if (par.contains(this))
				par.removeChild(this);
		}
	}
}
