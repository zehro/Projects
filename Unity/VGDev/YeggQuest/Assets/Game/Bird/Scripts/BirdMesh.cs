using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The BirdMesh is responsible for copying the positions and orientations of the procedurally-controlled
// BirdAnimator skeleton bones into the skeleton actually used by the bird's skinned mesh prefab. Pretty
// boilerplate-y and simple, but something has to do it.

namespace YeggQuest.NS_Bird
{
    public class BirdMesh : MonoBehaviour
    {
        public Bird bird;
        public SkinnedMeshRenderer skinnedMeshRenderer;

        public Transform[] skeletonFrom;
        public Transform[] skeletonTo;
        public Vector3[] rotations;
        public Vector3[] scaleRotations;

        void LateUpdate()
        {
            for (int i = 0; i < skeletonFrom.Length; i++)
            {
                skeletonTo[i].position = skeletonFrom[i].position;
                skeletonTo[i].rotation = skeletonFrom[i].rotation * Quaternion.Euler(rotations[i]);
                skeletonTo[i].localScale = Quaternion.Euler(scaleRotations[i]) * skeletonFrom[i].localScale;
            }
        }
    }
}