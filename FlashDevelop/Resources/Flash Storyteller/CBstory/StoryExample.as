package CBstory
{
	import cobaltric.Storyteller;
	import cobaltric.Dialogue;
	import flash.display.MovieClip;

	public class StoryExample extends Story
	{
		public function StoryExample(_st:MovieClip)
		{
			super(_st);
		}

		override protected function init():void
		{
			// fireplace
			st.addLine(new Dialogue("You have the goods, yes?", "Ruben", "fireplace", "+bgm_dr"));
			st.addLine(new Dialogue("Yeh, got 'em right here.", "Kara"));
			st.addLine(new Dialogue("Excellent. Now get out of my sight.", "Ruben", "", "", [20, 40]));
			
			// woods
			st.addLine(new Dialogue("Did I do the right thing, Robin?", "Kara", "woods"));
			st.addLine(new Dialogue("I don't know, Kara. Only time will tell.", "Robin"));
			
			// animate
			st.addLine(new Dialogue("Hey, look. You can animate things.", "Alex", "animate"));
			st.addLine(new Dialogue("AND PLAY SOUNDS!?", "Alex", "", "sfx_alert"));
			st.addLine(new Dialogue("You can loop them, too.", "Alex", "", "+sfx_alert"));
			st.addLine(new Dialogue("It can be incredibly annoying, I bet.", "Alex"));
			st.addLine(new Dialogue("I'll turn that off.", "Alex", "", "-sfx_alert"));
			st.addLine(new Dialogue("I'll stop the BGM, too.", "Alex", "", "-bgm"));
			
			// error
			st.addLine(new Dialogue("This should error out because *SOMEBODY* didn't set up their images correctly!", "Alex", "doesntExist"));
		}
	}
}

/*
Attributions

"fireplace"		http://pmdunity.tumblr.com/post/87098807132/
"woods"			http://pmdunity.tumblr.com/post/87098891667/

"bgm_dr"		https://www.youtube.com/watch?v=mkFgN-brawI
*/