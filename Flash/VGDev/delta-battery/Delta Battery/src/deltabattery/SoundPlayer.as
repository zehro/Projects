package deltabattery 
{
	import flash.media.Sound;
	import flash.media.SoundChannel;
	/**
	 * ...
	 * @author Alexander Huynh
	 */
	public class SoundPlayer 
	{
		[Embed(source = "../sound/db_bgm_1.mp3")]
		private static var bgm_1:Class;
		[Embed(source="../sound/db_bgm_2.mp3")]
		private static var bgm_2:Class;
		[Embed(source="../sound/db_explode_0.mp3")]
		private static var sfx_explode0:Class;
		[Embed(source="../sound/db_explode_1.mp3")]
		private static var sfx_explode1:Class;
		[Embed(source="../sound/db_explode_2.mp3")]
		private static var sfx_explode2:Class;
		[Embed(source="../sound/db_explode_3.mp3")]
		private static var sfx_explode3:Class;
		[Embed(source="../sound/db_launch_big.mp3")]
		private static var sfx_launch_big:Class;
		[Embed(source="../sound/db_launch_chain.mp3")]
		private static var sfx_launch_chain:Class;
		[Embed(source="../sound/db_launch_fast.mp3")]
		private static var sfx_launch_fast:Class;
		[Embed(source="../sound/db_launch_flak.mp3")]
		private static var sfx_launch_flak:Class;
		[Embed(source="../sound/db_launch_laser.mp3")]
		private static var sfx_launch_laser:Class;
		[Embed(source="../sound/db_launch_os_standard.mp3")]
		private static var sfx_launch_os_standard:Class;
		[Embed(source="../sound/db_launch_standard.mp3")]
		private static var sfx_launch_standard:Class;
		[Embed(source="../sound/db_menu_blip.mp3")]
		private static var sfx_menu_blip:Class;
		[Embed(source="../sound/db_menu_blip_over.mp3")]
		private static var sfx_menu_blip_over:Class;
		[Embed(source="../sound/db_no_ammo.mp3")]
		private static var sfx_no_ammo:Class;
		[Embed(source="../sound/db_purchase.mp3")]
		private static var sfx_purchase:Class;
		[Embed(source="../sound/db_voice.mp3")]
		private static var sfx_launch_voice:Class;

		public static var bgm:SoundChannel;
		
		public function SoundPlayer() 
		{
		}
		
		public static function play(s:String):void
		{
			var sound:Sound;
			switch (s)
			{
				case "db_explode_0":
					((new sfx_explode0) as Sound).play();
				break;
				case "db_explode_1":
					((new sfx_explode1) as Sound).play();
				break;
				case "db_explode_2":
					((new sfx_explode2) as Sound).play();
				break;
				case "db_explode_3":
					((new sfx_explode3) as Sound).play();
				break;
				case "sfx_launch_big":
					((new sfx_launch_big) as Sound).play();
				break;
				case "sfx_launch_chain":
					((new sfx_launch_chain) as Sound).play();
				break;
				case "sfx_launch_fast":
					((new sfx_launch_fast) as Sound).play();
				break;
				case "sfx_launch_flak":
					((new sfx_launch_flak) as Sound).play();
				break;
				case "sfx_launch_laser":
					((new sfx_launch_laser) as Sound).play();
				break;
				case "sfx_launch_os_standard":
					((new sfx_launch_os_standard) as Sound).play();
				break;
				case "sfx_launch_standard":
					((new sfx_launch_standard) as Sound).play();
				break;
				case "sfx_menu_blip":
					((new sfx_menu_blip) as Sound).play();
				break;
				case "sfx_menu_blip_over":
					((new sfx_menu_blip_over) as Sound).play();
				break;
				case "sfx_no_ammo":
					((new sfx_no_ammo) as Sound).play();
				break;
				case "sfx_purchase":
					((new sfx_purchase) as Sound).play();
				break;
				case "sfx_launch_voice":
					((new sfx_launch_voice) as Sound).play();
				break;
			}
		}
		
		public static function playBGM(isMenu:Boolean):void
		{
			if (bgm)
				bgm.stop();
			var snd:Sound = (isMenu ? new bgm_1 : new bgm_2);
			bgm = snd.play(0, 9999);
		}
		
		public static function isBGMplaying():Boolean
		{
			return bgm != null;
		}
		
		public static function stopBGM():void
		{
			if (bgm)
			{
				bgm.stop();
				bgm = null;
			}
		}
	}
}