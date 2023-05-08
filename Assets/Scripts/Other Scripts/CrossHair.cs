using UnityEngine;

public static class CrossHair
{
    public static Sprite convertTextureToSprite(Texture2D image) 
    {
        return Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
    }
}
