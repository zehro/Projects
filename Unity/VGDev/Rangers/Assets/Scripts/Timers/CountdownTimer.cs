using UnityEngine;

namespace Assets.Scripts.Timers
{
    /// <summary>
    /// Timer used for counting down
    /// </summary>
    public class CountdownTimer : Timer
    {
        /// <summary>
        /// Delegates and events to fire once timer times out
        /// </summary>
        /// <param name="t">Timer that has timer out</param>
        new public delegate void TimerEvent(CountdownTimer t);
        /// <summary>
        /// The timeout event
        /// </summary>
        new public event TimerEvent TimeOut;

        /// <summary>
        /// Initialze and start the timer
        /// </summary>
        /// <param name="interval">How long the timer will run for</param>
        /// <param name="id">ID of the timer</param>
        public override void Initialize(float interval, string id)
        {
            base.Initialize(interval, id);
            timer = interval;
        }

        /// <summary>
        /// Updating the timer to count down
        /// </summary>
        protected override void UpdateTimer()
        {
            if (on)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = 0;
                    // If the event has subscribers, fire it
                    FireTimerEvent();
                    Destroy(this);
                }
            }
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
        /// Creates a countdown timer on a GameObject.
        /// </summary>
        /// <param name="gameObject">The game object to add the timer to.</param>
        /// <param name="interval">How long the timer will run for.</param>
        /// <param name="id">ID of the timer.</param>
        /// <param name="timerEvent">The function to call when the timer runs out.</param>
		public static CountdownTimer CreateTimer(GameObject gameObject, float interval, string id, TimerEvent timerEvent)
        {
            CountdownTimer timer = gameObject.AddComponent<CountdownTimer>();
            timer.Initialize(interval, id);
            timer.TimeOut += new CountdownTimer.TimerEvent(timerEvent);
			return timer;
        }
    }
}
