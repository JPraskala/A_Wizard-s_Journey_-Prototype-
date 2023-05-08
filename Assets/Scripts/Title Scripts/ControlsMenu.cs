using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] Button returnButton;
    [SerializeField] Image panel;

    void Start() 
    {
        if (returnButton != null && panel != null) 
        {
            returnButton.onClick.AddListener(returnButtonFunction);
        }
        else 
        {
            throw new MissingComponentException("Controls canvas is not setup.");
        } 
    }


    void returnButtonFunction() 
    {
        canvasController.canvases.switchCanvases(canvasController.canvases.canvasTitle, canvasController.canvases.canvasControls);
    }
}
