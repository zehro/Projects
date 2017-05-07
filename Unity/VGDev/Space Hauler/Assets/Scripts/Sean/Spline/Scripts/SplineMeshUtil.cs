using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A static helper class that can generate a mesh from a spline, given additional information
// about how many sides it should have, its radius, and the behaviour of its UV coordinates.

namespace YeggQuest.NS_Spline
{
    public static class SplineMeshUtil
    {
        // Generates a procedural mesh and returns it.

        // spline           the Spline this mesh is going to be based on
        // helper           a helper transform object which is used to generate the mesh

        // sides            the number of sides that make up a segment
        // radius           the circular size of each segment
        // textureWrap      how many times the texture repeats around a segment
        // textureLength    how long the texture loops on the mesh in world space

        public static Mesh GenerateMesh(Spline spline, Transform helper, int sides, float radius, float textureWrap, float textureLength)
        {
            Vector3 helperPos = helper.position;
            Quaternion helperRot = helper.rotation;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            for (int i = 0; i < spline.sectionCount; i++)
            {
                for (int j = 1; j < spline.segmentCount - 1; j++)
                {
                    float angle = 360f / sides;
                    helper.position = spline.vertices[i, j] + spline.normals[i, j] * radius;

                    for (int k = 0; k <= sides; k++)
                    {
                        verts.Add(helper.position);
                        norms.Add((helper.position - spline.vertices[i, j]).normalized);
                        float u = textureWrap * k / sides;
                        float v = spline.lengths[i, j];
                        for (int l = 0; l < i; l++)
                            v += spline.lengths[l, spline.segmentCount - 1];
                        uvs.Add(new Vector2(u, v / textureLength));

                        helper.RotateAround(spline.vertices[i, j], spline.tangents[i, j], angle);
                    }
                }

                int trisPerSegment = sides + 1;
                int trisPerSection = (spline.segmentCount - 2) * trisPerSegment;

                int section = i * trisPerSection;

                for (int j = 0; j < spline.segmentCount - 3; j++)
                {
                    int segment1 = j * trisPerSegment;
                    int segment2 = (j + 1) * trisPerSegment;

                    for (int k = 0; k < sides; k++)
                    {
                        tris.Add(section + segment1 + k);
                        tris.Add(section + segment1 + k + 1);
                        tris.Add(section + segment2 + k);

                        tris.Add(section + segment1 + k + 1);
                        tris.Add(section + segment2 + k + 1);
                        tris.Add(section + segment2 + k);
                    }
                }
            }

            helper.position = helperPos;
            helper.rotation = helperRot;

            // Create mesh from data and return it ================================================================================

            Mesh mesh = new Mesh();
            mesh.name = "SplineMesh (GENERATED)";
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            return mesh;
        }
    }
}