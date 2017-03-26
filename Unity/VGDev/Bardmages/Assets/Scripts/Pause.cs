using UnityEngine;

class Pause : MonoBehaviour
{
    [SerializeField]
    private GameObject[] canvases;
    [SerializeField]
    private GameObject pauseMenu;

    private bool isPaused;
    private float timeScale;
    private bool[] wasOn;

    void Start()
    {
        isPaused = false;
        timeScale = Time.timeScale;
        wasOn = new bool[canvases.Length];
    }

    void Update()
    {
        if (!Assets.Scripts.Data.Data.Instance.CanPause)
            return;
        if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, PlayerID.One) || 
            ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, PlayerID.Two) || 
            ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, PlayerID.Three) || 
            ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, PlayerID.Four) ||
			Input.GetKeyDown(KeyCode.Return))
        {
            isPaused = !isPaused;
            Debug.Log(isPaused);
            if (isPaused)
                pause();
            else
                unPause();
        }
    }

    private void pause()
    {
        Time.timeScale = 0;
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].activeInHierarchy)
            {
                wasOn[i] = true;
                canvases[i].SetActive(false);
            }
        }
        pauseMenu.SetActive(true);
    }

    private void unPause()
    {
        Time.timeScale = timeScale;
        for (int i = 0; i < canvases.Length; i++)
        {
            if (wasOn[i])
            {
                wasOn[i] = false;
                canvases[i].SetActive(true);
            }
        }
        pauseMenu.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        unPause();
    }

    public void Restart()
    {
        Time.timeScale = timeScale;
        Assets.Scripts.Data.Data.Instance.loadScene();
    }

    public void Quit()
    {
        Time.timeScale = timeScale;
        Assets.Scripts.Data.Data.Instance.loadScene("MenuTest");
    }
}
