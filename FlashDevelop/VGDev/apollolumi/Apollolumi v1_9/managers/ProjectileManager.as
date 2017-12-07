package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import props.Projectile;
	import props.Floater;

	public class ProjectileManager extends Manager
	{
		public var projVec:Vector.<MovieClip> = new Vector.<MovieClip>();

		public function ProjectileManager(_par:MovieClip)
		{
			super(_par);
		}

		override public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			var i:int;
			if (projVec.length == 0) return;
			for (i = projVec.length - 1; i >= 0; i--)
				if (projVec[i].step(colVec, p))
					projVec.splice(i, 1);
				else if (ld)
					projVec[i].leftMouse(p);
		}

		public function addProjectile(xP:int, yP:int, spd:Number, rot:Number, type:String, life:int, dmg:Number, isFriendly:Boolean, tgt:Floater):void
		{
			var pro:Projectile = new Projectile(this, par, xP, yP, spd, rot, type, life, dmg, isFriendly, tgt);
			par.game.cont.addChild(pro);
			projVec.push(pro);
		}

		override public function getVec():Vector.<MovieClip>
		{
			return projVec;
		}

		override public function destroy():void
		{
			for each (var p:Projectile in projVec)
			{
				p.destroy();
				if (par.game.cont.contains(p))
					par.game.cont.removeChild(p);
			}
			projVec = new Vector.<MovieClip>();
		}
	}
}
