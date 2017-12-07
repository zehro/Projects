using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AsteroidSpawner))]
public class AsteroidSpawnerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AsteroidSpawner myScript = (AsteroidSpawner)target;
        if (GUILayout.Button("Regenerate"))
        {
            myScript.destroyAsteroids();
            myScript.spawnAsteroids();
        }
    }
}
