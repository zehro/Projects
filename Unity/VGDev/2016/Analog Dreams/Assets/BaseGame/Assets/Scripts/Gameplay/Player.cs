using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool quietFootsteps = false;

    Camera cam;
    float camXRot = 0.0f;                           // the look rotation yaw (controlled by mouse X)
    float camYRot = 0.0f;                           // the look rotation pitch (controlled by mouse Y)
    float camSensitivity = 0.0005f;                 // the sensitivity of the camera to mouse movement
    float camDrag = 0.35f;                          // the factor by which the camera approaches the target rotation
    float camHeight = 1.5f;                         // the vertical distance the camera rests above the player's origin
    float camBobHStrength = 0.09f;                  // the distance the camera travels horizontally when bobbing
    float camBobVStrength = 0.15f;                  // the distance far the camera travels vertically when bobbing
    float camShake = 0;                             // the distance the camera should currently shake
    float camShakeFalloff = 0.98f;                  // how quickly the camera shake resets to 0

    CharacterController player;
    float playerWalkSpeed = 6.0f;                   // how fast the player walks
    float playerSlideSpeed = 5.0f;                  // how fast the player slides down oversteep geometry
    float playerJumpForce = 8.0f;                   // the force the player applies upwards when they jump
    float playerPushForce = 60.0f;                  // the force the player enacts on rigidbodies they run into
    float playerGravForce = 24.0f;                  // the force of gravity enacted on the player
    float playerAntiBumpForce = 0.0f;               // the force which attempts to keep the player grounded
    float playerAntiBumpReset = 6.0f;               // the default force which attempts to keep the player grounded
    int   playerJumpCount = 0;                      // the number of times the player has jumped
    bool  playerCanJump = false;                    // whether or not the player can jump
    bool  playerIsGrounded = false;                 // whether or not the player is grounded
    bool  playerIsSliding = false;                  // whether or not the player is sliding
    float playerMaxGroundDist = 0.6f;               // the maximum distance from the floor the player can be to be grounded
    float playerMaxGroundAngle = 46.0f;             // the maximum slope the player can stand on while still being grounded
    public Vector3 playerMove;                      // the vector which represents the player's overall attempted move per step
    Vector3 playerLastMove;                         // the vector which represents the total move that the player made the last step
    Vector3 playerSlideNormal;                      // the vector which stores the normal of the thing the player should be sliding against
    public Quaternion playerRot;                    // the quaternion which represents the reference-frame rotation of the player
    ControllerColliderHit playerStoredHit;          // the collider hit which stores the data on the last thing the player bumped into

    AudioSource footstepAudio;
    public AudioClip[] footstepSounds;              // the footstep sounds to select from when the player steps
    int   footstepIndex = 0;                        // the index of the footstep sound that last played (used to avoid repetitions)
    float footstepTimer = 0.0f;                     // the timer which keeps track of roughly how far the player has walked (for headbob & footstep audio)
    float footstepTimerPrev = 0.0f;                 // the previous value of the above timer (the footstep audio routine uses this to check for full steps)
    float footstepSpeed = 0.045f;                   // the speed at which player movement in the local XZ plane adds to the footstep timer
    float footstepReset = 0.9f;                     // the decimal to which the timer resets when the player stops moving or is airborne

    AudioSource groundAudio;
    public AudioClip groundJumpSound;               // the sound played when the player jumps
    public AudioClip groundLandSound;               // the sound played when the player lands

    AudioSource windAudio;
    float windMinSpeed = 0.21f;                     // the minimum speed the player must be moving to hear the wind effect
    float windMaxSpeed = 0.75f;                     // the speed the player must be moving for the wind effect to be played at full volume
    float windVolumeFalloff = 0.9f;                 // the factor by which the wind volume diminishes if it is above the target volume
    float windVolumeCutoff = 0.001f;                // the cutoff at which the wind volume is truncated to 0.0 if it falls under

    void Awake()
    {
        cam = transform.Find("PlayerCamera").GetComponent<Camera>();
        player = GetComponent<CharacterController>();
        footstepAudio = GetComponents<AudioSource>()[0];
        groundAudio = GetComponents<AudioSource>()[1];
        windAudio = GetComponents<AudioSource>()[2];
    }

    void Update()
    {
        // Camera mouselook is based on mouse input

        camXRot += Input.GetAxisRaw("Mouse X") * camSensitivity * Time.timeScale;
        camYRot += Input.GetAxisRaw("Mouse Y") * camSensitivity * Time.timeScale;
        camYRot = Mathf.Clamp(camYRot, -90, 90);
        Quaternion xRot = Quaternion.AngleAxis(camXRot, Vector3.up);
        Quaternion yRot = Quaternion.AngleAxis(camYRot, Vector3.left);
        cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, xRot * yRot, camDrag * Time.timeScale);

        // The camera headbob is an animation based on the footstep timer that repeats on twos
        // The strength of the headbob animation is based on how long the last step was

        Vector3 step = Quaternion.Inverse(playerRot) * playerLastMove;
        float stepDist = Mathf.Sqrt(step.x * step.x + step.z * step.z);
        float hBob = Mathf.Sin(footstepTimer * Mathf.PI + 3) * camBobHStrength * stepDist;
        float vBob = Mathf.Sin(footstepTimer * 2 * Mathf.PI + 3) * camBobVStrength * stepDist;
        cam.transform.localPosition = xRot * new Vector3(hBob, camHeight + vBob, 0);

        // Camera shake

        cam.transform.localPosition += Random.onUnitSphere * camShake;
        camShake *= camShakeFalloff;

        // Footstep audio plays every time the footstep timer crosses to the next whole number
        // The volume of the footstep audio is also based on how long the last step was

        footstepAudio.volume = stepDist * (quietFootsteps ? 2.5f : 5);

        if (Mathf.Floor(footstepTimer) > Mathf.Floor(footstepTimerPrev))
        {
            footstepIndex += Random.Range(1, footstepSounds.Length);
            footstepIndex %= footstepSounds.Length;
            footstepAudio.pitch = 0.9f + Random.Range(0.0f, 0.2f);
            footstepAudio.PlayOneShot(footstepSounds[footstepIndex]);
        }

        footstepTimerPrev = footstepTimer;

        // Wind audio - target volume is controlled by how fast the player is moving
        // The actual volume of the audio increases immediately to the target volume,
        // but decreases slowly, which makes the wind effect sound more realistic

        float t = Mathf.InverseLerp(windMinSpeed, windMaxSpeed, playerLastMove.magnitude);
        if (windAudio.volume <= t * t)
            windAudio.volume = t * t;
        else
            windAudio.volume *= windVolumeFalloff;
        if (windAudio.volume < windVolumeCutoff)
            windAudio.volume = 0;
    }

    void FixedUpdate()
    {
        // Get movement input and sanitize it

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(hor, ver);
        if (input.magnitude > 1)
            input.Normalize();

        // Pass in the input to the footstep timer

        footstepTimer += input.magnitude * footstepSpeed;
        if (input.magnitude == 0 || !playerIsGrounded)
            footstepTimer = Mathf.Floor(footstepTimer) + footstepReset;

        // Start building what the player's attempted move is going to be
        // What the player can do is based on whether or not they're grounded

        playerMove.x = input.x * playerWalkSpeed;
        playerMove.z = input.y * playerWalkSpeed;

        if (playerIsGrounded)
        {
            // Keep the player tracked to the ground (cheapo method)
            
            playerMove.y = -playerAntiBumpForce * input.magnitude;
            playerAntiBumpForce = playerAntiBumpReset;

            // Let the player jump (plays audio)

            if (!Input.GetButton("Jump"))
                playerCanJump = true;
            else if (playerCanJump)
            {
                playerJumpCount++;
                playerCanJump = false;
                playerMove.y = playerJumpForce;
                groundAudio.pitch = 0.9f + Random.Range(0.0f, 0.2f);
                groundAudio.PlayOneShot(groundJumpSound);
            }
        }

        else if (playerIsSliding)
        {
            // Slide the player down oversteep slopes

            Vector3 slideMove = Quaternion.Inverse(playerRot) * playerSlideNormal;
            slideMove.Scale(new Vector3(playerSlideSpeed, -playerSlideSpeed, playerSlideSpeed));
            playerMove += slideMove;
        }

        // Finish building the move (adding gravity) and then make it
        // Store whatever the end result is in the vector playerLastMove
        
        if (!playerIsGrounded && !playerIsSliding)
            playerMove.y -= playerGravForce * Time.deltaTime;
        playerRot = transform.rotation * Quaternion.AngleAxis(camXRot, Vector3.up);
        playerLastMove = transform.position;
        player.Move(playerRot * playerMove * Time.deltaTime);
        playerLastMove = -playerLastMove + transform.position;

        // Check whether or not the player is grounded or sliding (they aren't opposite conditions)
        // If there's something directly underneath the player, they're grounded and not sliding.
        // Otherwise, if we hit something in the last frame, check that normal. If it's less than the
        // maximum slope the player can stand on, the player is grounded, otherwise they're sliding
        // ~(1 << 9) is a layermask which collides with everything except the IgnorePlayer layer
        // This layer is used by objects to be considered "unsolid" (recently grabbed ones, usually)

        bool wasGrounded = playerIsGrounded;
        playerIsGrounded = Physics.Raycast(transform.position, playerRot * Vector3.down, playerMaxGroundDist, ~(1 << 9));
        playerIsSliding = false;

        if (!playerIsGrounded && playerStoredHit != null)
        {
            float angle = Vector3.Angle(playerStoredHit.normal, playerRot * Vector3.up);
            
            if (angle < playerMaxGroundAngle)
                playerIsGrounded = true;

            else if (angle < 89)
            {
                playerIsSliding = true;
                playerSlideNormal = playerStoredHit.normal;
            }
        }
        
        playerStoredHit = null;

        // Play the landing sound if the player wasn't grounded in the previous frame and now they are
        // The volume and pitch of the landing sound are based on how fast the player was trying to fall

        if (playerIsGrounded && !wasGrounded)
        {
            float v = Mathf.Clamp01(playerMove.y * -0.04f);
            groundAudio.pitch = 1.1f + Random.Range(0.0f, 0.3f) - v * 0.35f;
            groundAudio.PlayOneShot(groundLandSound, v * v * 2.25f);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Store this collision for grounded/sliding checks
        // Enact a push force on any colliding rigidbodies

        playerStoredHit = hit;

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
            body.AddForce(hit.moveDirection * playerPushForce);
    }

    public void Portal(Transform from, Transform to)
    {
        // Utilize parenting for seamless portal transition

        Transform p = transform.parent;
        transform.SetParent(from, true);
        transform.SetParent(to, false);
        transform.parent = p;
    }

    public Vector3 getPhysicalVelocity()
    {
        return playerLastMove / Time.fixedDeltaTime;
    }

    public Quaternion getAngularVelocity()
    {
        Quaternion result = Quaternion.AngleAxis(Input.GetAxisRaw("Mouse X") * camSensitivity * Time.timeScale, Vector3.up);
        result *= Quaternion.AngleAxis(Input.GetAxisRaw("Mouse Y") * camSensitivity * Time.timeScale, Vector3.left);
        return result;
    }

    public bool isGrounded()
    {
        return (playerIsGrounded && !playerIsSliding);
    }

    public void cameraShake(float shake)
    {
        camShake = shake;
    }

    public void setAntiBumpForce(float abf)
    {
        playerAntiBumpForce = abf;
    }
}