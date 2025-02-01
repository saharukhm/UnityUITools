using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ImageImporter : EditorWindow
{
    private int selectedAssetType = 0;
    private string[] assetTypes = new string[] { "UI Asset", "Art Asset" };

    private int selectedUIType = 0;
    private string[] uiTypes = new string[] { "Icon", "Image", "Shape" };

    private int selectedArtType = 0;
    private string[] artTypes = new string[] { "Character", "VFX", "Environment", "Food", "Prop" };

    private List<string> filePaths = new List<string>();
    private string validationFeedback = string.Empty;
    private bool isValid = false;
    private Texture2D greenTickTexture;

    [MenuItem("Tools/Custom Image Importer")]
    public static void ShowWindow()
    {
        GetWindow<ImageImporter>("Custom Image Importer");
    }

    private void OnEnable()
    {
        
        greenTickTexture = Resources.Load<Texture2D>("GreenTick");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Image Assets", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Single File"))
        {
            string singleFilePath = EditorUtility.OpenFilePanel("Select an image file", "", "png,jpg,psd");
            if (!string.IsNullOrEmpty(singleFilePath))
            {
                filePaths.Clear();
                filePaths.Add(singleFilePath);
            }
        }
        EditorGUILayout.HelpBox("To stop the multiple file selection, press Cancel in the file explorer.", MessageType.Warning); //Baad me fix karega
        if (GUILayout.Button("Select Multiple Files"))
        {
            

            string filePath;
            do
            {
                filePath = EditorUtility.OpenFilePanel("Select an image file (Cancel to end)", "", "png,jpg,psd");
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePaths.Add(filePath);
                }
            } while (!string.IsNullOrEmpty(filePath));
        }

        if (filePaths.Count > 0)
        {
            EditorGUILayout.LabelField("Selected Files:");
            foreach (var path in filePaths)
            {
                EditorGUILayout.LabelField(path);
            }

            GUILayout.Space(20);

            selectedAssetType = GUILayout.SelectionGrid(selectedAssetType, assetTypes, 2);

            if (selectedAssetType == 0) 
            {
                selectedUIType = EditorGUILayout.Popup("UI Type", selectedUIType, uiTypes);
            }
            else if (selectedAssetType == 1) 
            {
                selectedArtType = EditorGUILayout.Popup("Art Type", selectedArtType, artTypes);
            }

            if (GUILayout.Button("Validate"))
            {
                ValidateFiles();
            }

            if (isValid)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Validation successful!", greenTickTexture), GUILayout.Height(20));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Import"))
                {
                    ImportFiles();
                    this.Close(); 
                }
            }

            if (!string.IsNullOrEmpty(validationFeedback) && !isValid)
            {
                EditorGUILayout.HelpBox(validationFeedback, MessageType.Error);
            }
        }
    }

    private void ValidateFiles()
    {
        validationFeedback = string.Empty;
        isValid = true;

        foreach (var filePath in filePaths)
        {
            if (string.IsNullOrEmpty(filePath)) continue;

            string feedback = "";
            bool valid = false;

            if (selectedAssetType == 0) 
            {
                switch (selectedUIType)
                {
                    case 0: 
                        valid = ImageValidator.ValidateTexture(filePath, 256, 256, true, out feedback);
                        break;
                    case 1:
                        valid = ImageValidator.ValidateTexture(filePath, 2048, 2048, false, out feedback);
                        break;
                    case 2:
                        valid = ImageValidator.ValidateTexture(filePath, 1024, 1024, true, out feedback);
                        break;
                }
            }
            else if (selectedAssetType == 1) 
            {
                switch (selectedArtType)
                {
                    case 0:
                        valid = ImageValidator.ValidateTexture(filePath, 1024, 1024, false, out feedback);
                        break;
                    case 1:
                        valid = ImageValidator.ValidateTexture(filePath, 1024, 1024, false, out feedback);
                        break;
                    case 2:
                        valid = ImageValidator.ValidateTexture(filePath, 1024, 1024, false, out feedback);
                        break;
                    case 3: 
                        valid = ImageValidator.ValidateTexture(filePath, 512, 512, true, out feedback);
                        break;
                    case 4: 
                        valid = ImageValidator.ValidateTexture(filePath, 512, 512, true, out feedback);
                        break;
                }
            }

            if (!valid)
            {
                validationFeedback = $"File {Path.GetFileName(filePath)}: {feedback}";
                isValid = false;
                return;
            }
        }
    }

    private void ImportFiles()
    {
        foreach (var filePath in filePaths)
        {
            if (string.IsNullOrEmpty(filePath)) continue;

            string fileName = Path.GetFileName(filePath);
            string destinationPath = "";

            if (selectedAssetType == 0)
            {
                switch (selectedUIType)
                {
                    case 0: 
                        destinationPath = $"Assets/UI/Icons/UI_Icon_{fileName}";
                        break;
                    case 1:
                        destinationPath = $"Assets/UI/Images/UI_Image_{fileName}";
                        break;
                    case 2: 
                        destinationPath = $"Assets/UI/Shapes/UI_Shapes_{fileName}";
                        break;
                }
            }
            else if (selectedAssetType == 1) 
            {
                switch (selectedArtType)
                {
                    case 0: 
                        destinationPath = $"Assets/Art/2D/Characters/Art_Character_{fileName}";
                        break;
                    case 1: 
                        destinationPath = $"Assets/Art/2D/EffectsAndAnimations/Art_VFX_{fileName}";
                        break;
                    case 2: 
                        destinationPath = $"Assets/Art/2D/Environments/Art_ENV_{fileName}";
                        break;
                    case 3: 
                        destinationPath = $"Assets/Art/2D/Foods/Art_Food_{fileName}";
                        break;
                    case 4: 
                        destinationPath = $"Assets/Art/2D/Props/Art_Prop_{fileName}";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(destinationPath))
            {
                string directory = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                FileUtil.CopyFileOrDirectory(filePath, destinationPath);
                Debug.Log($"Imported {fileName} to {destinationPath}");
                AssetDatabase.ImportAsset(destinationPath, ImportAssetOptions.ForceUpdate);
                TextureOptimizer.OptimizeTexture(destinationPath);
            }
        }

        AssetDatabase.Refresh();
    }
}
