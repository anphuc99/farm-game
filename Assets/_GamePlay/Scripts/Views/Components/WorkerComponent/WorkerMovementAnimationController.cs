using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class WorkerMovementAnimationController : MonoBehaviour
    {
        public Animator animator;             // Tham chiếu đến Animator chứa các Trigger và parameter "move"
        public float minSpeedThreshold = 0.1f;  // Ngưỡng tốc độ tối thiểu để xác định chuyển động (để tránh noise)

        private Vector3 lastPosition;
        private string lastDirection = "";

        void Start()
        {
            // Lưu vị trí ban đầu
            lastPosition = transform.position;
        }

        void Update()
        {
            // Tính delta position và tốc độ di chuyển dựa trên Time.deltaTime
            Vector3 currentPosition = transform.position;
            Vector3 delta = currentPosition - lastPosition;
            float speed = delta.magnitude / Time.deltaTime;  // Tốc độ di chuyển tính theo đơn vị trên giây

            // Cập nhật parameter "move" cho animator
            animator.SetFloat("move", speed);

            // Nếu tốc độ vượt qua ngưỡng, xác định hướng chuyển động
            if (speed > minSpeedThreshold)
            {
                string direction = "";
                // Lấy vector hướng di chuyển (normalized)
                Vector3 moveDir = delta.normalized;

                // So sánh thành phần horizontal và vertical (trong trường hợp game 2D, ta dùng x và y)
                if (Mathf.Abs(moveDir.x) >= Mathf.Abs(moveDir.y))
                {
                    // Di chuyển ngang chiếm ưu thế
                    direction = moveDir.x > 0 ? "right" : "left";
                }
                else
                {
                    // Di chuyển dọc chiếm ưu thế
                    direction = moveDir.y > 0 ? "top" : "down";
                }

                // Nếu hướng thay đổi so với frame trước, trigger animation tương ứng
                if (direction != lastDirection)
                {
                    animator.SetTrigger(direction);
                    lastDirection = direction;
                }
            }
            else
            {
                // Nếu không di chuyển, có thể reset hướng hoặc làm gì đó (tùy vào animator)
                //lastDirection = "";
            }

            // Cập nhật vị trí cho frame tiếp theo
            lastPosition = currentPosition;
        }
    }
}
