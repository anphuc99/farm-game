using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class ShopWorldSceneManager : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_SHOP);
        }
    }
}
