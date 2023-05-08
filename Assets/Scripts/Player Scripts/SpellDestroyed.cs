using UnityEngine;

public class SpellDestroyed : MonoBehaviour
{
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.layer != gameObject.layer) 
        {
            Destroy(gameObject);
        }
    }
}
