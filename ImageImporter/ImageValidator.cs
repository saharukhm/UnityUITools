using UnityEditor;
using UnityEngine;

public static class ImageValidator
{
    public static bool ValidateTexture(string filePath, int maxWidth, int maxHeight, bool checkNPOT, out string feedback)
    {
        feedback = string.Empty;
        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(System.IO.File.ReadAllBytes(filePath)))
        {
            feedback = "Failed to load the image.";
            return false;
        }
        if (checkNPOT && (Mathf.IsPowerOfTwo(texture.width) == false || Mathf.IsPowerOfTwo(texture.height) == false))
        {
            feedback = "Texture is NPOT (Non-Power-Of-Two).";
            return false;
        }
        if (texture.width > maxWidth || texture.height > maxHeight)
        {
            feedback = $"Texture resolution exceeds the limit of {maxWidth}x{maxHeight}.";
            return false;
        }
        return true;
    }
}
