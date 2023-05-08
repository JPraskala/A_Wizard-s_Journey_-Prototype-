using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stamina : MonoBehaviour
{
    public Slider staminaBar;
    WaitForSeconds increaseClock = new WaitForSeconds(0.5f);
    Coroutine regen;
    const int maxStamina = 150;
    const int minStamina = 0;
    float currentStamina;
    public static Stamina instance;

    void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
        }
    }

    void Start() 
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        staminaBar.minValue = 0;
    }

    public void stamina(float amount) 
    {
        if (amount < 0) 
        {
            throw new System.Exception("Amount cannot be negative.");
        }

        if (currentStamina - amount >= 0) 
        {
            currentStamina -= amount;

            if (currentStamina <= 0) 
            {
                currentStamina = 0;
            }

            staminaBar.value = currentStamina;

            if (regen != null) 
            {
                StopCoroutine(regen);
            }
            

            regen = StartCoroutine(increaseStamina());
        }
        else 
        {
            return;
        }
    }


    IEnumerator increaseStamina() 
    {
        yield return new WaitForSeconds(5.5f);

        while (currentStamina < maxStamina) 
        {
            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;
            yield return increaseClock;
        }

        regen = null;
    }
    
    public float getCurrentStamina() 
    {
        return currentStamina;
    }
}
