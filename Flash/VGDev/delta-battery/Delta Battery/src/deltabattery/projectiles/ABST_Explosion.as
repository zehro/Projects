package deltabattery.projectiles
{
	import deltabattery.ABST_Base;
	import deltabattery.managers.ManagerArtillery;
	import deltabattery.managers.ManagerMissile;
	import deltabattery.SoundPlayer;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	
	/**	An abstract explosion
	 *
	 * 	Explosions		- kill all nearby missiles
	 * 					- disappear after their animation ends
	 * 					- usually do not affect bullets
	 * 					- usually do not move
	 * 
	 * @author Alexander Huynh
	 */
	public class ABST_Explosion extends ABST_Base 
	{
		public var par:MovieClip;
		public var mc:MovieClip;
		public var type:int;
		
		public var range:int;
		
		private var origin:Point;
		
		public function ABST_Explosion(_par:MovieClip, _mc:MovieClip, _origin:Point, _type:int = 0, scale:Number = 1, flak:Boolean = false) 
		{
			par = _par;
			mc = _mc;		
			origin = _origin;
			
			type = _type;
			
			mc.x = origin.x;
			mc.y = origin.y;
			
			mc.scaleX = mc.scaleY = scale;
			
			mc.rotation = getRand( -180, 180);
			
			range = 30 * scale;
			
			if (flak)
				SoundPlayer.play("db_explode_3");
			else if (scale > 1)
				SoundPlayer.play("db_explode_0");
			else if (scale == 1)
				SoundPlayer.play("db_explode_1");
			else
				SoundPlayer.play("db_explode_2");
		}
		
		public function step(manMiss:ManagerMissile, manArty:ManagerArtillery):Boolean
		{
			var miss:ABST_Missile;	
			var i:int;
			var d:Number;
			for (i = manMiss.objArr.length - 1; i >= 0; i--)
			{
				miss = manMiss.objArr[i]
				if (miss.type == 1) continue;		// ignore player projectiles

				d = getDistance(mc.x, mc.y, miss.mc.x, miss.mc.y);
				if (d < range)
					miss.destroy(d);
			}
			
			var arty:ABST_Artillery;			
			for (i = manArty.objArr.length - 1; i >= 0; i--)
			{
				arty = manArty.objArr[i]
				if (type == arty.type) continue;

				d = getDistance(mc.x, mc.y, arty.mc.x, arty.mc.y);
				if (d < range)
					arty.destroy(d);
			}
				
			if (mc.currentFrame != mc.totalFrames) return false;
			
			if (par.contains(mc))
				par.removeChild(mc);

			return true;
		}
	}
}