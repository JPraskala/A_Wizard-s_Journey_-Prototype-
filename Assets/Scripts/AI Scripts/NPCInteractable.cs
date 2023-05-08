using UnityEngine;
using System.Collections;

public class NPCInteractable : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    Vector3 checkPosition;
    float rotationDuration = 1f;
    int talking = Animator.StringToHash("talking");
    Vector3 direction;
    [SerializeField] Player character;
    bool characterExists;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
       rb.useGravity = true;
       rb.isKinematic = false;
       rb.detectCollisions = true;
       characterExists = character != null;
    }

    void Update() 
    {
        direction = character.transform.position - transform.position;
    }

    public void interact()
    {
        if (characterExists) 
        {
            StartCoroutine(Rotate());
        }
        else 
        {
            Debug.LogError("Character is null.");
            Application.Quit();
        }
    }

    IEnumerator Rotate() 
    {
        while (true) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float time = 0f;
            Quaternion startingRotation = transform.rotation;

            while (time < rotationDuration) 
            {
                transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, time / rotationDuration);

                time += Time.deltaTime;

                yield return null;
            }

            transform.rotation = targetRotation;
            anim.SetBool(talking, true);
        }
    }

    public void reset() 
    {
        print("Reset");
    }

    IEnumerator RotateOriginalPosition() 
    {
        yield return null;
    }
}
