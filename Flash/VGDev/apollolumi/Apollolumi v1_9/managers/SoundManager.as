package managers
{
	import flash.display.MovieClip;
	import flash.media.Sound;

	public class SoundManager
	{
		private var cg:MovieClip;
		public var sndMap:Object = new Object();

		public function SoundManager(_par:MovieClip)
		{
			cg = _par;
			// -- TODO add sound definitions here
			sndMap["ast1"] = new SFX_astImpact1();
			sndMap["ballistic1"] = new SFX_Ballistic1();
			sndMap["deploy"] = new SFX_deploy();
			sndMap["explode0"] = new SFX_explode();
			sndMap["explode1"] = new SFX_explode1();
			sndMap["explode2"] = new SFX_explode2();
			sndMap["explode3"] = new SFX_explode3();
			sndMap["impact1"] = new SFX_impact1();
			sndMap["laser1"] = new SFX_Laser1();
			sndMap["laser2"] = new SFX_Laser2();
			sndMap["launch1"] = new SFX_Launch1();
			sndMap["launch2"] = new SFX_Launch2();
			sndMap["launch3"] = new SFX_Launch3();
			sndMap["minPickup"] = new SFX_MineralPickup();
			sndMap["pdl"] = new SFX_PDL();
			sndMap["playerLaser"] = new SFX_playerLaser();
			sndMap["playerTractor"] = new SFX_playerTractor();
		}
		
		public function playSound(s:String, unique:Boolean = false):void
		{
			sndMap[s].play();
			// -- TODO check for non-repeating sounds
		}
		
		public function destroy():void
		{
			// SoundMixer.stopAll();
		}
	}
}
