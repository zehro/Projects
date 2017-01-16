package deltabattery.particles 
{
	import flash.display.MovieClip;
	import deltabattery.ABST_Base;
	import deltabattery.managers.ManagerParticle;
	import flash.geom.Point;
	
	/**
	 * 	A MovieClip acting as a particle.
	 *
	 * 	For used in a factory/warehouse configuration in ManagerParticle.
	 * 
	 *	@author Alexander Huynh
	 */
	public class Particle extends ABST_Base 
	{
		protected var man:ManagerParticle;
		public var factoryIndex:int;		// used in ManagerParticle
		public var factoryKey:int;
		
		public var mc:MovieClip;			// the actual particle MovieClip
		protected var mcTotal:int;			// mc.totalFrames
		
		public var dx:Number;		// amount to move the particle by per step
		public var dy:Number;
		public var g:Number;		// gravity
		
		public var rotSpd:Number = 0;
		
		private var idle:Boolean;	// speed improvement; TRUE if this particle has finished animating
		
		public function Particle(_man:ManagerParticle, _mc:MovieClip, _origin:Point, _rot:Number, _dx:Number = 0, _dy:Number = 0, _g:Number = 0) 
		{
			super();
			
			man = _man;
			mc = _mc;
			mcTotal = mc.totalFrames;
			
			reuse(_origin, _rot, _dx, _dy, _g);
		}
		
		// initialization - can be called to reuse this particle from ManagerParticle
		public function reuse(origin:Point, rot:Number, _dx:Number = 0, _dy:Number = 0, _g:Number = 0):void
		{
			mc.gotoAndPlay(1);
			mc.x = origin.x; mc.y = origin.y;
			
			dx = _dx; dy = _dy;
			g = _g;
			
			if (!rot)
			{
				mc.rotation = getRand( -180, 180);
				rotSpd = getRand( -.5, .5);
			}
			else
				mc.rotation = rot;
			
			mc.scaleX = mc.scaleY = getRand(.8, 1);
			idle = false;
		}
		
		// returns TRUE if this particle should be removed, FALSE otherwise
		public function step():Boolean
		{
			if (idle) return true;				// speed improvement
			
			if (mc.currentFrame == mcTotal)
			{
				mc.stop();
				idle = true;
				return true;
			}
			
			mc.rotation += rotSpd;
			mc.x += dx;
			mc.y += dy + g;
			
			return false;
		}
	}
}