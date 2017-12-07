package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.events.TimerEvent;
	import flash.geom.Point;
	import flash.utils.Timer;
	
	/**
	 * Specialized abstract class for vehicle 'projectiles'.
	 * 
	 * Includes support for hit points.
	 * 
	 * @author Alexander Huynh
	 */
	public class ABST_Vehicle extends ABST_Missile 
	{
		public var hpMax:int = 20
		public var hp:int = hpMax;
		
		protected const BAR_W:Number = 28.65;
		
		public function ABST_Vehicle(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			super(_cg, _mc, _origin, _target, _type, params);
			
			mc.rotation = rot = 0;			// don't rotate the MovieClip
		}		

		override public function destroy(distance:Number = 0):void
		{
			if (markedForDestroy) return;	// already going to be destroyed, quit
			
			// TODO check HP
			if (hp > 1)
			{
				hp--;
				mc.mc_hp.bar.width = (hp / hpMax) * BAR_W;
				return;
			}
			
			checkTarget(false);
			
			if (awardMoney && type == 0)
			{
				var m:int = money + money * (distance < 40 ? -.0125 * distance + .5 : 0)
				cg.addMoney(m);
				cg.manPart.popup(new Point(mc.x, mc.y - 20), m);
			}
			
			markedForDestroy = true;
			mc.visible = false;
 
			timerKill = new Timer(2000);
			timerKill.addEventListener(TimerEvent.TIMER, cleanup);
			timerKill.start();
			
			if (createExplosion)
			{
				cg.manExpl.spawnExplosion(new Point(mc.x, mc.y), type, explosionScale);
				for (var i:int = 0; i < 7 + getRand(0, 6); i++)
					cg.manPart.spawnParticle("debris", new Point(mc.x + getRand( -15, 15), mc.y + getRand( -10, 10)),
											 getRand(0, 360), getRand(-.5, .5), getRand(0, .5), 1.5);
			}

			if (tgt)
			{
				cg.game.c_main.removeChild(tgt);
				tgt.visible = false;
				tgt = null;
			}
		}
	}
}