using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Walking/Sprinting")]
    int velocityX = Animator.StringToHash("Velocity X");
    int velocityZ = Animator.StringToHash("Velocity Z");
    int isRunning = Animator.StringToHash("isRunning");
    int isMoving = Animator.StringToHash("isMoving");
    float vInput;
    float hInput;
    Vector3 zMovement;
    Vector3 xMovement;
    float speed;
    bool sprinting;
    bool idle;
    public Stamina stamina;
    public Health health;
    public Mana mana;
    int injured = Animator.StringToHash("injured");
    int dead = Animator.StringToHash("dead");
    bool hasMovementComponents;

    [Header("Jumping")]
    const int Ground = 3;
    int isJumping = Animator.StringToHash("isJumping");

    [Header("Components")]
    Rigidbody rb;
    Animator anim;

    [Header("Spells")]
    int fire = Animator.StringToHash("fire");
    int ice = Animator.StringToHash("ice");
    int storm = Animator.StringToHash("storm");
    bool hasSpells;

    [Header ("Extra")]
    bool animSetup;
    bool checkComponents;
    CapsuleCollider capsule;
    CharacterController controller;
    NavMeshObstacle obstacle;
    DayNightCycle cycle;
    [SerializeField] GameObject fireball;
    [SerializeField] GameObject iceball;
    [SerializeField] GameObject healthOrb;
    [System.NonSerialized] public GameObject iceballInstantiate;
    [System.NonSerialized] public GameObject fireballInstantiate;
    public static Player instance;

    #region Start Functions
    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        controller = GetComponent<CharacterController>();
        obstacle = GetComponent<NavMeshObstacle>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.drag = 1;
        rb.angularDrag = 0.3f;

        
        hasSpells = hasParam("fire", anim) && hasParam("ice", anim) && hasParam("storm", anim);
        hasMovementComponents = hasParam("Velocity X", anim) && hasParam("Velocity Z", anim) && hasParam("isRunning", anim);
        animSetup = anim.isHuman && anim.isActiveAndEnabled && anim.isInitialized && anim.isOptimizable;
        checkComponents = stamina != null && health != null && mana != null && (rb != null ^ controller != null) && this.gameObject != null && capsule != null && obstacle != null;
    }

    void Start()
    {
        if (animSetup && checkComponents)
        {
            DontDestroyOnLoad(gameObject);
            checkTag();
        }
        else
        {
            throw new MissingComponentException("Animator and components are not setup.");
        }
    }
    #endregion

    #region Movement with Animations
    void Update()
    {
        playerWounded();

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.PAUSE);
        }

        if (Input.GetKeyDown(KeyCode.B)) 
        {
            health.damage(110);
        }
    }

    bool playerMove() 
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("spells"))
        {
            return false;
        }
        else
        {
            return true;
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

    void checkTag() 
    {
        if (tagExists("Player")) 
        {
            if (this.gameObject.tag != "Player") 
            {
                this.gameObject.tag = "Player";
            }
        }
        else 
        {
            throw new UnityException("Player tag does not exist.");
        }
    }

    void movement() 
    {
        if (playerMove() && hasMovementComponents) 
        {
            hInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");
            sprinting = Input.GetButton("Sprint");
            idle = hInput == 0 && vInput == 0;
            playerSprinting();
            spells();

            zMovement = Vector3.forward * vInput * speed;
            xMovement = Vector3.right * hInput * speed;
        }
        else
        {
            return;
        }
    }

    void playerWounded() 
    {
        if (hasParam("injured", anim) && hasParam("dead", anim)) 
        {
            switch (Health.instance.playerStatus()) 
            {
                case Health.well:
                    anim.SetBool(injured, false);
                    movement();
                    break;
                case Health.hurt:
                    anim.SetBool(injured, true);
                    movement();
                    break;
                case Health.dead:
                    anim.SetTrigger(dead);
                    StartCoroutine(loadLoseScene());
                    break;
                default:
                    Debug.LogError("There was a problem with the playerStatus function.");
                    Application.Quit();
                    break;
            }
        }
        else 
        {
            throw new MissingReferenceException("Both animator parameters were not found.");
        }
    }

    IEnumerator loadLoseScene() 
    {
        yield return new WaitForSeconds(2.5f);
        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.LOSE);
    }

    void spells()
    {
        if (idle && hasSpells)
        {
            if ((Input.GetKeyDown(KeyCode.Alpha1) ^ Input.GetKeyDown(KeyCode.Keypad1)) && (mana.manaBar.value >= 10))
            {
                anim.SetTrigger(fire);
                Mana.instance.mana(20);
                if (fireball != null) 
                {

                    float fireballSpeed = 20f;
                    Vector3 fireballPosition = transform.position + transform.forward;
                    fireballPosition.y = transform.position.y + 0.8f;
                    fireballInstantiate = Instantiate(fireball, fireballPosition, transform.rotation);

                    Rigidbody fireballRigidbody = fireballInstantiate.GetComponent<Rigidbody>();

                    if (fireballRigidbody == null) 
                    {
                        fireballRigidbody = fireballInstantiate.AddComponent<Rigidbody>();
                    }

                    fireballRigidbody.useGravity = false;

                    SphereCollider collider = fireballInstantiate.GetComponent<SphereCollider>();

                    if (collider == null) 
                    {
                        collider = fireballInstantiate.AddComponent<SphereCollider>();
                    } 

                    collider.isTrigger = true;
                    collider.radius = 0.8f;

                    fireballRigidbody.velocity = transform.forward * fireballSpeed;

                    Destroy(fireballInstantiate, 3f);

                }
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha2) ^ Input.GetKeyDown(KeyCode.Keypad2)) && (mana.manaBar.value >= 5))
            {
                anim.SetTrigger(ice);
                Mana.instance.mana(15);
                if (iceball != null) 
                {
                    float iceballSpeed = 10f;
                    Vector3 iceballPosition = transform.position + transform.forward;
                    iceballPosition.y = transform.position.y + 0.5f;
                    iceballInstantiate = Instantiate(iceball, iceballPosition, transform.rotation);

                    Rigidbody iceballRigidbody = iceballInstantiate.GetComponent<Rigidbody>();

                    if (iceballRigidbody == null) 
                    {
                        iceballRigidbody = iceballInstantiate.AddComponent<Rigidbody>();
                    }

                    iceballRigidbody.useGravity = false;

                    SphereCollider collider = iceballInstantiate.GetComponent<SphereCollider>();
                    
                    if (collider == null) 
                    {
                        collider = iceballInstantiate.AddComponent<SphereCollider>();
                    }

                    collider.isTrigger = true;
                    collider.radius = 0.5f;

                    iceballRigidbody.velocity = transform.forward * iceballSpeed;

                    Destroy(iceballInstantiate, 3f);
                }
            }
            // else if ((Input.GetKeyDown(KeyCode.Alpha3) ^ Input.GetKeyDown(KeyCode.Keypad3)) && (mana.manaBar.value >= 15))
            // {
            //     anim.SetTrigger(storm);
            //     Mana.instance.mana(15);
            // }
            else 
            {
                return;
            }
        }
    }

    void playerSprinting()
    {
        if (!idle && sprinting && stamina.staminaBar.value >= 1)
        {
            anim.SetBool(isRunning, true);
            speed = 3;
            Stamina.instance.stamina(0.08f);
        }
        else if (!idle && !sprinting ^ (!idle && sprinting && stamina.staminaBar.value < 1))
        {
            anim.SetBool(isRunning, false);
            speed = 1.5f;
        }
        else 
        {
            speed = 0;
        }
    }

    void FixedUpdate()
    {
        var turnSpeed = 90f;
    
        transform.Translate(zMovement * Time.fixedDeltaTime);
        transform.Translate(xMovement * Time.fixedDeltaTime);

        if (vInput >= 0)
        {
            transform.Rotate(Vector3.up, hInput * Time.fixedDeltaTime * turnSpeed);
        }

        anim.SetFloat(velocityX, xMovement.x);
        anim.SetFloat(velocityZ, zMovement.z);
        
    }
    #endregion
}
