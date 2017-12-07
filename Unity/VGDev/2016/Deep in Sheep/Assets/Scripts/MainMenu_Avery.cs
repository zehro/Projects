using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TeamUtility.IO;

public class MainMenu_Avery : MonoBehaviour {
    //clip that plays when selection changes
    public AudioClip switchButtonSound;
    public AudioClip SelectSound;

    //buttons that make up the two menus
    public Button[] mainMenuButtons;
    public Button[] sceneSelectButtons;
    public Button[] optionsButtons;

    //the button in which is currently selected of the current list
    private int selectedIndex;

    //canInteract - variable that helps keep a delay in between selections
    //menuActive - if true, input selects options of the main menu
    //sceneActive - if true, input selects options of the scene selection menu
    //optionsActive - if true, input selects optionsMenu
    private bool canInteract;
    private bool menuActive;
    private bool sceneActive;
    private bool optionsActive;
    private bool tutorialActive;
    private bool creditsActive;

    private static bool musicSound;
    private static bool fxSound;
    private static bool controllers;

    //canvas in worldspace in which buttons apear
    public GameObject mainMenuCanvas;
    public GameObject sceneSelectCanvas;
    public GameObject optionsmenuCanvas;
    public GameObject creditsMenu;

    public AudioSource music;

    public GameObject musicCheck;
    public GameObject fxCheck;
    public GameObject ctrlrCheck;

    public PlayerID player;

    //objects references for animation purposes
    public GameObject barn;
    public GameObject cameraController;

    bool newListener;

    //runs on startup
    void Start()
    {
        optionsmenuCanvas.SetActive(false);
        creditsMenu.SetActive(false);
        creditsActive = false;
        menuActive = true;
        sceneActive = false;
        optionsActive = false;
        tutorialActive = false;
        canInteract = true;
        selectedIndex = 0;
        musicSound = true;
        fxSound = true;
        music.ignoreListenerVolume = true;
        newListener = true;
    }

    public static bool getMusicSound()
    {
        return musicSound;
    }

    public static bool getFXSound()
    {
        return fxSound;
    }

    public static bool getControllers()
    {
        return controllers;
    }

    public void setMusicSound(bool newMusicSound)
    {
        musicSound = newMusicSound;
    }

    public void setFXSound(bool newFXSound)
    {
        fxSound = newFXSound;
    }

    public void setControllers(bool newControllers)
    {
        controllers = newControllers;
    }

