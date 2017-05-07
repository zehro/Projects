using UnityEngine;
using TeamUtility.IO;
using YeggQuest.NS_Spline;

enum MoveState {MOVING, STOPPED, END}

public class TruckController : MonoBehaviour {

    #region PublicVariables

    public bool tubCollision = true;
    public PlayerID player;
    [Header("Camera Options")]
    public Vector2 lookXRestraints;
    public Vector2 lookYRestraints;
    [Range(0f, 5f)]
    public float lookSpeed;

    [Header("Physics Options")]
    public float maxMovementSpeed;
    [Range(0f, 70f)]
    public float warpForce;
    [Range(0f, 200f)]
    public float moveForce;
    [Range(0f, 1f)]
    public float drag = .1f;
    #endregion

    #region PrivateVariables
    MoveState state = MoveState.STOPPED;
    Rigidbody body;
    Camera cam;
    Animator anim;
    Vector3 position;
    Vector3 speed;
    SplineFollower follower;
    Spline spline;

    int tunnelRadius = 50;

    float lookX;
    float lookY;
    float x, y, z, t;
    #endregion

    #region Initialization
    // Use this for initialization
    void Start () {
        cam = GetComponentInChildren<Camera>();
        body = GetComponent<Rigidbody>();
        follower = GetComponentInParent<YeggQuest.NS_Spline.SplineFollower>();
        follower.Playing(false);
        spline = follower.wrapper.spline;
        anim = GetComponentInChildren<Animator>();
    }
    #endregion

    #region Updates
    void Update() {
        switch (state)
        {
            case MoveState.END:
                // At end of level reset camera rotationn
                //cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation,
                //    Quaternion.Euler(0, 0, 0), .1f);
                break;
            default:
                // Get inputs from the camera inputs
                lookX += InputManager.GetAxis("LookHorizontal", player) * lookSpeed;
                lookY += InputManager.GetAxis("LookVertical", player) * lookSpeed;
                // Limit the look axises
                lookX = Mathf.Clamp(lookX, lookXRestraints.x, lookXRestraints.y);
                lookY = Mathf.Clamp(lookY, lookYRestraints.x, lookYRestraints.y);
                // Calculate the rotation using Euler angles,
                Quaternion rotation = Quaternion.Euler(lookY, lookX, 0);
                // Set the camera's new rotation
                cam.transform.localRotation = rotation;
                break;
        }
        Vector2 polar = TCUtil.CartesionToPolar(transform.localPosition);
        if (polar.x >= .80f && tubCollision) {
            
            // TODO: Warn the player

            if (polar.x >= .90f * tunnelRadius)
            {
                transform.localPosition = new Vector3(Mathf.Cos(polar.y) * (.85f * tunnelRadius), 
                    Mathf.Sin(polar.y) * (.85f * tunnelRadius), transform.localPosition.z);
                GetComponent<CargoHealth>().loseCargo();
            }
        }
    }

    void FixedUpdate() {
        // Get move inputs
        x = getHorizontal();
        y = getVetical();
        switch (state) {
            case MoveState.STOPPED:
                if (InputManager.GetButton("Submit", player)) {
                    start();
                }
                break;
            case MoveState.MOVING:
                // Updating the direction animation variable
                anim.SetFloat("Direction", x);

                // Calculate the motion direction vector and scale it by the moveForce
                speed = (x * transform.parent.right + y * transform.parent.up) * moveForce;
                // Clamp the speed
                speed = Vector3.ClampMagnitude(speed, maxMovementSpeed);
                // Add the player's speed to the rigidbody velocity relative to the parent
                body.AddRelativeForce(speed, ForceMode.VelocityChange);
                // Drag
                TCUtil.Drag(body, body.velocity, drag);
                // Adjust forward
                int sec = follower.currenSection;
                int seg = follower.currentSegment;
                // Grab tangents from spline
                int numOfTangets = 10;
                Vector3 smoothedTangent = new Vector3(0,0,-5);
                for (int i = 0; i < numOfTangets; i++)
                {
                    smoothedTangent = smoothedTangent + Spline.Tangent(spline.vertices, sec, seg, spline.segmentCount, 0);
                    seg++;
                    if(seg >= spline.segmentCount - 1)
                    {
                        seg = 0;
                        sec++;
                        if(sec >= spline.vertices.GetLength(0))
                        {
                            sec = spline.vertices.GetLength(0)-1;
                            seg = spline.segmentCount - 2;
                        }
                    }
                }
                smoothedTangent.Normalize();
                transform.localRotation = Quaternion.Lerp(transform.localRotation,
                    Quaternion.LookRotation(smoothedTangent), .01f);

                break;
            case MoveState.END:
                break;
        }
    }
    #endregion

    #region Methods

    float getHorizontal()
    {
        return InputManager.GetAxisRaw("Horizontal", player);
    }

    float getVetical()
    {
        return InputManager.GetAxisRaw("Vertical", player);
    }

    public void shutdown()
    {
        state = MoveState.END;
        body.velocity = Vector3.zero;
        follower.enabled = false;
    }

    public void start()
    {
        state = MoveState.MOVING;
        follower.Playing(true);
    }

    public void reverse(Vector3 normal)
    {
        body.AddForce(-normal * 300 + Vector3.forward * warpForce, ForceMode.Impulse);
    }

    public int getState() {
        return (int) state;
    }

    #endregion
}
