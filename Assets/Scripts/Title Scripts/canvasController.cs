using UnityEngine;

public class canvasController : MonoBehaviour
{
    public Canvas canvasTitle;
    public Canvas canvasControls;
    public static canvasController canvases;

    void Awake() 
    {
        if (canvases == null) 
        {
            canvases = this;
        }
    } 

    void Start() 
    {
        switchCanvases(canvasTitle, canvasControls);
    }

    public void switchCanvases(Canvas canvasToShow, Canvas canvasToHide) 
    {
        if (canvasToShow == null || canvasToHide == null) 
        {
            throw new MissingComponentException("At least one canvas is null.");
        }
        else 
        {
            canvasToShow.gameObject.SetActive(true);
            canvasToHide.gameObject.SetActive(false);
        }
    }
}
