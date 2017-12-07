package cobaltric
{
	import flash.display.MovieClip;
	import flash.media.Sound;
	import flash.media.SoundChannel;
	import flash.media.SoundMixer;
	import flash.media.SoundTransform;
	import flash.utils.Timer;
	import flash.events.TimerEvent;

	public class SoundManager
	{
		public static var eng:MovieClip;									// a reference to the Engine
		
		public static var sndMap:Object = new Object();					// maps a String to a Sound
		
		public static var bgm:SoundChannel;								// the single background music
		public static var sndTF:SoundTransform = new SoundTransform();		// for fading the BGM out
		
		// fading helpers
		public static var timer:Timer;
		public static var fade:int = 0;
		public static var maxFade:int = 1;
		
		// holds any sounds that are set to loop
		public static var sndLoop:Object = new Object();

		public function SoundManager(_eng:MovieClip)
		{
			eng = _eng;
			// -- add sound definitions here		
			sndMap["sfx_alert"] = new SFX_AlertWheeer();
			sndMap["bgm_dr"] = new BGM_DR();
		}
		
		// play the given sound effect numTimes, or once if not provided
		public static function playSound(s:String, numTimes:int = -1):void
		{
			if (numTimes != -1)
			{
				startLoop(s, numTimes);
				return;
			}
			sndMap[s].play();
		}
		
		// play the specified background music at the given volume (or 100% if not provided)
		public static function playBGM(s:String, vol:Number = 1):void
		{
			stopBGM();
			bgm = sndMap[s].play(0, int.MAX_VALUE);				// loop forever
			if (vol != 1)
				bgm.soundTransform = new SoundTransform(vol);
		}
		
		// stop the background music
		public static function stopBGM():void
		{
			if (bgm)
			{
				bgm.stop();
				fade = 0;
				if (timer)
				{
					if (timer.hasEventListener(TimerEvent.TIMER))
						timer.removeEventListener(TimerEvent.TIMER, tick);
					timer.stop();
				}
			}
			bgm = null;
		}
		
		// fade the background music out, duration in frames
		public static function fadeBGM(duration:int = 30):void
		{
			fade = maxFade = duration;
			sndTF.volume = 1;
			timer = new Timer(33);
			timer.addEventListener(TimerEvent.TIMER, tick);
			timer.start();
		}
		
		// fade helper
		private static function tick(e:TimerEvent):void
		{
			sndTF.volume = fade / maxFade;
			if (bgm)
				bgm.soundTransform = sndTF;
			if (--fade <= 0)
				stopBGM();
		}
		
		public static function startLoop(s:String, numTimes:int = int.MAX_VALUE):SoundChannel
		{
			if (!sndLoop[s])
				sndLoop[s] = new SoundChannel();
			sndLoop[s] = sndMap[s].play(0, numTimes);
			return sndLoop[s];
		}
		
		public static function stopLoop(s:String):void
		{
			if (!sndLoop[s]) return;
			sndLoop[s].stop();
			sndLoop[s] = null;
		}
		
		public static function shutUp():void
		{
			stopBGM();
			SoundMixer.stopAll();
			sndLoop = new Object();
		}
	}
}
