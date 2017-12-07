using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpringJoint))]

public class PlayerCursor : MonoBehaviour
{
    public Texture cursorIsIdle;            // The sprite to display when the cursor is idle
    public Texture cursorCanInteract;       // The sprite to display when the cursor can interact with something
    public Texture cursorIsGrabbing;        // The sprite to display when the cursor is currently grabbing something
    public float cursorScale = 1;           // How large the cursor is onscreen

    PlayerInteractable lookingAt;           // What interactive object the player is currently looking at (if any)
    float interactDistance = 2.5f;          // The maximum distance from the player at which objects can be clicked on

    PlayerGrabbable grabbing;               // What grabbable object the player is currently grabbing (if any)
    float grabTime;                         // When the player began grabbing the object they're currently grabbing (used in conjunction with below)
    Vector3 grabPosition;                   // The position at which the object being grabbed wants to be held (diff. objects look nice with diff. values)
    float snapTime = 0.25f;                 // How long it takes before an object the player is grabbing is subject to the snap condition below
    float snapDistance = 1;                 // The maximum distance a grabbed object can get from the player before they automatically drop it
    float throwForce = 200;                 // How much force the player exerts on a grabbed object when they throw it

    GameController game;                    // The game
    SpringJoint grabJoint;                  // The spring joint used to grab rigidbodies

    void Awake()
    {
        game = GetComponentInParent<GameController>();
        grabJoint = GetComponent<SpringJoint>();
    }

    void Update()
    {
        if (game.gameState() != GameState.Play && game.gameState() != GameState.ExitingLevel)
            return;

        // Every frame, the cursor updates what interactable thing it's looking at
        // This is done by raycasting into the scene while ignoring the player / raycastignored
        // ~( (1 << 8) | (1 << 9) ) is a layermask which ignores those above two layers selectively
        // If we find a PlayerInteractable object within range, but it says it's disabled, ignore it

        RaycastHit hit;
        Ray ray = game.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Physics.Raycast(ray, out hit, interactDistance, ~( (1 << 8) | (1 << 2) ));

        PlayerInteractable current = null;
        if (hit.transform != null)
            current = hit.transform.GetComponent<PlayerInteractable>();
        if (current != null && !current.isInteractable())
            current = null;
        lookingAt = current;

        // If the player isn't grabbing anything, and they left click while looking at something interactable,
        // they need to either begin grabbing it (if it's PlayerGrabbable) or simply send it an interact signal

        if (grabbing == null)
        {
            if (Input.GetButtonDown("Mouse Left Click") && lookingAt != null)
            {
                if (lookingAt is PlayerGrabbable)
                {
                    grabbing = (PlayerGrabbable) lookingAt;
                    grabTime = Time.time;
                    grabPosition = grabbing.holdPosition;
                    transform.localPosition = grabPosition;

                    grabbing.transform.parent = transform;
                    grabbing.interact(0);
                    grabJoint.connectedBody = hit.rigidbody;

                    lookingAt = null;
                }

                else
                    lookingAt.interact(0);
            }
        }

        // Check if the grabbed object should be released. Multiple conditions satisfy this:
        // 1) The user presses LMB (dropped release)
        // 2) The user presses RMB (thrown release)
        // 3) The object becomes noninteractable
        // 4) After snapTime, the object's distance from the grabber becomes greater than snapDistance

        else
        {
            if (grabPosition != grabbing.holdPosition)
            {
                grabPosition = grabbing.holdPosition;
                grabbing.transform.parent = null;
                transform.localPosition = grabPosition;
                grabbing.transform.parent = transform;
            }

            if ((Input.GetButtonDown("Mouse Left Click"))
            || (Input.GetButtonDown("Mouse Right Click"))
            || (!grabbing.isInteractable())
            || (Time.time > grabTime + snapTime && Vector3.Distance(transform.position, grabbing.transform.position) > snapDistance))
            {
                grabbing.transform.parent = null;
                grabbing.interact(1);
                grabJoint.connectedBody.velocity = game.player.getPhysicalVelocity();
                if (Input.GetButtonDown("Mouse Right Click"))
                    grabJoint.connectedBody.AddForce(transform.rotation * Vector3.forward * throwForce);
                grabJoint.connectedBody = null;

                grabbing = null;
            }
        }

        // Make the grabber bob with the camera

        Vector3 target = grabPosition - (transform.parent.localPosition - new Vector3(0, 1.5f, 0)) * 0.5f;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, 0.5f);
    }
    
    void OnGUI()
    {
        if (game.gameState() != GameState.Play)
            return;

        // Draw the cursor to the screen
        // Which sprite is used depends on the cursor's state

        Texture cursor;
        if (isGrabbing())
            cursor = cursorIsGrabbing;
        else if (isLookingAt())
            cursor = cursorCanInteract;
        else
            cursor = cursorIsIdle;

        float size = cursor.width * cursorScale;
        Rect position = new Rect((Screen.width - size) / 2, (Screen.height - size) / 2, size, size);
        GUI.DrawTexture(position, cursor);
    }

    // Helper functions which let the cursor (and any other object) obtain
    // convenient conditional information about the cursor's state

    public bool isLookingAt()
    {
        return (lookingAt != null);
    }

    public bool isLookingAt(PlayerInteractable test)
    {
        return (lookingAt == test);
    }

    public bool isLookingAt(PlayerInteractable[] test)
    {
        foreach (PlayerInteractable t in test)
            if (lookingAt == t)
                return true;
        return false;
    }

    public bool isGrabbing()
    {
        return (grabbing != null);
    }

    public bool isGrabbing(PlayerGrabbable test)
    {
        return (grabbing == test);
    }
}