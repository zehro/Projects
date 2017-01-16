namespace Assets.Scripts.Player.AI
{
	/// <summary> A policy for the AI to make decisions with. </summary>
	public interface IPolicy
	{
		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		void ChooseAction(AIController controller);
	}
}

