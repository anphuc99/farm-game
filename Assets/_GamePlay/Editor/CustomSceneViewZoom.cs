using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CustomSceneViewZoom
{
    private static bool isZPressed = false;

    static CustomSceneViewZoom()
    {
        // Đăng ký callback để lắng nghe sự kiện Scene GUI
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // Theo dõi trạng thái phím Z
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Z)
        {
            isZPressed = true;
        }
        else if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Z)
        {
            isZPressed = false;
        }

        // Nếu đang giữ phím Z và kéo chuột trái
        if (isZPressed && e.type == EventType.MouseDrag && e.button == 0)
        {
            // Tính zoomDelta dựa vào hướng kéo chuột (kéo sang phải: zoom in, sang trái: zoom out)
            float zoomDelta = -e.delta.x * 0.1f; // Thay đổi hệ số nhạy nếu cần

            if (sceneView.camera.orthographic)
            {
                // Lấy vị trí con chuột trên màn hình GUI
                Vector2 mousePos = e.mousePosition;

                // Chuyển đổi vị trí con chuột sang tọa độ world bằng ray và mặt phẳng lấy pivot làm điểm gốc
                Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
                Plane plane = new Plane(sceneView.camera.transform.forward, sceneView.pivot);
                float distance;
                Vector3 worldMouseBefore = sceneView.pivot;
                if (plane.Raycast(ray, out distance))
                {
                    worldMouseBefore = ray.GetPoint(distance);
                }

                // Cập nhật kích thước zoom
                float newSize = sceneView.camera.orthographicSize + zoomDelta;
                newSize = Mathf.Max(newSize, 0.1f); // Đảm bảo giá trị không âm
                sceneView.camera.orthographicSize = newSize;
                sceneView.size = newSize;

                // Tính lại vị trí world của con chuột sau khi thay đổi zoom
                ray = HandleUtility.GUIPointToWorldRay(mousePos);
                Vector3 worldMouseAfter = sceneView.pivot;
                if (plane.Raycast(ray, out distance))
                {
                    worldMouseAfter = ray.GetPoint(distance);
                }

                // Điều chỉnh pivot để vị trí world của con chuột không thay đổi
                Vector3 diff = worldMouseBefore - worldMouseAfter;
                sceneView.pivot += diff;

                sceneView.Repaint();
                e.Use();
            }
        }
    }
}
