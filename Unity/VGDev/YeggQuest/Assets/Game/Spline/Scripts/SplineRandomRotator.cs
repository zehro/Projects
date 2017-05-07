using UnityEngine;

namespace YeggQuest.NS_Spline {

    // Randomly rotates a spline by a small amount at the start of the level.

    [RequireComponent(typeof(SplineNode))]
    [DisallowMultipleComponent]
    public class SplineRandomRotator : MonoBehaviour
    {
        // The amount of variance (positive and negative) that the spline can be rotated by.
        [SerializeField]
        [Tooltip("The amount of variance (positive and negative) that the spline can be rotated by.")]

        private float rotationVariance;

        // Randomly rotates the spline node

        private void Start()
        {
            rotationVariance = Mathf.Abs(rotationVariance);
            SplineNode node = GetComponent<SplineNode>();
            Vector3 baseOrientation = node.worldOrientation;
            baseOrientation.x += GetRandomVariance();
            baseOrientation.y += GetRandomVariance();
            baseOrientation.z += GetRandomVariance();
            node.worldOrientation = baseOrientation;
        }

        // Gets a random variance to rotate around an axis by.

        private float GetRandomVariance()
        {
            return Random.Range(-rotationVariance, rotationVariance);
        }
    }
}