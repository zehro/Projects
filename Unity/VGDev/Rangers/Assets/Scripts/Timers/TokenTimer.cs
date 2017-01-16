using System;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Timers
{
    /// <summary>
    /// Functions like a normal timer with the added benefit of having the IDs being related to tokens
    /// </summary>
    public class TokenTimer : Timer
    {
        /// <summary>
        /// Delegates and events to fire once timer times out
        /// </summary>
        /// <param name="t">Timer that has timer out</param>
        new public delegate void TimerEvent(TokenTimer t);
        /// <summary>
        /// The timeout event
        /// </summary>
        new public event TimerEvent TimeOut;

        // Type of the token the timer is for
        protected Enums.Arrows tokenType;

        /// <summary>
        /// Constant interval for tokens to remove them for player
        /// </summary>
        public static float TOKEN_INTERVAL = 5f;

        /// <summary>
        ///  Overriding the initialize method. Use the token type.ToString() as id
        /// </summary>
        /// <param name="interval">How long the timer will run for</param>
        /// <param name="id">ID of the timer</param>
        public override void Initialize(float interval, string id)
        {
            base.Initialize(interval, id);
            // Get the type of token based on the ID
            tokenType = (Enums.Arrows)Enum.Parse(typeof(Enums.Arrows), id);
        }

        /// <summary>
        /// Fires the timeout delegate
        /// </summary>
        protected override void FireTimerEvent()
        {
            // If the event has subscribers, fire it
            if (TimeOut != null) TimeOut(this);
        }
        #region C# Properties
        /// <summary>
        /// Type of token associated with the timer
        /// </summary>
        public Enums.Arrows TokenType
        {
            get { return tokenType; }
        }
        #endregion
    }
}
