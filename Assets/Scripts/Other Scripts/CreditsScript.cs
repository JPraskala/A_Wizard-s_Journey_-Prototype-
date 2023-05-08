using UnityEngine;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] Canvas creditsScreen;
    [SerializeField] Image panel;
    [SerializeField] Button creditsButton;

    void Start() 
    {
        if (creditsScreen != null && panel != null && creditsButton != null) 
        {
            creditsButton.onClick.AddListener(returnToTitle);
        }
        else 
        {
            throw new MissingComponentException("All components for the credits screen were not found.");
        }
    }


    void returnToTitle() 
    {
        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.TITLE);
    }
}
