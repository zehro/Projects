using UnityEngine;
using System.Collections;

public class PortalDoorSurface : MonoBehaviour
{
    public Mesh entranceSurface;
    public Mesh exitSurface;
    public Shader portalShader;
    public Color color;
    public float activationMultiplier = 1;

    float playerMinDist = 12;
    float playerMaxDist = 16;
    float playerSign;
    //float playerCooldown;
    //float playerReset = 2;
    bool isEntrance;
    PortalDoorSurface linkedSurface;

    RenderTexture portalRenderTex;
    Material portalMaterial;

    GameController game;
    MeshRenderer portalRenderer;
    Camera cam;

    void Awake()
    {
        game = GameObject.Find("GameController").GetComponent<GameController>();
        portalRenderer = GetComponent<MeshRenderer>();
        cam = transform.Find("PortalDoorSurfaceCamera").GetComponent<Camera>();
    }

    void Start()
    {
        // Create the render texture and the material and assign everything required
        // to get the pipeline working (this lets you see through the portal)

        portalRenderTex = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
        portalRenderTex.Create();
        portalRenderTex.hideFlags = HideFlags.DontSave;

        portalMaterial = new Material(portalShader);
        portalMaterial.hideFlags = HideFlags.DontSave;
        portalMaterial.SetTexture("_MainTex", portalRenderTex);

        portalRenderer.material = portalMaterial;
        cam.targetTexture = portalRenderTex;
    }

    void Update()
    {
        // Check if the player has walked through this portal, and if so, portal them

        if (linkedSurface != null)
        {
            Vector3 localPoint = transform.InverseTransformPoint(game.playerCamera.transform.position);

            float sign = Mathf.Sign(localPoint.y);
            float signTest = (isEntrance ? 1 : -1);
            bool xCheck = (Mathf.Abs(localPoint.x) < 1);
            bool yCheck = (Mathf.Abs(localPoint.y) < 4f);
            bool zCheck = (Mathf.Clamp(localPoint.z, 0, 4) == localPoint.z);
            
            if (sign != playerSign && sign == signTest && xCheck && yCheck && zCheck && playerSign != 0)// && playerCooldown == 0)
            {
                //playerCooldown = playerReset;
                //linkedSurface.playerCooldown = playerReset;
                game.player.Portal(transform, linkedSurface.transform);
            }

            playerSign = sign;
            /*if (playerCooldown > 0)
                playerCooldown--;*/
        }
    }

    void LateUpdate()
    {
        if (linkedSurface != null &&
           (Vector3.Distance(transform.position, game.playerCamera.transform.position) < playerMaxDist * activationMultiplier ||
            Vector3.Distance(linkedSurface.transform.position, game.playerCamera.transform.position) < playerMaxDist * linkedSurface.activationMultiplier))
        {
            cam.gameObject.SetActive(true);
            cam.transform.position = Vector3.zero;
            cam.transform.rotation = Quaternion.identity;
            cam.transform.SetParent(game.playerCamera.transform, false);
            cam.transform.SetParent(transform, true);
            cam.transform.SetParent(linkedSurface.transform, false);
            cam.transform.parent = null;
            cam.fieldOfView = game.playerCamera.fieldOfView;
        }

        else
            turnOffCam();

        float dist = Vector3.Distance(transform.position, game.playerCamera.transform.position);
        float t = Mathf.Clamp01((dist - playerMinDist * activationMultiplier) / (playerMaxDist * activationMultiplier - playerMinDist * activationMultiplier));
        color.a = Mathf.Max(color.a, Mathf.SmoothStep(0, 1, t));
        portalMaterial.SetColor("_FadeColor", color);
    }

    public void setSurfaceType(bool entrance)
    {
        isEntrance = entrance;
        GetComponent<MeshFilter>().mesh = (entrance ? entranceSurface : exitSurface);
        transform.localRotation = Quaternion.Euler(0, 0, (entrance ? 180 : 0));
    }

    public void setLinkedSurface(PortalDoorSurface link)
    {
        linkedSurface = link;
        //playerCooldown = 2;
    }

    public void turnOffCam()
    {
        if (cam == null)
            return;
        cam.gameObject.SetActive(false);
        cam.transform.parent = transform;
    }
}