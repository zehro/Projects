using UnityEngine;
using System.Collections;
using Assets.Scripts.Arrows;
using Assets.Scripts.Player;

public class Grapple : MonoBehaviour {

    private float distance;

    private GameObject ground;
    private Vector3 groundOffset;

    private LineRenderer line;

	// Use this for initialization
	void Start () {
        Vector3 playerPos = transform.position + new Vector3(0, 1, 0);
        distance = Vector3.Distance(playerPos, Position);

        line = gameObject.AddComponent<LineRenderer>();
        line.SetWidth(0.05f, 0.05f);
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetColors(Color.red, Color.red);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (GetComponent<Rigidbody>().isKinematic)
        {
            Ungrapple();
        }

        Vector3 playerPos = transform.position + new Vector3(0, 1, 0);
        if (Vector3.Distance(playerPos, Position) > distance)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            GetComponent<Rigidbody>().AddForce(Position - playerPos, ForceMode.VelocityChange);
            rigidbody.velocity /= 2;
            if (rigidbody.velocity.magnitude > 8)
            {
                rigidbody.velocity *= 8 / rigidbody.velocity.magnitude;
            }
            //velocity -= Vector3.Project(velocity, playerPos - Position);

            //Vector3 unitPos = (playerPos - Position) / (playerPos - Position).magnitude;
            //transform.position = Position + unitPos * distance - new Vector3(0, 1, 0);
        }

        UpdateLine();
    }

    void UpdateLine()
    {
        Vector3[] linePoints = new Vector3[2];
        linePoints[0] = transform.position + new Vector3(0, 1, 0);
        linePoints[1] = Position;
        line.SetPositions(linePoints);
    }

    public void Ungrapple()
    {
        GetComponent<Controller>().ParkourComponent.Grappling = false;
        Destroy(GetComponent<LineRenderer>());
        Destroy(this);
    }

    #region C# Properties
    /// <summary>
    /// Position of the end of the grapple
    /// </summary>
    public Vector3 Position
    {
        get { return ground.transform.position + groundOffset; }
        set { groundOffset = value; }
    }

    /// <summary>
    /// The ground that this grapple is attached to
    /// </summary>
    public GameObject Ground
    {
        get { return ground; }
        set { ground = value; }
    }
    #endregion
}
