using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    void Update()
    {
        float range = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, range);
        foreach(Collider collider in colliderArray) 
        {
            if (collider.TryGetComponent(out NPCInteractable nPCInteractable)) 
            {
                if (Input.GetKeyDown(KeyCode.E)) 
                {
                    nPCInteractable.interact();
                }
                else if (Input.GetKeyDown(KeyCode.Escape)) 
                {
                    nPCInteractable.reset();
                }
                else 
                {
                    return;
                }
            }
        }
    }
}
