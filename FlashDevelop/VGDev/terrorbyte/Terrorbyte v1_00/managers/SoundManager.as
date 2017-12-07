package managers
{
	import flash.display.MovieClip;
	import flash.media.Sound;
	import flash.media.SoundChannel;
	import flash.media.SoundMixer;
	import flash.media.SoundTransform;
	import flash.events.Event;

	public class SoundManager
	{
		private var eng:MovieClip;
		public var sndMap:Object = new Object();
		public var bgm:SoundChannel;
		public var sndTF:SoundTransform = new SoundTransform();
		
		private var whichBGM:Boolean;

		public function SoundManager(_eng:MovieClip)
		{
			eng = _eng;
			// -- add sound definitions here					
			sndMap["BGM_main"] = new BGM_main();
			sndMap["BGM_CogEmpt"] = new BGM_CogEmpt();
			
			sndMap["SFX_menuBeep"] = new SFX_menuBeep();
			sndMap["SFX_beepSingle"] = new SFX_beepSingle();
			sndMap["SFX_beepLow2"] = new SFX_beepLow2();
			sndMap["SFX_explosionPlane"] = new SFX_explosionPlane();
			sndMap["SFX_explosionMissile"] = new SFX_explosionMissile();
			sndMap["SFX_missileLaunch"] = new SFX_missileLaunch();
			sndMap["SFX_nodeClicked"] = new SFX_nodeClicked();
			sndMap["SFX_nodeHacked"] = new SFX_nodeHacked();
			sndMap["SFX_nodeSent"] = new SFX_nodeSent();
			sndMap["SFX_nodeTransferred"] = new SFX_nodeTransferred();
			sndMap["SFX_nodeOnline"] = new SFX_nodeOnline();
			sndMap["SFX_nodeOffline"] = new SFX_nodeOffline();
			sndMap["SFX_laser"] = new SFX_laser();
			sndMap["sfx_missileWarning"] = new SFX_MissileWarning();
			sndMap["SFX_hacking"] = new SFX_hacking();
			sndMap["SFX_complete"] = new SFX_complete();
			
			whichBGM = Math.random() > .5;
			playBGM(null);
		}
		
		private function playBGM(e:Event):void
		{
			if (whichBGM)
				bgm = sndMap["BGM_main"].play(0, 1);
			else
				bgm = sndMap["BGM_CogEmpt"].play(0, 1);
			if (!bgm.hasEventListener(Event.SOUND_COMPLETE))
				bgm.addEventListener(Event.SOUND_COMPLETE, playBGM);
			whichBGM = !whichBGM;
		}
		
		public function stopBGM():void
		{
			if (bgm)
				bgm.stop();
		}
		
		public function playSound(s:String):void
		{
			sndMap[s].play();
		}
		
		public function shutUp():void
		{
			SoundMixer.stopAll();
		}
	}
}
