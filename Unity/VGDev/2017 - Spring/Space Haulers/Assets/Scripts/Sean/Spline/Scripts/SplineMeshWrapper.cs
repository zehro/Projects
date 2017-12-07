using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SplineMeshWrapper is a derivative type of SplineWrapper. It creates a mesh around the
// generated spline (with some additional information like sides, radius, and UV behaviour),
// and can optionally give renderers, collision, and materials to each of its children.

namespace YeggQuest.NS_Spline
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SplineMeshWrapper : SplineWrapper
    {
        [Range(16, 42)]
        public int sides = 16;
        [Range(5f, 50f)]
        public float radius = 0.5f;
        [Range(1, 3)]
        public int textureWrap = 2;
        [Range(0.5f, 2f)]
        public float textureLength = 1;
        public bool solid = false;

        public Mesh nodeMesh;
        public Mesh nodeCollisionMesh;
        public Material nodeMaterial;
        public PhysicMaterial nodeCollisionMaterial;

        private bool hasGeneratedOnce = false;

        public override void Setup()
        {
            base.Setup();

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.GetComponent<MeshFilter>() == null)
                    child.AddComponent<MeshFilter>();
                child.GetComponent<MeshFilter>().sharedMesh = nodeMesh;

                if (child.GetComponent<MeshCollider>() == null)
                    child.AddComponent<MeshCollider>();
                child.GetComponent<MeshCollider>().enabled = solid;
                child.GetComponent<MeshCollider>().sharedMesh = nodeCollisionMesh;
                child.GetComponent<MeshCollider>().sharedMaterial = nodeCollisionMaterial;

                if (child.GetComponent<MeshRenderer>() == null)
                    child.AddComponent<MeshRenderer>();
                child.GetComponent<MeshRenderer>().sharedMaterial = nodeMaterial;
            }

            GetComponent<MeshFilter>().sharedMesh = SplineMeshUtil.GenerateMesh(spline, transform, sides, radius, textureWrap, textureLength);
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
            GetComponent<MeshCollider>().sharedMaterial = nodeCollisionMaterial;
            GetComponent<MeshCollider>().enabled = solid;


            hasGeneratedOnce = true;
        }

        public override void Teardown()
        {
            base.Teardown();

            if (hasGeneratedOnce)
            {
                DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
            }
        }
    }
}