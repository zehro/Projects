#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
public class ConnectLogic : MonoBehaviour
{
    [MenuItem("Analog Dreams/Connect Logic")]
    static void connectLogic()
    {
        if (Selection.transforms.Length == 2)
        {
            LogicOutput output = Selection.transforms[0].GetComponent<LogicOutput>();
            LogicInput input = Selection.transforms[0].GetComponent<LogicInput>();

            if (output == null)
                output = Selection.transforms[1].GetComponent<LogicOutput>();
            if (input == null)
                input = Selection.transforms[1].GetComponent<LogicInput>();

            if (output != null && input != null)
            {
                GameObject cable = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/BaseGame/LogicCable.prefab", typeof(GameObject));
                GameObject newCable = (GameObject) PrefabUtility.InstantiatePrefab(cable);
                Undo.RegisterCreatedObjectUndo(newCable, "Created new cable");

                LogicCable logicCable = newCable.transform.GetComponent<LogicCable>();
                logicCable.start = output;
                logicCable.end = input;
                return;
            }
        }

        Debug.LogError("<color=red>Connect Logic:</color> Please select one logic output and one logic input.");
    }
}

#endif