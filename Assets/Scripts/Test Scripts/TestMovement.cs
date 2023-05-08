using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [Header ("Animations")]
    Animator anim;
    int speed;
    float turnSpeed;
    int isRunning = Animator.StringToHash("isRunning");
    int animatorSpeed = Animator.StringToHash("Speed");

    [Header ("Constants")]
    const int walkingSpeed = 2;
    const int runningSpeed = 4;

    void Start() 
    {
        anim = GetComponent<Animator>();
        

        if (anim.isHuman == false || !anim.isActiveAndEnabled) 
        {
           Debug.LogError("Something went wrong");  
           Application.Quit();
        }
        else 
        {
            turnSpeed = 90f;
            Update();
        }
    }

    void Update() 
    {
        switch (Input.GetButton("Sprint")) 
        {
            case true:
                speed = runningSpeed;
                anim.SetBool(isRunning, true);
                break;
            default:
                speed = walkingSpeed;
                anim.SetBool(isRunning, false);
                break;
        }

        // Walking
        Vector3 velocity = Vector3.forward * Input.GetAxis("Vertical") * speed;
        transform.Translate(velocity * Time.deltaTime);
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed);
        anim.SetFloat(animatorSpeed, velocity.z);

    }
}