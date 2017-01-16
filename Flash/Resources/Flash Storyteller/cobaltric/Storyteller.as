package cobaltric
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.ui.Keyboard;
	import flash.events.KeyboardEvent;
	import flash.media.Sound;
	
	public class Storyteller extends MovieClip
	{
		private var TEXT_SPEED:int = 4;			// characters per frame to show
		
		private var transcript:Array;			// list of Dialogues
		private var diag:Dialogue;				// currently active Dialogue
		private var stringInd:uint;				// index for substring of current dialogue message
		private var stringTgt:String;			// full dialogue
		
		private var eng:MovieClip;				// the containing MovieClip, Engine
		
		private var transitionLock:Boolean;		// if a transition is occurring, true and disables input
		private var transition:int;				// counter for transitions
		private var transitionTarget:int;		// goal count for transitions
		private var transitionState:int;		// state machine var
		
		public function Storyteller()
		{
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);

			// setup Storyteller
			transcript = [];
			stringTgt = "";
			stringInd = 0;
			
			transitionLock = false;
			transition = 0;
			transitionTarget = -1;
			transitionState = 0;
			
			eng = MovieClip(parent);	// reference to Engine
			stage.stageFocusRect = false;
			stage.focus = this;
			
			// setup textbox
			box.adv.addEventListener(MouseEvent.CLICK, boxPressed);
			box.done.visible = false;
	
			// hide and disable interactivity of black transition overlay
			mc_ovr.alpha = 0;
			mc_ovr.buttonMode = mc_ovr.mouseEnabled = mc_ovr.mouseChildren = false;
		
			stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyPress);
		}
		
		// called by a Story to add lines of Dialogue in order
		public function addLine(d:Dialogue):void
		{
			transcript.push(d);
		}
		
		// skipBG is true when called from after a transition
		public function nextSay(skipBG:Boolean = false):void
		{
			diag = transcript.shift();	// grab the next dialogue
			
			stringTgt = diag.msg;		// setup the text
			stringInd = 0;
			box.text = "";
			box.done.visible = false;
			
			if (!skipBG)				// set the BG if not being skipped
				setBackground(diag.img);
			
			box.tName.text = diag.nam;	// set the name
				
			// handle sounds: 
			// +bgm_XXXX		to stop previous BGM and play BGM XXXX
			// -bgm				to stop all BGM
			// XXXX				to play SFX XXXX once
			// +XXXX			to start looping SFX XXXX
			// -XXXX			to stop looping SFX XXXX
			if (diag.snd != "")
			{
				if (diag.snd.substr(0, 4) == "-bgm")
				{
					SoundManager.stopBGM();
				}
				else if (diag.snd.substr(1, 3) == "bgm")
				{
					SoundManager.playBGM(diag.snd.substr(1));
				}
				else
				{
					if (diag.snd.charAt(0) == '+')
						SoundManager.startLoop(diag.snd.substring(1));
					else if (diag.snd.charAt(0) == '-')
						SoundManager.stopLoop(diag.snd.substring(1));
					else
						SoundManager.playSound(diag.snd);
				}
			}
				
			addEventListener(Event.ENTER_FRAME, updateText);
			box.tText.text = "";
		}
		
		private function updateText(e:Event):void
		{
			if (stringTgt == "") return;
			if (box.tText.text.length == stringTgt.length)
			{
				textDone();
				return;
			}
			stringInd += TEXT_SPEED;
			if (stringInd > stringTgt.length - 1)
				stringInd = stringTgt.length;
			box.tText.text = stringTgt.substr(0, stringInd);
		}
		
		private function boxPressed(e:MouseEvent):void
		{
			// do nothing if in the middle of a transition
			if (transitionLock) return;
			
			// check for transition
			if (diag && diag.trans)
			{
				startTransition();
				return;
			}
			
			//sfx_beep.play();
			
			// interrupt text display and show all if currently showing text
			if (box.tText.text.length != stringTgt.length)
				textDone();
			// move on to the next line of dialogue
			else if (transcript.length > 0)
				nextSay();
		}
		
		// called when the current message is fully displayed
		private function textDone():void
		{
			removeEventListener(Event.ENTER_FRAME, updateText);
			box.tText.text = stringTgt;
			box.done.visible = true;
		}
		
		// called when a new message is to be displayed
		private function setTextTgt(t:String):void
		{
			stringTgt = t;
			stringInd = 0;
			box.tText.text = "";
			box.done.visible = false;
		}
		
		// create a fade out/in transition
		private function startTransition():void
		{
			transitionLock = true;
			transition = 0;
			transitionTarget = diag.trans[0];
			transitionState = 0;
			
			addEventListener(Event.ENTER_FRAME, updateTransition);
		}
		
		private function updateTransition(e:Event):void
		{
			if (++transition == transitionTarget)
			{
				transition = 0;
				transitionState++;
				if (transitionState == 1)				// finished from 0 to 100% black
				{
					setBackground(transcript[0].img);
					box.tText.text = "";
					box.tName.text = "";
					box.done.visible = false;
					transitionTarget = diag.trans[1];
				}
				else if (transitionState == 2)			// finished holding
				{
					transitionTarget = diag.trans[0];
				}
				else if (transitionState == 3)			// finished from 100 to 0% black
				{
					transitionLock = false;
					nextSay(true);
				}
			}

			// update overlay alpha
			if (transitionState == 0)
				mc_ovr.alpha = transition / transitionTarget;
			else if (transitionState == 1)
				mc_ovr.alpha = 1;
			else if (transitionState == 2)
				mc_ovr.alpha = 1 - (transition / transitionTarget);
			else if (transitionState == 3)
				mc_ovr.alpha = 0;
		}
		
		// change the background image
		private function setBackground(img:String):void
		{
			if (img != "")
			{
				try
				{
					bg.errText.visible = false;
					bg.gotoAndStop(img);	// set the image
				} catch (e:Error)
				{
					bg.gotoAndStop("default");
					bg.errText.visible = true;
					bg.errText.text = "Error: could not find the given image label:\n\"" + img + "\"";
				}
			}
		}
		
		// same functionality as clicking
		private function onKeyPress(e:KeyboardEvent):void
		{
			if (e.keyCode == Keyboard.SPACE)
				boxPressed(null);
		}
	}
}
