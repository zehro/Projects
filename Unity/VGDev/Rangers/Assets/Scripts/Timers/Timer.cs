using UnityEngine;
using System;

namespace Assets.Scripts.Timers
{
    /// <summary>
    /// Timer class that can fire events
    /// </summary>
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// Delegates and events to fire once timer times out
        /// </summary>
        /// <param name="t">Timer that has timer out</param>
        public delegate void TimerEvent(Timer t);
        /// <summary>
        /// The timeout event
        /// </summary>
        public event TimerEvent TimeOut;

        // Interval is how long the timer will count for
        // Timer is the current time being counted
        protected float interval, timer = 0f;
        // Whether or not the timer is running
        protected bool on = false;
        // ID for identifying timers after they have started
        protected string id = "";

        /// <summary>
        /// Initialize and start the timer
        /// </summary>
        /// <param name="interval">How long the timer will run for</param>
        /// <param name="id">ID of the timer</param>
        public virtual void Initialize(float interval, string id)
        {
            this.interval = interval;
            timer = 0f;
            this.id = id;
            on = true;
        }

        void Update()
        {
            UpdateTimer();
        }

        /// <summary>
        /// How the timer counts
        /// </summary>
        protected virtual void UpdateTimer()
        {
            if (on)
            {
                timer += Time.deltaTime;
                if (timer >= interval)
                {
                    timer = interval;
                    FireTimerEvent();
                    Destroy(this);
                }
            }
        }

        /// <summary>
        /// Resetting the timer, also turns it back on
        /// </summary>
        public virtual void Reset()
        {
            Initialize(interval, id);
        }

        /// <summary>
        /// Fires the timeout delegate
        /// </summary>
        protected virtual void FireTimerEvent()
        {
            // If the event has subscribers, fire it
            if (TimeOut != null)
            {
                TimeOut(this);
            }
        }

        #region C# Properties
        /// <summary>
        /// Whether or not the timer is enabled
        /// </summary>
        public bool Enabled
        {
            get { return on; }
            set { on = value; }
        }
        /// <summary>
        /// ID of the timer
        /// </summary>
        public string ID
        {
            get { return id; }
        }
        /// <summary>
        /// Current time of the timer
        /// </summary>
        public float CurrentTime
        {
            get { return timer; }
        }
        #endregion

        /// <summary>
        /// Whether or not the timer is on and running
        /// </summary>
        public bool On
        {
            get { return on; }
            set { on = value; }
        }
        /// <summary>
        /// String representation of the timer
        /// </summary>
        /// <returns>The time as a string</returns>
        public override string ToString()
        {
            return timer.ToString();
        }

        /// <summary>
        /// String representation of the timer that is formatted
        /// </summary>
        /// <param name="places">Number of places to round to</param>
        /// <returns>A formatted string representation of the current time</returns>
        public string ToString(int places = 2)
        {
            return Math.Round(timer, places).ToString();
        }
    }
}
