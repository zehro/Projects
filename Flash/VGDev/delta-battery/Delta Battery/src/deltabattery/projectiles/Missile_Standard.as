package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import deltabattery.SoundPlayer;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 *	Standard Missile.
	 *
	 * 	A basic missile. Travels from its origin to its target.
	 * 
	 *	@author Alexander Huynh
	 */
	public class Missile_Standard extends ABST_Missile 
	{
		
		public function Missile_Standard(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			super(_cg, _mc, _origin, _target, _type, params);
			
			if (type == 0)
				SoundPlayer.play("sfx_launch_os_standard");
		}
	}
}