    //this method is responsible for changing the index of the current menu.
    //parameter buttons (which set of buttons to be indexed) 
    //parameter selectedInput (the direction in which buttons will be changed)
    IEnumerator menuSelect(Button[] buttons,  float selectedInput)
    {
        AudioSource.PlayClipAtPoint(switchButtonSound, Camera.main.transform.position);
        buttons[selectedIndex].GetComponent<Animation>().Stop();
        buttons[selectedIndex].GetComponent<Animation>().Play("idleButton");

        if (selectedInput > 0)
        {
            if (selectedIndex == 0) {
                selectedIndex = buttons.Length - 1;
            } else {
                selectedIndex--;
            }
        }

        if (selectedInput < 0)
        {
            if (selectedIndex == buttons.Length - 1)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex++;
            }

        }
        yield return new WaitForSeconds(0.4f);
        canInteract = true;
    }

    //called every frame
    //takes in input and decides which menu will take that input
    void FixedUpdate()
    {
        float verticalInput = InputManager.GetAxis("Vertical", player);
        float horizontalInput = InputManager.GetAxis("Horizontal", player);

        if (menuActive)
        {
            if (newListener)
            {
                Debug.Log("age");
                mainMenuButtons[selectedIndex].onClick.AddListener(delegate { StartCoroutine(handleSelectionMain()); Debug.Log("Gello"); });
                newListener = false;
            }
            mainMenuButtons[selectedIndex].GetComponent<Animation>().Play();
            if (verticalInput != 0 && canInteract)
            {
                canInteract = false;
                newListener = true;
                StartCoroutine(menuSelect(mainMenuButtons, verticalInput));
            }

            if (InputManager.GetButton("Submit") && canInteract)
            {
                canInteract = false;
                mainMenuButtons[selectedIndex].onClick.RemoveAllListeners();
                newListener = true;
                StartCoroutine(handleSelectionMain());
            }
        }

        if (optionsActive)
        {
            optionsButtons[selectedIndex].GetComponent<Animation>().Play();
            if (verticalInput != 0 && canInteract)
            {
                canInteract = false;
                StartCoroutine(menuSelect(optionsButtons, verticalInput));
            }

            if (InputManager.GetButton("Submit") && canInteract)
            {
                canInteract = false;
                StartCoroutine(handleSelectionOptions());
            }

        }

        if (tutorialActive)
        {
            if (InputManager.GetButton("Submit") && canInteract)
            {
                canInteract = false;
                selectedIndex++;
                StartCoroutine(handleSelectionTutorial());
            }
        }

        if (creditsActive)
        {
            if (InputManager.GetButton("Submit") && canInteract)
            {
                selectedIndex = 0;
                optionsActive = true;
                optionsmenuCanvas.SetActive(true);
                creditsMenu.SetActive(false);
                creditsActive = false;
            }
        }

        if (sceneActive)
        {
            if (horizontalInput != 0 && canInteract)
            {
                canInteract = false;
                StartCoroutine(menuSelect(sceneSelectButtons, -horizontalInput));
            }

            if (InputManager.GetButton("Submit") && canInteract)
            {
                canInteract = false;
                StartCoroutine(handleSelectionScene());
            }

            sceneSelectButtons[selectedIndex].GetComponent<Animation>().Play();
        }
    }

    //handles which method is called from selected button
    IEnumerator handleSelectionMain()
    {
        AudioSource.PlayClipAtPoint(SelectSound, Camera.main.transform.position);
        yield return new WaitForSeconds(0.4f);
        canInteract = true;
        switch (selectedIndex)
        {
            case 0:
                PlayGame();
                break;
            case 1:
                StartTutorial();
                break;
            case 2:
                StartOptions();
                break;
            case 3:
                Quit();
                break;
            default:
                break;
        }

    }

    IEnumerator handleSelectionTutorial()
    {
        AudioSource.PlayClipAtPoint(SelectSound, Camera.main.transform.position);
        yield return new WaitForSeconds(0.4f);
        canInteract = true;
        switch (selectedIndex)
        {
            case 0:
                break;
            case 1:
                cameraController.GetComponent<Animator>().Play("frame1_2Tutorial");
                break;
            case 2:
                cameraController.GetComponent<Animator>().Play("fraame2_3Tutorial");
                break;
            case 3:
                cameraController.GetComponent<Animator>().Play("tutorialExit");
                menuActive = true;
                tutorialActive = false;
                selectedIndex = 0;
                break;
            default:
                break;
        }
    }

    IEnumerator handleSelectionOptions()
    {
        AudioSource.PlayClipAtPoint(SelectSound, Camera.main.transform.position);
        yield return new WaitForSeconds(0.4f);
        canInteract = true;
        switch (selectedIndex)
        {
            case 0:
                fxToggle();
                break;
            case 1:
                musicToggle();
                break;
            case 2:
                ctrlrToggle();
                break;
            case 3:
                credits();
                break;
            case 4:
                optionsBack();
                break;
            default:
                break;
        }

    }

    //method envolked when play button is selected. triggers animation to the
    // barn and camera. sets input to scene selection menu. 
    void PlayGame()
    {
        mainMenuButtons[selectedIndex].GetComponent<Animation>().Stop();
        menuActive = false;
        sceneActive = true;
        selectedIndex = 0;
        cameraController.GetComponent<Animator>().Play("SceneSelect");
        barn.GetComponent<Animator>().Play("BarnDoors");
    }

    void credits()
    {
        optionsActive = false;
        optionsmenuCanvas.SetActive(false);
        creditsMenu.SetActive(true);
        creditsActive = true;
        //yield return new WaitForSeconds(0.4f);

    }

    //handles input in the scene selection menu depending on button
    IEnumerator handleSelectionScene()
    {
        AudioSource.PlayClipAtPoint(SelectSound, Camera.main.transform.position);
        yield return new WaitForSeconds(0.4f);
        canInteract = true;

        switch (selectedIndex)
        {
            case 0:
                SceneManager.LoadScene("RollingHills");
                music.Stop();
                break;
            case 1:
                SceneManager.LoadScene("Mountains");
                music.Stop();
                break;
            case 2:
                SceneManager.LoadScene("RoadMap");
                music.Stop();
                break;
            case 3:
                BackToMenu();
                break;
            default:
                break;
        }
    }

    //the method in the scene selection menu that returns to the main menu
    void BackToMenu()
    {
        sceneSelectButtons[selectedIndex].GetComponent<Animation>().Stop();
        menuActive = true;
        sceneActive = false;
        selectedIndex = 0;
        cameraController.GetComponent<Animator>().SetTrigger("CameraBack");
        barn.GetComponent<Animator>().SetTrigger("CloseTrigger");
    }

    void StartTutorial()
    {
        menuActive = false;
        tutorialActive = true;
        selectedIndex = 0;
        cameraController.GetComponent<Animator>().Play("enterTutorial");
        mainMenuButtons[1].GetComponent<Animation>().Stop();
        mainMenuButtons[1].GetComponent<Animation>().Play("idleButton");
    }

    //not finished, but gives an idea of what to come
    void StartOptions()
    {
        menuActive = false;
        optionsActive = true;
        selectedIndex = 0;
        mainMenuCanvas.SetActive(false);
        cameraController.GetComponent<Animator>().Play("menuSwitch");
        optionsmenuCanvas.SetActive(true);
    }

    void musicToggle()
    {
        if (musicSound == true)
        {
            musicCheck.SetActive(false);
            musicSound = false;
            music.volume = 0;

        }
        else if (musicSound == false)
        {
            musicCheck.SetActive(true);
            musicSound = true;
            music.volume = 1;

        }
    }

    void fxToggle()
    {
        if (fxSound == true)
        {
            fxCheck.SetActive(false);
            fxSound = false;
            AudioListener.volume = 0;
        }
        else if (fxSound == false)
        {
            fxCheck.SetActive(true);
            fxSound = true;
            AudioListener.volume = 1;

        }
    }

    void ctrlrToggle()
    {
        if (controllers == true)
        {
            ctrlrCheck.SetActive(false);
            controllers = false;
            InputManager.SetInputConfiguration("Windows_Gamepad2", PlayerID.Two);
            InputManager.SetInputConfiguration("Windows_Gamepad1", PlayerID.One);
        }
        else
        {
            ctrlrCheck.SetActive(true);
            controllers = true;
            InputManager.SetInputConfiguration("KeyboardAndMouse", PlayerID.One);
            InputManager.SetInputConfiguration("Windows_Gamepad1", PlayerID.Two);
        }
    }

    void optionsBack()
    {
        menuActive = true;
        optionsActive = false;
        optionsmenuCanvas.SetActive(false);
        cameraController.GetComponent<Animator>().Play("menuSwitch");
        mainMenuCanvas.SetActive(true);
        selectedIndex = 0;
    }

    //selected when quit is selected and exits the application
    void Quit()
    {
        Application.Quit();
    }

}
