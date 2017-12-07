package props
{
	import flash.display.MovieClip;

	public class FXExplosion extends Prop
	{			
		private var life:int;
		private var dAlpha:Number;
	
		public function FXExplosion(_xP:int, _yP:int, _life:int, _dAlpha:Number, scale:Number)
		{
			super(null, _xP, _yP);
			life = _life;
			dAlpha = _dAlpha;
			scaleX = scaleY = scale;
			
			mouseEnabled = false;
			buttonMode = false;
		}

		override public function childStep():Boolean
		{
			if (life > 0)
				life--;
			else
			{
				alpha -= dAlpha;
				if (alpha <= 0)
				{
					destroy();
					return true;
				}
			}
			
			return alpha == 0;
		}
		
		override public function destroy():void
		{
			life = 0;
			alpha = 0;
			if (parent && parent.contains(this))
				parent.removeChild(this);
		}
	}
}
