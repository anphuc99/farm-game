using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class NoticationYesNoParam
    {
        public class btn
        {
            public string text;
            public bool isDisable;
            public Action onClick;
        }
        public string title;
        public string description;
        public btn btnYes;
        public btn btnNo;
    }

    public class PopupYesNoNotification : MonoBehaviour, IPopupScript, IPopupBeginShow
    {
        public string PopupID { get; set; }
        public object param { get; set; }

        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        public TextMeshProUGUI textBtnYes;
        public TextMeshProUGUI textBtnNo;
        public Button btnYes;
        public Button btnNo;

        public void OnBeginShow()
        {
            NoticationYesNoParam notication = (NoticationYesNoParam)param;
            title.text = notication.title;
            description.text = notication.description;
            SetButton(notication.btnYes, btnYes, textBtnYes);
            SetButton(notication.btnNo, btnNo, textBtnNo);
        }

        private void SetButton(NoticationYesNoParam.btn btn, Button button, TextMeshProUGUI text)
        {

            if (btn == null || btn.isDisable)
            {
                button.gameObject.SetActive(false);
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
