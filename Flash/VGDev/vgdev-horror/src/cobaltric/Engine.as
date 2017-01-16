package cobaltric
{
	import flash.display.MovieClip;
	import flash.events.Event;
	
	/**
	 * Engine.as
	 * 
	 * Primary game loop event firer and state machine.
	 * 
	 * @author Alexander Huynh
	 */
	public class Engine extends MovieClip
	{
		private var gameState:int;				// 0:Intro, 1:Game, 2:Outro
		private var container:ABST_Container;	// the currently active container
		
		public function Engine()
		{
			gameState = 0;
			container = new ContainerIntro();
			addChild(container);

			// center the container
			container.x = 0;//stage.width * .5;
			container.y = 0;//stage.height * .5;

			addEventListener(Event.ENTER_FRAME, step);
		}
		
		public function step(e:Event) : void
		{
			if (!container.step())
				return;
				
			removeChild(container);
			switch (gameState)
			{
				case 0:
					container = new ContainerGame();
					gameState++;
					trace("Intro Container completed!");
				break;
				case 1:
					container = new ContainerOutro();
					gameState++;
					trace("Game Container completed!");
				break;
				case 2:
					gameState = 0;
					container = new ContainerIntro();
					trace("Outro Container completed!");
				break;
			}
			
			addChild(container);
			container.x = 0;// stage.width * .5;
			container.y = 0; //stage.height * .5;
		}
	}
	
}
