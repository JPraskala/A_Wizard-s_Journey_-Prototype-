using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider healthBar;
    const int maxHealth = 200;
    const int minHealth = 0;
    public static Health instance;
    float currentHealth;
    
    [Header ("States")]
    public const int well = 1;
    public const int hurt = 2;
    public const int dead = 3;

    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthBar.minValue = minHealth;
    }

    public void damage(float amount)
    {
        if (amount < 0) 
        {
            throw new System.Exception("Amount cannot be negative.");
        }
        else 
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }

            healthBar.value = currentHealth;
        }
    }

    public void increaseHealth(float healthAmount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healthAmount;
            if (currentHealth >= 200) 
            {
                currentHealth = 200;
            }
            healthBar.value = currentHealth;
        }
        else 
        {
            return;
        }
    }

    public int playerStatus()
    {
        float halfHealth = maxHealth * .5f;

        if (healthBar.value > halfHealth) 
        {
            return well;
        }
        else if (healthBar.value > minHealth && healthBar.value <= halfHealth) 
        {
            return hurt;
        }
        else 
        {
            return dead;
        }
    }

    public float getCurrentHealth() 
    {
        return currentHealth;
    }
}
