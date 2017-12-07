using UnityEngine;
using TeamUtility.IO;

/// <summary>
/// Simple third person controller with sheep wrangling capabilites.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveForce;
    public float distance;
    public float height;
    public float xRot;
    public float isGroundedDist;
    public float groundStickForce;

    //public int player;
    public PlayerID player;

    private float groundedTimeout;
    private float airControlTimeout;

    Animator animator;
    ParticleSystem dust;

    int playerControl = 1;

    public float lookSpeed = 50;
    float x = 0.0f;
    float dx;
    float dy;
    float dz;

    Rigidbody body;
    Camera cam;
    Transform model;
    Vector3 velocity, negDistance, position, camForward, spd, desiredVelocity, addVec;
    Quaternion rotation;

    void Start()
    {
        //Initialize the component references
        body = GetComponent<Rigidbody>();
        Debug.Assert(transform.Find("Camera").gameObject.GetComponent<Camera>());
        cam = transform.Find("Camera").gameObject.GetComponent<Camera>();
        Debug.Assert(transform.Find("Model"));
        model = transform.FindChild("Model");
        distance = -cam.transform.localPosition.z;
        height = cam.transform.localPosition.y;
        xRot = cam.transform.localRotation.x;
        groundedTimeout = -1000;
        airControlTimeout = -1000;
        animator = model.GetComponent<Animator>();
        dust = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        //Get move inputs and scale the move speed by the axis values
        //dx = Input.GetAxisRaw("Move Vertical " + (player)) * playerControl;
        dx = InputManager.GetAxis("Vertical", player) * playerControl;
        //dz = Input.GetAxisRaw("Move Horizontal " + (player)) * playerControl;
        dz = InputManager.GetAxis("Horizontal", player) * playerControl;
        //Project camera direction onto xz-plane
        camForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        //Calculate the motion direction vector and scale it by the moveForce
        spd = Vector3.Normalize(dx * camForward + dz * cam.transform.right) * moveForce * playerControl;
        //Create velocity vector and scale it by the speed
        desiredVelocity = (spd) + new Vector3(0, body.velocity.y, 0);
        //Set the velocity to the new velocity
        addVec = desiredVelocity - body.velocity;
        float mag = addVec.magnitude;
        if (this.isOnGround())
            mag = Mathf.Min(mag, moveForce) * playerControl;
        else
        {
            if (this.airControlTimeout < Time.fixedTime)
                mag = Mathf.Min(mag, moveForce * 0.2f) * playerControl;
            else
                mag = 0;
        }
        addVec = addVec.normalized * mag;
        body.velocity += addVec;
        if (this.isOnGround())
            body.velocity += new Vector3(0, -this.groundStickForce, 0);

        transform.forward = Vector3.Lerp(transform.forward, (dx * camForward + dz * cam.transform.right).normalized, .4f);

        //Modify states depending on whether model is moving
        animator.SetBool("run", body.velocity.magnitude > 5);
        DustParticles();
    }

    void Update()
    {
        //Get inputs from the camera inputs and the hit button
        x += InputManager.GetAxis("LookHorizontal", player) * lookSpeed * distance * 0.02f * playerControl;
        //var trig = Input.GetAxis("Hit " + (player));

        //Camera collision
        RaycastHit hit;
        if (Physics.Raycast(model.position,
            (cam.transform.position - model.position).normalized, out hit, distance)
            && hit.transform.tag != "Sheep" && hit.transform.tag != "Player")
        {
            //cam.transform.position = hit.point /*+ new Vector3(0, 1f, 0)*/;
            //distance = cam.transform.position.z - hit.point.z;
        } else
        {
            distance = 5.8f;
        }
        //Calculate the rotation using Euler angles,
        //get distance from player, calculate camera position
        rotation = Quaternion.Euler(xRot, x, 0);
        negDistance = new Vector3(0.0f, height, -distance);
        position = rotation * negDistance + model.position;

        //Set the camera's new rotation and position
        cam.transform.rotation = rotation;
        cam.transform.position = position;
        //Fix for camera on player 2 facing wrong direction
        //cam.transform.localRotation = rotation;
        //cam.transform.localPosition = position;
        //Player pushes sheep if hit button is pressed
        //if (trig > 0)
        //    hitSheep();
    }

    public void setPlayerNum(PlayerID player)
    {
        this.player = player;
    }

    /// <summary>Set whether the players can move or not</summary>
    /// <param name="i">1 for movement, 0 for no movement</param>
    public void setPlayerControl(int i)
    {
        playerControl = i;
    }

    /// <summary>Get whether the players can move or not</summary>
    /// <param name="i">1 for movement, 0 for no movement</param>
    public int GetPlayerControl()
    {
       return playerControl;
    }

    /// <summary>
    /// Use when launching the player into the air to cancel forces adhering them to the ground
    /// </summary>
    public void setGroundedTimeout()
    {
        this.groundedTimeout = Time.fixedTime + 0.6f;
        this.airControlTimeout = Time.fixedTime + 2.0f;
    }

    /// <summary>
    /// Call when the player is hitting a sheep
    /// </summary>
    void hitSheep()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, model.forward, out hit, 5))
        {
            if (hit.transform.gameObject.tag == "Sheep")
                hit.transform.gameObject.GetComponent<Sheep>().changeVelocity(model.forward * 20);
        }
    }

    bool isOnGround()
    {
        if (this.groundedTimeout > Time.fixedTime)
            return false;
        RaycastHit dontcare = new RaycastHit(); //Doesn't seem to be possible to skip this parameter in the collider raycast
        bool hit = Physics.Raycast(new Ray(this.transform.position, new Vector3(0, -1, 0)), out dontcare, isGroundedDist);//this.GetComponent<CapsuleCollider>().height + 0.4f);
        //Debug.DrawRay(this.transform.position, new Vector3(0, -isGroundedDist, 0));
        return hit;
    }

    void DustParticles()
    {
        
        if (body.velocity.magnitude > 5)
        {
            dust.enableEmission = true;
        } else
        {
            dust.enableEmission = false;
        }
    }
}
