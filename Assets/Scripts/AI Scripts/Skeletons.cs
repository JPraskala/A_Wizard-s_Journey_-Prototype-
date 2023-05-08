using UnityEngine;
using UnityEngine.AI;

public class Skeletons : MonoBehaviour
{
    enum states 
    {
        Idle,
        Running,
        Attacking,
        Fleeing
    }

    Rigidbody rb;
    Animator anim;
    CapsuleCollider capusle;
    bool animSetup;
    bool checkComponents;
    bool checkAnimatorParamters;
    bool shouldFlee;
    float fleeRange;
    int canPunch = Animator.StringToHash("canPunch");
    int VelX = Animator.StringToHash("VelX");
    int VelZ = Animator.StringToHash("VelZ");
    int Idle = Animator.StringToHash("Idle");
    [SerializeField] Transform target;
    NavMeshAgent agent;
    float timeSinceAttack;
    float attackDelay;
    float attackRange;
    states state;

    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        capusle = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        animSetup = anim.isHuman && anim.isInitialized && anim.isActiveAndEnabled && anim.isOptimizable;
        checkComponents = rb != null && anim != null && capusle != null && this.gameObject != null && target != null && agent != null;
        checkAnimatorParamters = hasParam("canPunch", anim) && hasParam("VelX", anim) && hasParam("VelZ", anim) && hasParam("Idle", anim);
    }

    void Start() 
    {
        if (animSetup && checkComponents && checkAnimatorParamters) 
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.detectCollisions = true;

            if (tagExists("Skeletons")) 
            {
                if (this.gameObject.tag != "Skeletons") 
                {
                this.gameObject.tag = "Skeletons";
                }
            }
            else 
            {
                throw new MissingReferenceException("Skeletons tag does not exist.");
            }
            attackDelay = 1.5f;
            attackRange = 1.2f;
            shouldFlee = false;
            fleeRange = 1f;
            agent.avoidancePriority = 100;
        }
        else 
        {
            throw new MissingComponentException("Components are not all setup in skeleton.");
        }
    }

    void Update() 
    {
        skeletonController();
    }

    void skeletonController() 
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (Mathf.Approximately(distance, 0f)) 
        {
            throw new System.Exception("Distance cannot be zero.");
        }
        else if (distance <= 0.4f) 
        {
            state = states.Attacking;
            print("Attacking");
        }
        else if (shouldFlee && distance < fleeRange) 
        {
            state = states.Fleeing;
            print("Fleeing");
        }
        else if (distance >= 5.5f) 
        {
            state = states.Idle;
            anim.SetBool(Idle, true);
        }
        else if (distance > 1f && distance < 5.5f) 
        {
            state = states.Running;
            anim.SetBool(Idle, false);
            print("Running");
        }
        else 
        {
            return;
        }

        print(distance);

        decideState();
    }

    void decideState() 
    {
        
        switch (state) 
        {
            case states.Idle:
                if (agent.isOnNavMesh) 
                {
                    agent.isStopped = true;
                }
                break;
            case states.Running:
                Vector3 targetPosition = target.position;
                targetPosition.y = transform.position.y;
                agent.SetDestination(targetPosition);
                Vector3 agentVelocity = agent.velocity.normalized;
                anim.SetFloat(VelX, agentVelocity.x);
                anim.SetFloat(VelZ, agentVelocity.z);
                transform.LookAt(targetPosition);
                if (agent.isOnNavMesh) 
                {
                    agent.isStopped = false;
                }
                timeSinceAttack = 0f;
                break;
            case states.Attacking:
                float rotationSpeed = 1.5f;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

                if (Vector3.Distance(transform.position, target.position) <= attackRange) 
                {
                    if (timeSinceAttack >= attackDelay) 
                    {
                        anim.SetTrigger(canPunch);
                        timeSinceAttack = 0f;
                    }
                    else 
                    {
                        anim.ResetTrigger(canPunch);
                        timeSinceAttack += Time.deltaTime;
                    }
                }
                else 
                {
                    state = states.Running;
                    timeSinceAttack = 0f;
                    anim.ResetTrigger(canPunch);
                }
                break;
            case states.Fleeing:
                float fleeDistance = 1.3f;
                Vector3 fleeDirection = (transform.position - target.position).normalized;
                Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;
                if (agent.isOnNavMesh) 
                {
                    agent.SetDestination(fleePosition);
                    agent.isStopped = false;
                }
                anim.SetBool(Idle, false);
                break;
            default:
                throw new System.Exception("Invalid state.");
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

    static bool tagExists(string tag) 
    {
        return GameObject.FindGameObjectsWithTag(tag).Length > 0;
    }
}
