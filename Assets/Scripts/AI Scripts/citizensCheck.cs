using UnityEngine;
using UnityEngine.AI;

public class citizensCheck : MonoBehaviour
{
    NavMeshObstacle obstacle;
    Rigidbody rb;
    CapsuleCollider capsule;
    Animator anim;
    bool check;
    
    
    void Awake() 
    {
        obstacle = GetComponent<NavMeshObstacle>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        check = obstacle != null && rb != null && anim != null && capsule != null && this.gameObject != null;
    }

    void Start() 
    {
        if (check) 
        {
            return;
        }
        else 
        {
            throw new UnityException("Components are not setup for the citizens.");
        }
    }
}
