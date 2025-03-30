using System.IO;
using UnityEditor;
using UnityEngine;

public class ReplaceFileHelper
{
    [MenuItem("Assets/ReplaceWith")]
    public static void DoSomething()
    {
        string selectedFileInProjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string newFilePath = EditorUtility.OpenFilePanel("Select File to Replace With", "", "");
        if (!string.IsNullOrEmpty(newFilePath))
        {
            ReplaceFile(selectedFileInProjectPath, newFilePath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
    
    private static void ReplaceFile(string originalPath, string newPath)
    {
        if (!File.Exists(originalPath))
        {
            Debug.LogError("Original file does not exist!");
            return;
        }

        try
        {
            // Delete the original file
            File.Delete(originalPath);

            // Copy the new file to the location of the original file
            File.Copy(newPath, originalPath);

            Debug.Log("File replaced successfully!");
        }
        catch (IOException e)
        {
            Debug.LogError("Error replacing file: " + e.Message);
        }
    }
}