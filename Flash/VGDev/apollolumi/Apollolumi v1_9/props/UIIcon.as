package props
{
	import flash.display.MovieClip
	
	public class UIIcon extends MovieClip
	{
		public var costs:Vector.<int> = new Vector.<int>(4, true);
		public var iconName:String, iconDes:String;
		
		public function UIIcon()
		{
			iconName = iconDes = "";
		}
		
		public function setInfo(t:String, d:String):void
		{
			iconName = t;
			iconDes = d;
		}
		
		public function setCosts(re:int, ye:int, gr:int, cy:int):void
		{
			costs[0] = re;
			costs[1] = ye;
			costs[2] = gr;
			costs[3] = cy;
		}
	}
}
