using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The five BirdAnimatorNodes control the visual behavior of five (out of six) of the bird's physics-controlled
// "blobby" nodes - his left, his right, his front, his back, and his butt. (His head is controlled by BirdAnimatorHead.)
// All this class is responsible for is matching the position of the physics node when the bird is in ball form,
// and returning to a default local position and orientation when it is not.

namespace YeggQuest.NS_Bird
{
    public class BirdAnimatorNode : MonoBehaviour
    {
        public BirdAnimator animator;   // the bird animator
        public Rigidbody physicsNode;   // the rigidbody physics node that this node is paired to

        private Vector3 restingPos;     // the default local position this node should return to the bird animator is walking

        void Start()
        {
            restingPos = transform.localPosition;
        }

        void LateUpdate()
        {
            // Every frame, the node sets its position to the linked rigidbody (but also jiggles with the root scale)
            // It then lerps into its default resting position, based on how much the BirdAnimator isn't balled up

            float t = Mathf.SmoothStep(0, 1, 1 - animator.GetBallAmount());
            transform.position = Yutil.ApplyScale(animator.transform, physicsNode.transform.position);
            transform.localPosition = Vector3.Lerp(transform.localPosition, restingPos, t);
        }
    }
}