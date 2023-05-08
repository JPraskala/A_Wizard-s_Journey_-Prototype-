using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] Button start;
    [SerializeField] Button controls;
    [SerializeField] Button credits;
    [SerializeField] Button quit;
    bool checkComponents;

    void Awake() 
    {
        checkComponents = panel != null && start != null && controls != null && credits != null && quit != null && this.gameObject != null;
    }

    void Start() 
    {
        if (!checkComponents) 
        {
            throw new MissingComponentException("Missing components for title screen.");
        }
        else 
        {
            start.onClick.AddListener(startButton);
            controls.onClick.AddListener(controlsButton);
            credits.onClick.AddListener(creditsButton);
            quit.onClick.AddListener(quitButton);
        }
    }

    void startButton() 
    {
        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.MAIN);
    }

    void controlsButton() 
    {
        canvasController.canvases.switchCanvases(canvasController.canvases.canvasControls, canvasController.canvases.canvasTitle);
    }

    void creditsButton() 
    {
        GameManager.gameManagerInstance.sceneManager(GameManager.myScenes.CREDITS);
    }

    void quitButton() 
    {
        Application.Quit();
    }
}
