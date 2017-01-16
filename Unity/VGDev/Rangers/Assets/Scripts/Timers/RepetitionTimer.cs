using UnityEngine;
using Assets.Scripts.Util;

namespace Assets.Scripts.Timers
{
    /// <summary>
    /// Timer that repeats a certain timeout delegate
    /// </summary>
    public class RepetitionTimer : Timer
    {
        /// <summary>
        /// Delegates and events to fire once timer times out
        /// </summary>
        /// <param name="t">Timer that has timer out</param>
        new public delegate void TimerEvent(RepetitionTimer t);
        /// <summary>
        /// The timeout event
        /// </summary>
        new public event TimerEvent TimeOut;
        /// <summary>
        /// The final event once the timer has finished
        /// </summary>
        public event TimerEvent FinalTick;

        // Type of timer running
        private Enums.RepetitionTimerSettings type;
        // How many times the timer will repeat
        private int repeat = 0;

        /// <summary>
        /// Override the initialization for timers
        /// </summary>
        /// <param name="interval">How long the timer will run for</param>
        /// <param name="id">ID of the timer</param>
        public override void Initialize(float interval, string id)
        {
            // Call base
            base.Initialize(interval, id);
            // Repeat will be unused
            repeat = 0;
            // Timer will repeat forever
            type = Enums.RepetitionTimerSettings.Unlimited;
        }

        /// <summary>
        /// Initializing the timer and turning it on
        /// </summary>
        /// <param name="interval">How long the timer will run for</param>
        /// <param name="id">ID of the timer</param>
        /// <param name="repeat">Number of times for the timer to repeat</param>
        public void Initialize(float interval, string id, int repeat)
        {
            // Call base
            base.Initialize(interval, id);
            // Timer will repeat a limited number of times
            type = Enums.RepetitionTimerSettings.Limited;
            // Timer will repeat based on the number that was passed in
            this.repeat = repeat;
        }

        /// <summary>
        /// Updating the timer
        /// </summary>
        protected override void UpdateTimer()
        {
            if (on)
            {
                timer += Time.deltaTime;
                if (timer >= interval)
                {
                    // If the event has subscribers, fire it
                    FireTimerEvent();
                    timer = 0;
                    if (type.Equals(Enums.RepetitionTimerSettings.Limited))
                    {
                        if (--repeat <= 0) FireFinalEvent();
                    }
                }
            }
        }

        /// <summary>
        /// Resetting the timer
        /// </summary>
        public override void Reset()
        {
            // If the timer repeating infinitely
            if (type.Equals(Enums.RepetitionTimerSettings.Unlimited)) Initialize(interval, id);
            // Else it is limited
            else Initialize(interval, id, repeat);
        }

        /// <summary>
        /// Fires the timeout delegate
        /// </summary>
        protected override void FireTimerEvent()
        {
            // If the event has subscribers, fire it
            if (TimeOut != null) TimeOut(this);
        }

        /// <summary>
        /// Fires the final timeout delegate
        /// </summary>
        protected void FireFinalEvent()
        {
            if (FinalTick != null) FinalTick(this);
            Destroy(this);
		}

		/// <summary>
		/// Creates a repetition timer on a GameObject.
		/// </summary>
		/// <param name="gameObject">The game object to add the timer to.</param>
		/// <param name="interval">How long the timer will run for.</param>
		/// <param name="id">ID of the timer.</param>
		/// <param name="timerEvent">The function to call when the timer runs out.</param>
		public static void CreateTimer(GameObject gameObject, float interval, string id, TimerEvent timerEvent)
		{
			RepetitionTimer timer = gameObject.AddComponent<RepetitionTimer>();
			timer.Initialize(interval, id);
			timer.TimeOut += new RepetitionTimer.TimerEvent(timerEvent);
		}
    }
}
