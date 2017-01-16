using UnityEngine;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Information about what an arrow hit to use or pass along as needed.
    /// </summary>
    public class CollisionInfo : MonoBehaviour
    {
        /// <summary>
        /// Position that the arrow hit
        /// </summary>
        private Vector3 hitPosition;
        /// <summary>
        /// Rotation of the area hit
        /// </summary>
        private Quaternion hitRotation;
        /// <summary>
        /// Whether or not the arrow is a trigger
        /// </summary>
        private bool isTrigger;

        #region C# Properties
        /// <summary>
        /// Position that the arrow hit
        /// </summary>
        public Vector3 HitPosition
        {
            get { return hitPosition; }
            set { hitPosition = value; }
        }
        /// <summary>
        /// Rotation of the area hit
        /// </summary>
        public Quaternion HitRotation
        {
            get { return hitRotation; }
            set { hitRotation = value; }
        }
        /// <summary>
        /// Whether or not the arrow is a trigger
        /// </summary>
        public bool IsTrigger
        {
            get { return isTrigger; }
            set { isTrigger = value; }
        }
        #endregion
    }
}
