using UnityEngine;
using UnityEngine.UI;

public class CrossHairSprite : MonoBehaviour
{
    [SerializeField] Texture2D crosshair;
    [SerializeField] Camera mainCamera;
    [SerializeField] Image crosshairImage;
    Sprite newCrosshair;
    bool componentsNull;

    void Awake() 
    {
        componentsNull = (crosshair == null || mainCamera == null) || crosshairImage == null;
    }

    void Start() 
    {
        if (componentsNull || this.gameObject == null) 
        {
            throw new System.Exception("CrossHair Sprite cannot be setup.");
        }
        else 
        {
            newCrosshair = CrossHair.convertTextureToSprite(crosshair);
            crosshairImage.sprite = newCrosshair;
        }
    }

    void LateUpdate() 
    {
        Vector3 centerPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 0.1f));

        if (crosshairImage.transform != null) 
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.nearClipPlane;
            Vector3 aimPosition = mainCamera.ScreenToWorldPoint(mousePos);
            crosshairImage.transform.position = Vector3.Lerp(crosshairImage.transform.position, aimPosition, Time.deltaTime * 10f);
        }
    }
}
