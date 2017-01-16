package props
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import managers.MineralManager;
	import managers.Manager;
	import props.MineralPopup;

	public class Mineral extends Floater
	{		
		private var manager:MineralManager;
		private var type:int;
		private var amount:int;
		private var rotAmt:Number;
		private var origPt:Point;
		
		public var dragged:Boolean;		// if being collected by a structure
	
		public function Mineral(mm:MineralManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number,  _hp:Number, _type:int, _impure:Number = 0)
		{
			super(_cg, _xP, _yP, _dX, _dY, _hp);
			ID = "mineral";
			manager = mm;
			type = _type;
			gotoAndStop(type + 1);
			collidable = false;

			var col:uint = 0xE52727;
			if (type == 1)
				col = 0xCCCD23;
			else if (type == 2)
				col = 0x1E8834;
			else if (type == 3)
				col = 0x2790CD;

			TITLE = "Mineral";
			setMapIcon(col, "m");
			
			rotAmt = getRand(-3, 3);
			if (rotAmt == 0)
				rotAmt = 1 * (Math.random() > .5 ? 1 : -1);
				
			origPt = new Point();
			
			// reduce potency of this mineral if it was spawned from an impure asteroid
			amount = getRand(3, 5) * int(_impure == 0 ? 1 : _impure) + 1;
			if (_impure != 0)
				base.scaleX = base.scaleY = _impure + .5;

			addEventListener(Event.ADDED_TO_STAGE, init);
		}

		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			var rot:Number = Math.random() * 360;
			rotation = rot;
			tgt.rotation = -rot;
		}
		
		// set forceCollect to TRUE if structure is gathering this
		public function rightMouse(forceCollect:Boolean = false):void
		{
			if (!forceCollect)
			{
				var p:Point = localToGlobal(new Point(mouseX, mouseY));
				if (!base.hitTestPoint(p.x, p.y)) return;
			}
			cg.changeMineral(type, amount);	
			cg.sndMan.playSound("minPickup");
			var mp:MineralPopup = new MineralPopup(cg, x, y, amount, type);
			cg.game.cont.addChild(mp);
			cg.objArr.push(mp);
			destroy();
		}

		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			var p:Point = localToGlobal(new Point(mouseX, mouseY));
			var m:Point = localToGlobal(origPt);

			rotation += rotAmt;
			tgt.rotation -= rotAmt;

			tgt.visible = getDistN(m.x, m.y, p.x, p.y) < uiDist;

			return hp == 0;
		}
	}
}
