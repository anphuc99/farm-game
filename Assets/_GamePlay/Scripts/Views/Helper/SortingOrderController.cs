using UnityEngine;
using UnityEngine.Rendering;

namespace Views
{
    public class SortingGroupController : MonoBehaviour
    {
        private SortingGroup sortingGroup;

        // Hệ số nhân cho y (có thể điều chỉnh theo game)
        public int yMultiplier = 100;

        // Giới hạn sortingOrder
        public int minOrder = -10000;
        public int maxOrder = 10000;

        void Awake()
        {
            sortingGroup = GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                Debug.LogWarning("Không tìm thấy thành phần SortingGroup trên " + gameObject.name);
            }
        }

        void LateUpdate()
        {
            // Tính sorting order dựa trên vị trí của đối tượng
            int order = (int)(-transform.position.y * yMultiplier + transform.position.x);
            // Giới hạn trong khoảng [minOrder, maxOrder]
            int clampedOrder = Mathf.Clamp(order, minOrder, maxOrder);

            if (sortingGroup != null)
            {
                sortingGroup.sortingOrder = clampedOrder;
            }
        }
    }

}
