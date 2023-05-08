using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SkeletonsTwo : MonoBehaviour
{
    enum states
    {
        IDLE,
        RUNNING,
        PUNCHING,
        FLEEING
    }

    NavMeshAgent agent;
    Rigidbody rb;
    CapsuleCollider capsule;
    Animator anim;
    bool checkComponents;
    bool checkAnim;
    [SerializeField] Transform target;
    int VelX = Animator.StringToHash("VelX");
    int VelZ = Animator.StringToHash("VelZ");
    int Idle = Animator.StringToHash("Idle");
    int Punch = Animator.StringToHash("Punch");
    bool checkParameters;
    [SerializeField] TextMeshProUGUI skeletonCounter;
    [SerializeField] GameObject spellOne;
    [SerializeField] GameObject spellTwo;
    bool skeletonsInitalized;

    #region setup
    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        checkAnim = anim.isHuman && anim.isInitialized && anim.isActiveAndEnabled && anim.isOptimizable;
        checkComponents = agent != null && rb != null && capsule != null && anim != null && target != null && skeletonCounter != null && this.gameObject != null;
    }

    void Start() 
    {
        if (checkAnim && checkComponents) 
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
                Debug.LogWarning("Skeletons tag does not exist.");
            }
            StartCoroutine(delayUpdateSkeletons());
            checkParameters = hasParam("VelX", anim) && hasParam("VelZ", anim) && hasParam("Idle", anim) && hasParam("Punch", anim);
            skeletonsInitalized = false;
            UpdateSkeletonCounter();
        }
        else 
        {
            throw new MissingComponentException("Skeletons are not properly setup");
        }
    }
    #endregion

    #region Control Skeletons
    void Update() 
    {
           
        skeletonController();
        UpdateSkeletonCounter();
        
    }

    void skeletonController() 
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if ((Health.instance.playerStatus() == Health.well ^ Health.instance.playerStatus() == Health.hurt)) 
        {
            if (distance >= 8.5f) 
            {
                decideState(states.IDLE);
            }
            else if (distance >= 1.5f && distance < 8.5f) 
            {
                decideState(states.RUNNING);

                // if (distance >= 5.5f) 
                // {
                //     decideState(states.FLEEING);
                // }
            }
            else if (distance < 1.5f)
            {
                decideState(states.PUNCHING);
            }
            else 
            {
                decideState(states.IDLE);
            }
        }
    }

    void decideState(states currentState) 
    {
        if (checkParameters) 
        {
            switch (currentState) 
            {
                case states.IDLE:
                    anim.SetBool(Idle, true);
                    anim.SetBool(Punch, false);
                    if (agent.isOnNavMesh) 
                    {
                        agent.isStopped = true;
                    }
                    break;
                case states.RUNNING:
                    anim.SetBool(Idle, false);
                    anim.SetBool(Punch, false);
                    float rotationSpeed = 1.5f;
                    Vector3 directionToTarget = (target.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                    agent.SetDestination(target.position);
                    Vector3 agentVelocity = agent.velocity.normalized;
                    anim.SetFloat(VelX, agentVelocity.x);
                    anim.SetFloat(VelZ, agentVelocity.z);
                    if (agent.isOnNavMesh) 
                    {
                        agent.isStopped = false;
                    }
                    break;
                case states.PUNCHING:
                    anim.SetBool(Punch, true);
                    Vector3 toTarget = (target.position - transform.position).normalized;
                    float dot = Vector3.Dot(transform.forward, toTarget);
                    if (dot < 0.5f) 
                    {
                        decideState(states.RUNNING);
                    }
                    else 
                    {

                        if (agent.isOnNavMesh) 
                        {
                            agent.isStopped = true;
                        }
                    }
                    Health.instance.damage(0.2f);
                    break;
                case states.FLEEING:
                    // anim.SetBool(Idle, false);
                    // anim.SetBool(Punch, false);
                    // float fleeDistance = 4f;
                    // Vector3 fleeDirection = (transform.position - target.position).normalized;
                    // Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;
                    // if (agent.isOnNavMesh) 
                    // {
                    //     agent.SetDestination(fleePosition);
                    //     agent.isStopped = false;
                    // }

                    // if (agent.remainingDistance <= agent.stoppingDistance && agent.isOnNavMesh) 
                    // {
                    //     decideState(states.IDLE);
                    // }
                    break;
                default:
                    throw new System.Exception("Invalid state.");
            }
        }
        else 
        {
            throw new MissingComponentException("Not all animator parameters for the skeletons exist.");
        }
    }
    #endregion

    #region Additional Methods
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

    public int getNumberOfSkeletons() 
    {
        GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeletons");
        int numSkeletons = skeletons.Length;
        
        return numSkeletons;
    }

    void OnTriggerEnter(Collider collision) 
    {
        if ((spellOne != null && spellTwo != null)) 
        {
            if (collision.gameObject == Player.instance.fireballInstantiate || collision.gameObject == Player.instance.iceballInstantiate) 
            {
                Destroy(this.gameObject);
                print("hit");
            }
        }
        else 
        {
            throw new MissingComponentException("SpellOne and SpellTwo will not work correctly.");
        }
    }
    #endregion

    #region UI 
    IEnumerator delayUpdateSkeletons() 
    {
        yield return new WaitForSeconds(3f);
        skeletonsInitalized = true;
        UpdateSkeletonCounter();
    }

    void UpdateSkeletonCounter() 
    {
        int numSkeletons = getNumberOfSkeletons();
        skeletonCounter.text = "Skeletons: " + numSkeletons;

        if (numSkeletons == 0 && skeletonsInitalized) 
        {
            GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.LOSE);
        }
    }

    void OnDestroy() 
    {
        UpdateSkeletonCounter();
    }
    #endregion
}
