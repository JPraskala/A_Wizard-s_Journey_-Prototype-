using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections;

public class Town_Walking : MonoBehaviour
{
    Rigidbody rb;
    CharacterController controller;
    NavMeshAgent agent;
    GameObject guard;
    Animator anim;
    bool animSetup;
    bool checkComponents;
    CapsuleCollider capsule;
    Transform[] targets;
    bool checkTargets;
    float stoppingDistance;
    bool verifyAgent;
    int targetIndex;
    int VelocityX = Animator.StringToHash("Velocity X");
    int velocityZ = Animator.StringToHash("Velocity Z");
    int Idle = Animator.StringToHash("Idle");
    int Turn = Animator.StringToHash("Turn");
    bool grounded;
    

    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        capsule = GetComponent<CapsuleCollider>();
        controller = GetComponent<CharacterController>();
        grounded = false;
        guard = GameObject.FindGameObjectWithTag("Guards");
        anim = GetComponent<Animator>();
        animSetup = anim.isHuman && anim.isActiveAndEnabled && anim.isInitialized && anim.isOptimizable;
        checkComponents = (rb != null ^ controller != null) && agent != null && guard != null && capsule != null && this.gameObject != null;


        targets = GameObject.FindGameObjectsWithTag("Target").Select(t => t.transform).ToArray();
        checkTargets = targets != null && targets.Length == 4;


        verifyAgent = agent.isActiveAndEnabled && agent.isOnNavMesh && !agent.isOnOffMeshLink;
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

    void Start() 
    {
        if (animSetup && checkComponents && checkTargets && verifyAgent) 
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.detectCollisions = true;
            targetIndex = 0;
            agent.SetDestination(targets[0].position);
        }
        else 
        {
            return;
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            grounded = true;
        }
    }

    void Update() 
    {
        if (grounded) 
        {
            if (hasParam("Velocity X", anim) && hasParam("Velocity Z", anim) && hasParam("Idle", anim) && hasParam("Turn", anim)) 
            {
                
                if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance) 
                {
                    anim.SetBool(Idle, true);
                    StartCoroutine(Wait());
                    anim.SetTrigger(Turn);
                }
                else 
                {
                    anim.SetBool(Idle, false);
                }

                if (destinationTest()) 
                {
                    anim.SetFloat(VelocityX, agent.velocity.x);
                    anim.SetFloat(velocityZ, agent.velocity.z);
                }
                else 
                {
                    Debug.LogError("Destination returned " + false);
                }
            }
            else 
            {
                Debug.LogError("All four animator parameters do not exist.");
            }
        }
        else 
        {
            return;
        }

        print(grounded);
    }

    bool destinationTest() 
    {
        return agent.SetDestination(targets[targetIndex].position);
    }

    IEnumerator Wait() 
    {
        yield return new WaitForSeconds(3f);

        targetIndex = (targetIndex + 1) % targets.Length;
        agent.SetDestination(targets[targetIndex].position);
    }
}
