using UnityEngine;

public class DestroyHealth : MonoBehaviour
{
    void OnTriggerEnter(Collider other) 
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0) 
        {
            float playerHealth = Health.instance.getCurrentHealth();

            if (other.gameObject.CompareTag("Player") && (playerHealth > 0 && playerHealth < 200)) 
            {
                Destroy(gameObject);
                Health.instance.increaseHealth(playerHealth);
            }
            else 
            {
                return;
            } 
        }
        else 
        {
            Debug.LogWarning("Player tag does not exist.");
        }
    }
}
