package utils
{
	import flash.display.MovieClip;
	import flash.geom.ColorTransform;
	import props.Floater;
	import managers.GUIManager;
	
	public class UIArrow extends MovieClip
	{
		public var manager:GUIManager;
		public var par:MovieClip;
		public var game:MovieClip;
		public var tgt:Floater;
		
		private var X_WINDOW:int = 400;	// half of screen size plus padding
		private var Y_WINDOW:int = 300;
		
		public function UIArrow(_manager:GUIManager, _par:MovieClip, _game:MovieClip, _tgt:Floater, col:uint)
		{
			manager = _manager;
			par = _par;
			tgt = _tgt;
			game = _game;
			var ct:ColorTransform = new ColorTransform();
			ct.color = col;
			this.transform.colorTransform = ct;
		}
		
		public function step():Boolean
		{
			if (tgt.hp <= 0) destroy();
			else
			{
				var xDist:Number = -game.x - tgt.x;
				var yDist:Number = -game.y - tgt.y;
				
				visible = ((xDist < 0 ? -xDist : xDist) > X_WINDOW) || ((yDist < 0 ? -yDist : yDist) > Y_WINDOW);
				if (visible)
				{					
					var dirChosen:Boolean;

					if (xDist < -X_WINDOW)
						x = -game.x + X_WINDOW - 20;
					else if (xDist > X_WINDOW)
						x = -game.x - X_WINDOW + 20;
					else
					{
						x = tgt.x;
						rotation = yDist > 0 ? 270 : 90;
						dirChosen = true;
					}

					if (yDist < -Y_WINDOW + 130)
						y = -game.y + Y_WINDOW - 20 - 130;
					else if (yDist > Y_WINDOW)
						y = -game.y - Y_WINDOW + 20;
					else
					{
						y = tgt.y;
						if (!dirChosen)
						{
							rotation = xDist > 0 ? 180 : 0;
							dirChosen = true;
						}
					}
					
					if (!dirChosen)
					{
						if ((xDist < 0 ? -xDist : xDist) < (yDist < 0 ? -yDist : yDist))
							rotation = xDist > 0 ? 180 : 0;
						else
							rotation = yDist > 0 ? 270 : 90;
					}
				}
			}
			return tgt.hp <= 0;
		}
		
		public function destroy():void
		{
			if (par.contains(this))
				par.removeChild(this);
		}
	}
}
