using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthManaStaminaManager : MonoBehaviour
{
    [Header ("Health")]
    public Slider healthBar;
    const int maxHealth = 200;
    const int minHealth = 0;
    int currentHealth;
    public const int well = 1;
    public const int hurt = 2;
    public const int dead = 3;

    [Header ("Mana")]
    public Slider manaBar;
    const int maxMana = 120;
    const int minMana = 0;
    int currentMana;
    Coroutine manaRegen;

    [Header ("Stamina")]
    public Slider staminaBar;
    Coroutine staminaRegen;
    const int maxStamina = 150;
    const int minStamina = 0;
    float currentStamina;

    [Header ("Extra")]
    public static HealthManaStaminaManager playerStats;
    bool checkSliders;
    [SerializeField] Canvas healthManaStamina;

    #region Setup
    void Awake() 
    {
        if (playerStats == null) 
        {
            playerStats = this;
            //DontDestroyOnLoad(gameObject);
        }
        else 
        {
            gameObject.SetActive(false);
        }

        checkSliders = healthBar != null && manaBar != null && staminaBar != null;
    }

    void Start() 
    {
        if (checkSliders && healthManaStamina != null) 
        {
            setupHealth();
            setupMana();
            setupStamina();
        }
        else 
        {
            throw new MissingComponentException("At least one of the sliders is null.");
        }
    }

    void setupHealth() 
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthBar.minValue = minHealth;
    }

    void setupMana() 
    {
        currentMana = maxMana;
        manaBar.maxValue = maxMana;
        manaBar.value = maxMana;
        manaBar.minValue = minMana;
    }

    void setupStamina() 
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        staminaBar.minValue = minStamina;
    }
    #endregion

    #region Handle Health
    public void damage(int healthAmount) 
    {
        if (healthAmount < 0) 
        {
            throw new System.Exception("Amount for health cannot be negative.");
        }


        if (currentHealth - healthAmount >= 0) 
        {
            currentHealth -= healthAmount;

            if (currentHealth <= 0) 
            {
                currentHealth = 0;
            }

            healthBar.value = currentHealth;
        }
        else 
        {
            return;
        }
    }

    public void increaseHealth() 
    {
        if (currentHealth < maxHealth) 
        {
            currentHealth += maxHealth / 80;
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

    public int getCurrentHealth() 
    {
        return currentHealth;
    }
    #endregion

    #region Handle Mana
    public void mana(int manaAmount) 
    {
        if (manaAmount < 0) 
        {
            throw new System.Exception("Amount for mana cannot be negative.");
        }

        if (currentMana - manaAmount >= 0) 
        {
            currentMana -= manaAmount;

            if (currentMana <= 0) 
            {
                currentMana = 0;
            }

            manaBar.value = currentMana;

            if (manaRegen != null) 
            {
                StopCoroutine(manaRegen);
            }

            manaRegen = StartCoroutine(increaseMana());
        }
        else 
        {
            return;
        }
    }

    IEnumerator increaseMana() 
    {
        yield return new WaitForSeconds(3.5f);

        while (currentMana < maxMana) 
        {
            currentMana += maxMana / 100;
            manaBar.value = currentMana;
            yield return new WaitForSeconds(1f);
        }
        
        manaRegen = null;
    }

    public int getCurrentMana() 
    {
        return currentMana;
    }
    #endregion

    #region Handle Stamina
    public void stamina(float staminaAmount) 
    {
        if (staminaAmount < 0) 
        {
            throw new System.Exception("Amount for stamina cannot be negative.");
        }

        if (currentStamina - staminaAmount >= 0) 
        {
            currentStamina -= staminaAmount;

            if (currentStamina <= 0) 
            {
                currentStamina = 0;
            }

            staminaBar.value = currentStamina;

            if (staminaRegen != null) 
            {
                StopCoroutine(staminaRegen);
            }

            staminaRegen = StartCoroutine(increaseStamina());
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
            yield return new WaitForSeconds(0.5f);
        }

        staminaRegen = null;
    }

    public float getCurrentStamina() 
    {
        return currentStamina;
    }
    #endregion
}
