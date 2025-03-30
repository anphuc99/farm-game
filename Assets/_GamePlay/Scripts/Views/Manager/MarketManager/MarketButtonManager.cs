using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class MarketButtonManager : MonoBehaviour
    {
        public void OnButtonClick()
        {
            var positionMarket = PositionHelper.GetPosition(PositionType.MarketPosition);
            Camera.main.transform.position = new Vector3(positionMarket.position.x, positionMarket.position.y, Camera.main.transform.position.z);
        }
    }
}
