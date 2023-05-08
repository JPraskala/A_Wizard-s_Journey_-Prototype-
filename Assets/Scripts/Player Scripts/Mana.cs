using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Mana : MonoBehaviour
{
    public Slider manaBar;
    const int maxMana = 120;
    const int minMana = 0;
    int currentMana;
    Coroutine regen;
    public static Mana instance;

    void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
        }
    }

    void Start() 
    {
        currentMana = maxMana;
        manaBar.maxValue = maxMana;
        manaBar.value = maxMana;
        manaBar.minValue = minMana;
    }

    public void mana(int amount) 
    {
        if (amount < 0) 
        {
            throw new System.Exception("Amount cannot be negative.");
        }

        if (currentMana - amount >= 0) 
        {
            currentMana -= amount;
            
            if (currentMana <= 0) 
            {
                currentMana = 0;
            }

            manaBar.value = currentMana;

            if (regen != null) 
            {
                StopCoroutine(regen);
            }

            regen = StartCoroutine(increaseMana());
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
            yield return new WaitForSeconds(1);
        }

        regen = null;
    }

    public int getCurrentMana() 
    {
        return currentMana;
    }
}
