using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class NoticationParam
    {
        public class btn
        {
            public string text;
            public bool isDisable;
            public Action onClick;
        }
        public string title;
        public string description;
        public btn button;
    }

    public class PopupNotification : MonoBehaviour, IPopupScript, IPopupBeginShow
    {
        public string PopupID { get; set; }
        public object param { get; set; }

        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        public TextMeshProUGUI textBtn;
        public Button btn;

        public void OnBeginShow()
        {
            NoticationParam notication = (NoticationParam)param;
            title.text = notication.title;
            description.text = notication.description;
            SetButton(notication.button, btn, textBtn);
        }

        private void SetButton(NoticationParam.btn btn, Button button, TextMeshProUGUI text)
        {

            if (btn == null || btn.isDisable)
            {
                button.gameObject.SetActive(true);
                text.text = TextConstant.OK;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    PopupManager.Instance.HidePopup(PopupID);
                });
            }
            else
            {
                button.gameObject.SetActive(true);
                text.text = btn.text;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    btn.onClick?.Invoke();
                });
            }
        }
    }
}
