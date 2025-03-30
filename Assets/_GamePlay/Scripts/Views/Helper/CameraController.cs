using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class CameraController : MonoBehaviour
    {
        // Lưu vị trí của điểm nhấn (click) hoặc cảm ứng trước đó
        private Vector3 lastPanPosition;
        // Biến kiểm tra trạng thái đang kéo màn hình hay không
        private bool isPanning;

        // Biến điều chỉnh độ nhạy, có thể chỉnh sửa trực tiếp từ Inspector
        [SerializeField]
        private float sensitivity = 1.0f;

        // Các giới hạn di chuyển của camera, có thể chỉnh sửa từ Inspector
        [SerializeField]
        private float minX = -10f;
        [SerializeField]
        private float maxX = 10f;
        [SerializeField]
        private float minY = -10f;
        [SerializeField]
        private float maxY = 10f;

        // LayerMask xác định các layer của đối tượng không cho phép di chuyển camera khi click vào
        [SerializeField]
        private LayerMask blockCameraMovementLayers;

        void Update()
        {
            // Xử lý cảm ứng (touch) cho thiết bị di động
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                // Nếu touch đang trên UI thì không thực hiện di chuyển camera
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                // (Tùy chọn) Nếu cần kiểm tra các đối tượng không cho di chuyển camera bằng raycast:
                /*
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, blockCameraMovementLayers))
                {
                    return;
                }
                */

                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    isPanning = true;
                }
                else if (touch.phase == TouchPhase.Moved && isPanning)
                {
                    PanCamera(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isPanning = false;
                }
            }
            // Xử lý input chuột cho PC
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // Nếu click chuột đang trên UI thì không thực hiện di chuyển camera
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                        return;

                    // Kiểm tra nếu click chuột chạm vào đối tượng trong layer chỉ định thì không di chuyển camera
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f, blockCameraMovementLayers))
                    {
                        return;
                    }

                    lastPanPosition = Input.mousePosition;
                    isPanning = true;
                }
                else if (Input.GetMouseButton(0) && isPanning)
                {
                    PanCamera(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isPanning = false;
                }
            }
        }

        // Hàm di chuyển camera theo chuyển động của chuột/cảm ứng với độ nhạy có thể điều chỉnh
        void PanCamera(Vector3 newPanPosition)
        {
            // Tính toán độ chênh lệch giữa vị trí nhấn trước và vị trí nhấn hiện tại
            Vector3 delta = Camera.main.ScreenToViewportPoint(lastPanPosition - newPanPosition);
            // Di chuyển camera theo độ chênh lệch nhân với hệ số độ nhạy
            transform.Translate(delta * sensitivity, Space.World);
            // Cập nhật lại vị trí nhấn cho lần di chuyển tiếp theo
            lastPanPosition = newPanPosition;

            // Giới hạn vị trí của camera trong vùng được xác định
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
            transform.position = clampedPosition;
        }
    }
}
