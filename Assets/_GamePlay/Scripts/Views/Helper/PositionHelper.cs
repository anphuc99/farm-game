using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public enum PositionType
    {
        HomePositon,
        ShopPosition,
        MarketPosition,
        WorldCanvasPosition,
    }

    public class PositionHelper : MonoBehaviour
    {
        public static Dictionary<PositionType, Transform> _positionTransform = new Dictionary<PositionType, Transform>();

        public PositionType positionType;               

        private void Awake()
        {
            _positionTransform.Add(positionType, transform);
        }

        public static Transform GetPosition(PositionType positionType)
        {
            if (_positionTransform.ContainsKey(positionType))
            {
                return _positionTransform[positionType];
            }
            return null;
        }
    }
}
