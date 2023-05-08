using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class Guards : MonoBehaviour
{
    [Header ("Components")]
    NavMeshAgent agent;
    Rigidbody rb;
    Animator anim;
    CapsuleCollider capsule;

    [Header ("Extra")]
    [SerializeField] Transform[] waypoints;
    int waypointIndex;
    bool checkAnim;
    bool checkAgent;
    bool checkComponents;
    bool hasParameters;
    bool checkTags;
    bool grounded;
    NavMeshPath path;

    [Header ("Animator Parameters")]
    int VelocityX = Animator.StringToHash("Velocity X");
    int VelocityZ = Animator.StringToHash("Velocity Z");
    int Idle = Animator.StringToHash("Idle");
    int Turn = Animator.StringToHash("Turn");

    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();

        checkAnim = anim.isHuman && anim.isInitialized && anim.isOptimizable && anim.isActiveAndEnabled;
        checkTags = tagExists("Guards") && tagExists("Target");
        checkAgent = agent.isActiveAndEnabled && agent.isOnNavMesh && !agent.isOnOffMeshLink;
        checkComponents = agent != null && rb != null && anim != null && capsule != null && waypoints.Length > 0 && this.gameObject != null;
    }

    void Start() 
    {
        if (checkAnim && checkAgent && checkComponents && checkTags) 
        {
            this.gameObject.tag = "Guards";
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.detectCollisions = true;
            grounded = false;
            waypointIndex = 0;
            hasParameters = hasParam("Velocity Z", anim) && hasParam("Velocity X", anim) && hasParam("Idle", anim) && hasParam("Turn", anim);
            agent.SetDestination(waypoints[waypointIndex].position);
            createPath();
        }
        else 
        {
            throw new MissingComponentException("Anim, agent, tags, and components are all not properly setup.");
        }
    }

    static bool tagExists(string tag) 
    {
        try 
        {
            GameObject.FindGameObjectsWithTag(tag);
            return true;
        }
        catch 
        {
            return false;
        }
    }

    static bool hasParam(string paramName, Animator animator) 
    {
        foreach (AnimatorControllerParameter param in animator.parameters) 
        {
            if (param.name == paramName) 
            {
                return true;
            }
        }

        return false;
    }

    void createPath() 
    {
        if (!destinationTest()) 
        {
            throw new UnityException("Agent cannot reach destination.");
        }
        else 
        {
            path = new NavMeshPath();
            agent.CalculatePath(waypoints[waypointIndex].position, path);

            if ((path.status == NavMeshPathStatus.PathPartial) || (path.status == NavMeshPathStatus.PathInvalid)) 
            {
                throw new UnityException("Agent cannot reach destination.");
            }
            else 
            {
                agent.SetPath(path);
            }

            if (waypoints[waypointIndex].CompareTag("Target")) 
            {
                agent.isStopped = true;
            }
            else 
            {
                agent.isStopped = false;
            }
        }
    }

    void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            grounded = true;
        }
    }

    void Update() 
    {
        if (hasParameters && agent.hasPath && agent.remainingDistance > agent.stoppingDistance && grounded) 
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && waypoints[waypointIndex].CompareTag("Target")) 
            {
                anim.SetBool(Idle, true);
                StartCoroutine(turnAround());
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[waypointIndex].position);
                createPath();
            }
            else 
            {
                anim.SetBool(Idle, false);
                anim.SetFloat(VelocityX, agent.velocity.x);
                anim.SetFloat(VelocityZ, agent.velocity.z);
            }
        }
        else 
        {
            throw new UnityException("Update function cannot be executed.");
        }
    }

    IEnumerator turnAround() 
    {
        yield return new WaitForSeconds(3f);
        anim.SetTrigger(Turn);
    }

    bool destinationTest() 
    {
        return agent.SetDestination(waypoints[waypointIndex].position);
    }
}
