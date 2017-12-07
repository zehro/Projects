package deltabattery.particles 
{
	import deltabattery.managers.ManagerParticle;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class ParticlePopup extends Particle 
	{
		
		public function ParticlePopup(_man:ManagerParticle, _mc:MovieClip, _origin:Point, _amt:int, isDay:Boolean) 
		{
			super(_man, _mc, _origin, 0, 0, 0, 0);
			
			if (!isDay)
				mc.base.gotoAndStop(2);
			mc.base.tf_amt.text = "$" + _amt;
			
			mc.scaleX = mc.scaleY = 1;
			mc.rotation = 0;
			rotSpd = 0;
		}
	}
}