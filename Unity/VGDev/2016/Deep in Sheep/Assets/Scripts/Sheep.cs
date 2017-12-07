using UnityEngine;

public class Sheep : MonoBehaviour
{

    public int points;
    public float radius = 6;
    private float range = 5;
    Animation anim;
    Rigidbody body;
    AudioSource audioSource;
    Agent agent;
    Vector3 corralLoc, runDirection;
    State state;
    int chance;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        state = State.GRAZE;
        //oldState = State.GRAZE;
        chance = Random.Range(1, 8);
        chance = 100 - chance;
        agent = GetComponent<Agent>();
        anim = GetComponentInChildren<Animation>();
    }

    void FixedUpdate()
    {
        //Check if the sheep is not upright and is not moving
        //Then lerp it back upright it and set any remainging velocities to zero
        if (state != State.CORRALLED)
        {
            bool playerNear = IsPlayerNear();
            if (body.velocity.magnitude < 2f && transform.up != Vector3.up && state == State.HIT)
            {
                ReErect();
                if (playerNear)
                {
                    state = State.RUN;
                    updateState();
                }
                else {
                    state = State.GRAZE;
                }
            }
            if (state != State.HIT)
            {
                if (state == State.WANDER)
                {
                    anim.Play("SheepJump");
                }
                else if (state == State.RUN)
                {
                    anim.Play("SheepRun");
                }
                if (playerNear)
                {
                    agent.getAgent().ResetPath();
                    state = State.RUN;
                    updateState();
                }
                else if (state == State.RUN && !agent.getAgent().hasPath)
                {
                    //agent.getAgent().ResetPath();
                    state = State.GRAZE;
                    updateState();
                }
                else if (!agent.getAgent().hasPath)
                {
                    updateState();
                }
                else if (Random.Range(0, 1000) > 998)
                {
                    baa();
                }
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (state != State.CORRALLED)
        {
            if (col.gameObject.tag == "Player")
            {
                GetComponent<NavMeshAgent>().enabled = false;
                state = State.HIT;
                updateState();
            }
        }
    }

    public void SetCorralled()
    {
        state = State.CORRALLED;
        //tag = "CorralledSheep";
        //corralLoc = transform.localPosition;
        ReErect();
        updateState();
    }

    public void toggleSheepMovement()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }

    public void changeVelocity(Vector3 velocity)
    {
        state = State.HIT;
        body.velocity = velocity;
        updateState();
    }

    void ReErect()
    {
        transform.up = Vector3.Lerp(transform.up, Vector3.up, .25f);
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    void baa()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.pitch = (float)0.1 * Random.Range(10, 16);
            audioSource.Play();
        }
    }

    void updateState()
    {
        switch (state)
        {
            case State.GRAZE:
                //Set the jumping animation to play only once more
                anim.GetClip("SheepJump").wrapMode = WrapMode.Once;
                //Re-enable the nav mesh agent in case it had been disabled by being hit
                GetComponent<NavMeshAgent>().enabled = true;
                agent.getAgent().speed = 3.5f;
                agent.getAgent().angularSpeed = 120;
                //Chance to change to wander
                //Debug.Log("is grazing");
                if (IsPlayerNear())
                {
                    state = State.RUN;
                }
                else if (Random.Range(1, 100) > chance)
                {
                    //Debug.Log("goes to wander");
                    state = State.WANDER;
                }
                break;
            case State.WANDER:
                //Set the jump animation to loop
                anim.GetClip("SheepJump").wrapMode = WrapMode.Loop;
                anim.Play("SheepJump");
                //Re-enable the nav mesh agent in case it had been disabled by being hit
                GetComponent<NavMeshAgent>().enabled = true;
                agent.getAgent().speed = 3.5f;
                agent.getAgent().angularSpeed = 120;
                //Chance to change back to grazing
                if (IsPlayerNear())
                {
                    state = State.RUN;
                    break;
                }
                else if (Random.Range(1, 100) > (chance / 2))
                {
                    state = State.GRAZE;
                }
                agent.moveToLocation(randomLocation());
                break;
            case State.HIT:
                //Set the jumping animation to play only once more
                anim.GetClip("SheepJump").wrapMode = WrapMode.Once;
                agent.getAgent().speed = 3.5f;
                agent.getAgent().angularSpeed = 120;
                //Debug.Log("Sheep was hit");
                GetComponent<NavMeshAgent>().enabled = false;
                break;
            case State.RUN:
                GetComponent<NavMeshAgent>().enabled = true;
                anim.Play("SheepRun");
                agent.getAgent().speed = 5;
                agent.getAgent().angularSpeed = 120;
                runFromPlayer();
                break;
            case State.CORRALLED:
                anim.Stop();
                GetComponent<NavMeshAgent>().enabled = false;
                //ReErect();
                GetComponent<Rigidbody>().mass = 5000;
                //transform.localPosition = corralLoc;
                break;
            default:
                //Re-enable the nav mesh agent in case it had been disabled by being hit
                GetComponent<NavMeshAgent>().enabled = true;
                agent.getAgent().speed = 3.5f;
                agent.getAgent().angularSpeed = 120;
                //Make sheep graze
                state = State.GRAZE;
                break;
        }
    }

    // Checks to see if a player is nearby
    bool IsPlayerNear()
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            Vector3 temp = (transform.position - p.transform.position);
            if (temp.magnitude <= 4)
            {
                runDirection = temp.normalized;
                return true;
            }
        }
        return false;
    }

    // Finds a point directly away from the player and directs the sheep to it
    public void runFromPlayer()
    {
        Vector3 temp = (runDirection * 15);
        Vector3 destination = new Vector3(transform.position.x + temp.x, transform.position.y, transform.position.z + temp.z);
        agent.moveToLocation(destination);
        Debug.Log("running from player");
    }

    // Find a random location close to the sheep's original location for the sheep to move to
    public Vector3 randomLocation()
    {
        Vector3 destination = new Vector3(transform.position.x + Random.Range(-range, range), transform.position.y, transform.position.z + Random.Range(-range, range));
        return destination;
    }

    private enum State { WANDER, GRAZE, RUN, HIT, CORRALLED };
}