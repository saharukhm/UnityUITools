using UnityEngine;

public static class TextureTransparencyChecker
{
    public static bool IsTextureTransparent(Texture2D texture)
    {       
        Color[] pixels = texture.GetPixels();     
        foreach (Color pixel in pixels)
        {
            if (pixel.a < 1.0f) 
            {
                return true;
            }
        }
        return false; 
    }
}
