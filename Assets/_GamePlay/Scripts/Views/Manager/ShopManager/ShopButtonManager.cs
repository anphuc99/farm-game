using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class ShopButtonManager : MonoBehaviour
    {
        public void OnButtonClick()
        {
            Transform shopPosition = PositionHelper.GetPosition(PositionType.ShopPosition);
            Camera.main.transform.position = new Vector3(shopPosition.position.x, shopPosition.position.y, Camera.main.transform.position.z);
        }
    }
}
