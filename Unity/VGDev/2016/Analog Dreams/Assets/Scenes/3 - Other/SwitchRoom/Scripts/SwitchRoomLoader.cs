using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class SwitchRoomLoader : MonoBehaviour
{
    public int index;

    void Start()
    {
        GameController.SWITCH_INDEX = index;
        SceneManager.LoadScene("SwitchRoom");
    }
}