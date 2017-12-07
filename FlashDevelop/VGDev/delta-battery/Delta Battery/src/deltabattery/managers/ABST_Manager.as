package deltabattery.managers
{
	import cobaltric.ContainerGame;
	import deltabattery.ABST_Base;
	import flash.display.MovieClip;
	
	/**	An abstract Manager
	 * 
	 * 	A Manager controls a specific object, keeps track of
	 * 	them in objArr, and is steppable by ContainerGame.
	 * 
	 * @author Alexander Huynh
	 */
	public class ABST_Manager extends ABST_Base
	{
		public var cg:ContainerGame;
		public var objArr:Array;			// an array to hold this specific Manager's objects
		
		public function ABST_Manager(_cg:ContainerGame) 
		{
			cg = _cg;
			objArr = [];
		}
		
		// called by ContainerGame - should step and remove (if applicable) all of its objects
		public function step():void
		{
			// -- override this function
		}
		
		public function hasObjects():Boolean
		{
			return objArr.length > 0;
		}
		
		public function destroy():void
		{
			trace(this + " destroying...");
			var mc:MovieClip;
			for (var i:int = 0; i < objArr.length; i++)
			{
				if (!mc) continue;
				mc.cleanup();
				if (MovieClip(mc.parent).contains(mc))
					MovieClip(mc.parent).removeChild(mc);
				mc = null;
			}
			objArr = null;
			cg = null;
		}
	}

}