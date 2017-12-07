using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFuncs : MonoBehaviour {
    public bool shaking = false;
    public float shakeStr = 0.5f;
    Vector3 origPos;

    public bool followX = false;
    public bool followY = false;
    Transform target;
    Camera cam;
    [Range(0, 1)]
    public float cameraDamping = 1f;

    // Use this for initialization
    void Start() {
        origPos = transform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        //Maintains the Camera aspect ratio to ensure everything is in the same place
        //When running on different computers of different screen sizes.
        cam = GetComponent<Camera>();
        //Screen.SetResolution(1280, 549, false);
        float screenRatio = cam.orthographicSize / (1280f / 549f);
        cam.orthographicSize = screenRatio * (cam.pixelHeight / cam.pixelWidth);
        //oldHeight = cam.pixelHeight;
        //oldWidth = cam.pixelWidth;
    }

    public bool getShaking() {
        return shaking;
    }
    // Update is called once per frame
    int oldWidth = 1280;
    int oldHeight = 549;
    void Update() {
        float normalAspect = 2;

        cam.fieldOfView = 70 * normalAspect / ((float)cam.pixelWidth / cam.pixelHeight);
        if (Input.GetKeyDown(KeyCode.F1)) {
            print(Screen.width + " , " + Screen.height);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (followX) {
            transform.position = Vector3.Slerp(transform.position, new Vector3(target.position.x, transform.position.y, transform.position.z), cameraDamping);
        }
        if (followY) {
            transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, target.position.y, transform.position.z), cameraDamping);
        }
    }

    public void shake(float shakeWeight, float time) {
        StartCoroutine(shakeScreen(shakeWeight, time));
    }
    public void shakeOnce() {
        transform.position = new Vector3(Random.Range(origPos.x - shakeStr, origPos.x + shakeStr),
                Random.Range(origPos.y - shakeStr, origPos.y + shakeStr),
                transform.position.z);
    }

    public void endShake() {
        transform.position = origPos;
    }
    public IEnumerator shakeScreen() {
        shaking = true;
        origPos = transform.position;
        float deltaT = 0;
        while (deltaT < 0.05f) {
            transform.position = new Vector3(Random.Range(origPos.x - shakeStr, origPos.x + shakeStr),
                Random.Range(origPos.y - shakeStr, origPos.y + shakeStr),
                transform.position.z);
            yield return new WaitForSeconds(0.03f);
            deltaT += Time.deltaTime;
        }
        transform.position = origPos;
        shaking = false;
    }

    public IEnumerator shakeScreen(float customShake) {
        shaking = true;
        Vector3 origPos = transform.position;
        float deltaT = 0;
        while (deltaT < 0.05f) {
            transform.position = new Vector3(Random.Range(origPos.x - customShake, origPos.x + customShake),
                Random.Range(origPos.y - customShake, origPos.y + customShake),
                transform.position.z);
            yield return new WaitForSeconds(0.03f);
            deltaT += Time.deltaTime;
        }
        transform.position = origPos;
        shaking = false;
    }

    public IEnumerator shakeScreen(float customShake, float wait) {
        shaking = true;
        Vector3 origPos = transform.position;
        float deltaT = 0;
        while (deltaT < 0.05f) {
            transform.position = new Vector3(Random.Range(origPos.x - customShake, origPos.x + customShake),
                Random.Range(origPos.y - customShake, origPos.y + customShake),
                transform.position.z);
            yield return new WaitForSeconds(wait);
            deltaT += Time.deltaTime;
        }
        transform.position = origPos;
        shaking = false;
    }

    bool inSlowMo = false;
    IEnumerator slowMo(float seconds) {
        if (!inSlowMo) {
            inSlowMo = true;
            Time.timeScale = 0.3f;
            yield return new WaitForSeconds(seconds);
            Time.timeScale = 1;
            inSlowMo = false;
        }
    }

    public void startSlowMo() {
        //StartCoroutine(slowMo(0.65f));
    }

    public void stopFollow() {
        followX = false;
        followY = false;
    }

    public void loadNextScene() {
        StartCoroutine(loadScene());
    }

    public void loadNextScene(int sceneNum) {
        StartCoroutine(loadScene(sceneNum));
    }

    IEnumerator loadScene() {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator loadScene(int sceneNum) {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneNum);
    }
    float waitTime = 3;
    public void setWaitTime(float val) {
        waitTime = val;
    }

}
