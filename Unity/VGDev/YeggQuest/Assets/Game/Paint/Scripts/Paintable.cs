using UnityEngine;

// The abstract Paintable class. For a script to be Paintable, it must implement
// the Paint() message, which receives a PaintRequest and should place paint
// on its object accordingly.

namespace YeggQuest.NS_Paint
{
    public abstract class Paintable : MonoBehaviour
    {
        // A helper method for all Paintable objects which creates the dual-material
        // setup needed for painting. While each Paintable implementation uses its
        // own shader, they are all applied as procedural second materials, which
        // this script sets up on the given renderer and then returns.

        protected Material InitializeMaterial(Renderer renderer, string shaderName)
        {
            Material mat;

            int len = renderer.sharedMaterials.Length;
            Material[] rendMats = new Material[len + 1];
            for (int i = 0; i < len; i++)
                rendMats[i] = renderer.sharedMaterials[i];
            rendMats[len] = mat = new Material(Shader.Find("Paint/" + shaderName));
            renderer.sharedMaterials = rendMats;

            mat.hideFlags = HideFlags.HideAndDontSave;
            mat.name = shaderName + " (" + gameObject.GetInstanceID() + ")";

            return mat;
        }

        // The function which globally loads the crucial two paint textures:
        // the cutoff and the normal map. Called by the Game at initialization.

        public static void LoadPaintTextures()
        {
            Shader.SetGlobalTexture("_PaintCutoffTex", Resources.Load<Texture>("Paint/PaintCutoff"));
            Shader.SetGlobalTexture("_PaintNormalTex", Resources.Load<Texture>("Paint/PaintNormal"));
        }

        public abstract bool Paint(PaintRequest request);
    }
}