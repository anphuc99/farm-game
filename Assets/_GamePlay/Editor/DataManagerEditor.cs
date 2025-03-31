using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;

public static class DataManagerEditor
{
    [MenuItem("Game Fram/Data Manager/Clear Data")]
    public static void ClearData()
    {
        if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to delete data?", "Delete", "Cancel"))
        {
            string filePath = Path.Combine(Application.persistentDataPath, "save.dat");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Đã xóa file dữ liệu tại: " + filePath);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy file dữ liệu tại: " + filePath);
            }

            // Reset dữ liệu trong DataManager.            
            Debug.Log("DataManager đã được reset.");
        }
    }

    [MenuItem("Game Fram/Data Manager/Open Persistent Data Path")]
    public static void OpenPersistentDataFolder()
    {
        // Lấy đường dẫn đến persistentDataPath
        string path = Application.persistentDataPath;
        // Mở thư mục bằng cách sử dụng EditorUtility.RevealInFinder (cross-platform)
        EditorUtility.RevealInFinder(path);
    }
}
