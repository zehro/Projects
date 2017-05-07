#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

namespace YeggQuest
{

    /// <summary>
    /// Updates the nav mesh in a level when the scene is played.
    /// </summary>
    [ExecuteInEditMode]
    class AutoNavMesh : MonoBehaviour
    {

        /// <summary>
        /// Sets up the delegate to modify the nav mesh.
        /// </summary>
        private void Start()
        {
            EditorApplication.playmodeStateChanged = OnPlay;
        }

        /// <summary>
        /// Updates the nav mesh when the scene is played.
        /// </summary>
        private void OnPlay()
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GenerateNavMesh();
            }
        }

        /// <summary>
        /// Updates the nav mesh.
        /// </summary>
        internal static void GenerateNavMesh()
        {
            NavMeshBuilder.BuildNavMesh();
        }
    }

    /// <summary>
    /// Updates the nav mesh in a level when the scene is saved.
    /// </summary>
    class AutoNavMeshSave : UnityEditor.AssetModificationProcessor
    {

        /// <summary>
        /// Called when assets are saved.
        /// </summary>
        /// <param name="paths">The paths of saved assets.</param>
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach(string path in paths)
            {
                if (path.Contains(".unity"))
                {
                    AutoNavMesh.GenerateNavMesh();
                    break;
                }
            }
            return paths;
        }
    }
}
#endif