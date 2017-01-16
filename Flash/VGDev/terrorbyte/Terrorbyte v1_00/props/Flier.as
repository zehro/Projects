package props
{
	import flash.display.MovieClip;
	import managers.Manager;
	import flash.geom.Point;
	import flash.filters.GlowFilter;

	public class Flier extends Prop
	{
		public var isAlive:Boolean = true;
		
		public var moveSpd:Number = 0;
		public var rotSpd:Number = 0;
		public var facing:Number;	// rotation of base image
		
		protected var trailArr:Vector.<MovieClip> = new Vector.<MovieClip>();		// all trail movie clips
		protected var trailLength:Number = 20;				// max distance to draw line before this line is stowed
		protected var trailMC:MovieClip;					// current drawing movie clip
		protected var trailX:Number, trailY:Number;			// starting anchor for trail line (ends at current location)
		protected var trailSaturation:int = 7;				// index of trail to start making alpha
		protected var trailAlphaDelta:Number = .2;			// amount of alpha to apply to over-saturated MC's
		protected var trailColor:uint = 0xFFFFFF;
		protected var trailGlow:GlowFilter;
		
		public function Flier(man:Manager, _xP:int, _yP:int, _speed:Number, _dir:Number)
		{
			super(man, _xP, _yP);
			moveSpd = _speed;
			trailGlow = glowB;
			newTrail();
		}
		
		override public function step():Boolean
		{
			if (getDistN(x, y, trailX, trailY) > trailLength)
				newTrail();

			trailMC.graphics.clear();
			trailMC.graphics.lineStyle(1, trailColor, .5);		// TODO set dynamically
			trailMC.graphics.moveTo(trailX, trailY);
			trailMC.graphics.lineTo(x, y);
			
			return childStep();
		}
		
		private function newTrail():void
		{
			trailX = x;	trailY = y;
			trailMC = new MovieClip();
			trailMC.filters = [trailGlow];
			trailArr.unshift(trailMC);
			manager.cg.trailsGFX.addChild(trailMC);
			
			if (trailArr.length > trailSaturation)
			{
				var i:int, j:int = trailArr.length;
				var alph:Number = 1 - trailAlphaDelta;
				for (i = trailSaturation; i < j; i++)
				{
					if (alph <= 0)
					{
						if (manager.cg.trailsGFX.contains(trailArr[i]))
							manager.cg.trailsGFX.removeChild(trailArr[i]);
						trailArr.splice(i, j - 1);
						return;
					}
					else
					{
						trailArr[i].alpha = alph;
						alph -= trailAlphaDelta;
					}
				}
			}
		}
		
		override public function restart():void
		{
			clearTrail();
			childRestart();
		}		
		
		public function clearTrail():void
		{
			for each (var mc:MovieClip in trailArr)
				if (manager.cg.trailsGFX.contains(mc))
					manager.cg.trailsGFX.removeChild(mc);
		}
		
		public function childRestart():void
		{
			// -- override this function
		}	
		
		protected function forward():void
		{
			y += Math.sin(degreesToRadians(facing)) * moveSpd;
			x += Math.cos(degreesToRadians(facing)) * moveSpd;
		}
		
		override public function destroy():void
		{
			childDestroy();
			if (parent && parent.contains(this))
				parent.removeChild(this);
		}
		
		protected function childDestroy():void
		{
			// -- override this function
		}
	}
}
