package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import props.Floater;
	import managers.Manager;
	import managers.ProjectileManager;

	public class Projectile extends Floater
	{
		private var manager:ProjectileManager;
		private var type:String;
		private var dmg:Number;
		
		public var isFriendly:Boolean;
		public var splash:Boolean;
		public var splashRange:Number = 50;
		public var tracking:Boolean;
		public var tgt:Floater;
		public var tgtSpd:Number = 5;	// ability for tracking things to turn

		public var moveSpd:Number = 1.7;		// acceleration per frame
		public var maxSpd:Number = 6;		// go no faster than this
		
		public var isRailgun:Boolean;
		public var isRailgunTrail:Boolean;
	
		public function Projectile(pm:ProjectileManager, _cg:MovieClip, _xP:int, _yP:int, _spd:Number, _rot:Number, _type:String, _life:int, _dmg:Number,
								   _isFriendly:Boolean, _tgt:Floater = null)
		{
			super(_cg, _xP, _yP, 0, 0, _life);
			
			manager = pm;
			
			rotation = _rot;
			dmg = _dmg;
			isFriendly = _isFriendly;
			tgt = _tgt;
			
			collidable = false;
			ignoreBounds = true;
			
			dY = Math.sin(degreesToRadians(rotation)) * _spd;
			dX = Math.cos(degreesToRadians(rotation)) * _spd;
			
			type = _type;
			if (type == "laserT")
			{
				gotoAndStop("laser");
				isRailgunTrail = true;
				collidable = false;
				if (cg.minimap.contains(mapIcon))
					cg.minimap.removeChild(mapIcon);
			}
			else
			{
				gotoAndStop(type);
				setMapIcon(0xEE0000, "shot");
			}
			
			ID = "projectile";
			
			switch (type)
			{
				case "energy":
					widthSP = heightSP = 30;		// override automatic setting
				break;
				case "bomb":
					widthSP = heightSP = 50;		// override automatic setting
					splash = true;
				break;
				case "missile":
					widthSP = heightSP = 40;
					splash = true;
				break;
				case "laser":
					isRailgun = true;
					ignoreBounds = true;
					widthSP = heightSP = 80;
				break;
			}
			
			if (tgt)
				tracking = true;
				
			//trace(type + " is going for a " + tgt + "(" + tracking + ")");
		}
		
		override public function collide(f:Floater):void
		{
			//trace(type + " hit a " + f);
			if (isFriendly && f.ID == "structure") return;
			if (!isFriendly && f.ID == "enemy") return;
			cg.changeHP(-dmg, f);
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{			
			// will not be run in step, so 'override' it here
			checkColl(colVec);
			if (--hp <= 0)
				destroy();

			if (isRailgun)
			{
				var proj:Projectile = new Projectile(manager, cg, x, y, 0, rotation, "laserT", 30, 0, true);
				cg.game.cont.addChild(proj);
				cg.objArr.push(proj);
			}
			
			if (isRailgunTrail && hp < 30)
				alpha = hp / 30;

			if (tgt)
			{
				if (tracking && tgt)
				{
					var tgtAng:Number = getAngle(x, y, tgt.x, tgt.y);
					if (tgtAng > rotation + 180) tgtAng -= 360;
					if (tgtAng < rotation - 180) tgtAng += 360;
						
					var rotInc:Number = (tgtAng - rotation) * .6;
					if (Math.abs(rotInc) > tgtSpd)
						rotInc = (rotInc < 0 ? -1 : 1) * tgtSpd;
					rotation += rotInc;	
	
					dY += Math.sin(degreesToRadians(rotation)) * moveSpd;
					dX += Math.cos(degreesToRadians(rotation)) * moveSpd;
	
					if (abs(dX) > maxSpd)
						dX = maxSpd * (dX > 0 ? 1 : -1);
					if (abs(dY) > maxSpd)
						dY = maxSpd * (dY > 0 ? 1 : -1);
						
					//if (getDistN(x, y, tgt.x, tgt.y < 60))		// proximity
						//destroy();
				}
				if (tgt.hp <= 0)
					destroy();
			}

			return hp <= 0;
		}
		
		private function handleSplash():void
		{
			var dist:Number = 0;
			var f:Floater = null;
			for each (var m:Manager in cg.managerC)	
				for each (var mc:MovieClip in m.getVec())
				{
					if (!(mc is Floater) || mc == this) continue;
					f = mc as Floater;
					if (f.ID == "structure" || f.ID == "projectile") continue;
					dist = getDist(this, f);
					if (dist < splashRange)
						cg.changeHP(-dmg * (dist / splashRange), f);
				}
			splash = false;
		}
		
		// handles when player lasers this object
		public function leftMouse(p:Point):void
		{
			if (!hitTestPoint(p.x, p.y)) return;
			destroy();
		}
		
		override public function destroy():void
		{
			hp = 0;
			collidable = false;
			tgt = null;
			tracking = false;
			switch (type)
			{
				case "energy":
					cg.sndMan.playSound("impact1");
					cg.addExplosion(x, y, .5);
				break;
				case "bomb":
					cg.sndMan.playSound("impact1");
					cg.addExplosion(x, y, 1);
				break;
				case "missile":
					cg.sndMan.playSound("explode2");
					cg.addExplosion(x, y, 1);
				break;
			}
			if (splash)
				handleSplash();
			if (cont.contains(this))
				cont.removeChild(this);
		}
	}
}