using UnityEditor;
using UnityEngine;

public static class TextureOptimizer
{
    public static void OptimizeTexture(string assetPath)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (textureImporter == null)
        {
            Debug.LogError("Failed to get TextureImporter for asset: " + assetPath);
            return;
        }     
        textureImporter.isReadable = true;
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (texture == null)
        {
            Debug.LogError("Failed to load texture: " + assetPath);
            return;
        }
        bool isTransparent = TextureTransparencyChecker.IsTextureTransparent(texture); 
        int maxSize = Mathf.Max(texture.width, texture.height); // Baad me fix karega
        if (maxSize <= 512)
        {
            textureImporter.maxTextureSize = 512;
        }
        else if (maxSize <= 1024)
        {
            textureImporter.maxTextureSize = 1024;
        }
        else
        {
            textureImporter.maxTextureSize = 2048;
        }

            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
            {
            name = "Android",
            maxTextureSize = textureImporter.maxTextureSize,
            overridden = true,
            format = isTransparent ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4,
            textureCompression = TextureImporterCompression.CompressedHQ
            };
        textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;   // Important hai yeah
        textureImporter.mipmapEnabled = true;
        textureImporter.SetPlatformTextureSettings(platformSettings);       
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Debug.Log($"Optimized {assetPath} for Android with max size {platformSettings.maxTextureSize} and format {platformSettings.format}");
    }
}
