using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class PopupShop : MonoBehaviour, IPopupScript
    {
        public string PopupID { get; set; }
        public object param { get; set; }

        public void OnButtonBackClick()
        {
            PopupManager.Instance.HidePopup(PopupID);
        }
    }
}
