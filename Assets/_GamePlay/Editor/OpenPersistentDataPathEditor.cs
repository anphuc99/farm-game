using UnityEditor;
using UnityEngine;

public class OpenPersistentDataPathEditor
{
    [MenuItem("Tools/Open Persistent Data Path")]
    public static void OpenPersistentDataFolder()
    {
        // Lấy đường dẫn đến persistentDataPath
        string path = Application.persistentDataPath;
        // Mở thư mục bằng cách sử dụng EditorUtility.RevealInFinder (cross-platform)
        EditorUtility.RevealInFinder(path);
    }
}
