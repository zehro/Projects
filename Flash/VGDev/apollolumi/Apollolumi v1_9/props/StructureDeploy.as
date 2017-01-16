package props
{
	import props.Floater;
	import flash.display.MovieClip;
	
	public class StructureDeploy extends Floater
	{
		private var type:String;
		private var dummy:MovieClip;
		private var turr:MovieClip;
		
		public function StructureDeploy(_cg:MovieClip, _xP:int, _yP:int, _type:String)
		{
			super(_cg, _xP, _yP, 0, 0, 100);
			type = _type;
			str.gotoAndStop(_type);
			collidable = false;

			dummy = new DeployDummy(cg.strMan, cg, x, y);
			cont.addChild(dummy);
			
			setMapIcon(0xFFFFFF, "deploy");
		}
		
		private function landed():void
		{
			cont.removeChild(dummy);
			dummy.destroy();
			dummy = null;
			
			// -- TODO add structure creation code here
			switch (type)
			{
				case "fdt":
					cg.strMan.addStruct(new ForceDefense(cg.strMan, cg, x, y, 0, 0, 50, 3));
				break;
				case "mt":
					cg.strMan.addStruct(new MissileTurret(cg.strMan, cg, x, y, 0, 0, 60, 2.5));
				break;
				case "pdl":
					cg.strMan.addStruct(new PointDefenseLaser(cg.strMan, cg, x, y, 0, 0, 50, 3));
				break;
				case "int":
					cg.strMan.addStruct(new Interceptor(cg.strMan, cg, x, y, 0, 0, 60, 3));
				break;
				case "ebt":
					turr = new BladeTurret(cg.strMan, cg, x, y, 0, 0, 60, 2);
					cg.strMan.addStruct(turr);
				break;
				case "amc":
					cg.strMan.addStruct(new AutoCollector(cg.strMan, cg, x, y, 0, 0, 35, 3));
				break;
				case "rail":
					cg.strMan.addStruct(new Railgun(cg.strMan, cg, x, y, 0, 0, 70, 3));
				break;
			}

			visible = false;
			destroy();
		}
		
		override public function collide(f:Floater):void
		{
			return;
		}

		override public function destroy():void
		{
			if (type == "ebt")
				turr.fx.visible = true;
			
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);
		}
	}
}