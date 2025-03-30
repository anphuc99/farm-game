using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;

public static class ClearDataMenu
{
    [MenuItem("Tools/Data Manager/Clear Data")]
    public static void ClearData()
    {
        if (EditorUtility.DisplayDialog("Xác nhận", "Bạn có chắc chắn muốn xóa dữ liệu?", "Xóa", "Hủy"))
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
}
