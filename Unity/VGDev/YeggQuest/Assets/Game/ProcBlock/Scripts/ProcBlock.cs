using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: comments

namespace YeggQuest.NS_ProcBlock
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ProcBlock : MonoBehaviour
    {
        public Mesh sourceMesh;
        public Transform sourceScale;
        private Vector3 scale;

        private bool hasGeneratedOnce = false;

        void OnEnable()
        {
            if (sourceScale == null)
                sourceScale = transform;
            if (sourceScale != null)
                scale = sourceScale.localScale;

            GetComponent<MeshFilter>().sharedMesh = new Mesh();
            GenerateMesh();
        }

        void OnDisable()
        {
            DestroyMesh();
        }

        void GenerateMesh()
        {
            DestroyMesh();

            if (sourceMesh == null || scale.x == 0 || scale.y == 0 || scale.z == 0)
                return;

            Vector3[] oldVerts = sourceMesh.vertices;
            Vector3[] newVerts = new Vector3[sourceMesh.vertexCount];
            for (int i = 0; i < sourceMesh.vertexCount; i++)
            {
                float x = ((Mathf.Abs(oldVerts[i].x) - 0.5f) / scale.x + 0.5f) * Mathf.Sign(oldVerts[i].x);
                float y = ((Mathf.Abs(oldVerts[i].y) - 0.5f) / scale.y + 0.5f) * Mathf.Sign(oldVerts[i].y);
                float z = ((Mathf.Abs(oldVerts[i].z) - 0.5f) / scale.z + 0.5f) * Mathf.Sign(oldVerts[i].z);
                newVerts[i].Set(x, y, z);
            }

            Mesh mesh = new Mesh();
            mesh.name = "ProcBlock (GENERATED)";
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.SetVertices(new List<Vector3>(newVerts));
            mesh.SetNormals(new List<Vector3>(sourceMesh.normals));
            mesh.SetUVs(0, new List<Vector2>(sourceMesh.uv));
            mesh.SetTriangles(sourceMesh.triangles, 0);
            mesh.SetColors(new List<Color>(sourceMesh.colors));
            GetComponent<MeshFilter>().sharedMesh = mesh;

            hasGeneratedOnce = true;
        }

        void DestroyMesh()
        {
            if (hasGeneratedOnce)
                DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
        }

        // ======================================================================================================================== LISTENERS

        void Update()
        {
            if (Application.isPlaying)
                return;

            if (sourceScale != null && scale != sourceScale.localScale)
            {
                scale = sourceScale.localScale;
                GenerateMesh();
            }
        }

        void OnValidate()
        {
            if (Application.isPlaying)
                return;

            GenerateMesh();
        }
    }
}