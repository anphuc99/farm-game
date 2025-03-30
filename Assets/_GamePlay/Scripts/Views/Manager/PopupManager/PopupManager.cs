using Codice.CM.Common;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Views
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance => instance;
        private static PopupManager instance;
        public List<PopupController> _listPopup;

        private Dictionary<string, PopupController> _popuController;
        private int _countPopupShow;

        private void Start()
        {
            _popuController = new Dictionary<string, PopupController>();
            for (int i = 0; i < _listPopup.Count; i++)
            {
                var popup = _listPopup[i];
                PopupController popupController = Instantiate(popup, transform);
                popupController.gameObject.SetActive(false);
                _popuController.Add(popupController.PopupID, popupController);
            }
            instance = this;
        }
        [BoxGroup("Test")]
        [Button]
        public void TestShow(string popupID)
        {
            ShowPopup(popupID);
        }
        [BoxGroup("Test")]
        [Button]
        public void TestHideAll()
        {
            HideAllPopup();
        }

        public PopupController ShowPopup(string PopupID, object param = null)
        {
            PopupController popupController = _popuController[PopupID];
            if (!popupController.isShow)
            {
                popupController.param = param;
                popupController.transform.SetAsLastSibling();
                popupController.Show();
                _countPopupShow++;
                popupController.isShow = true;
                //LockWorldCkick.Instance.Lock();
            }
            return popupController;
        }

        public void HidePopup(string PopupID)
        {
            if (_popuController.ContainsKey(PopupID))
            {
                PopupController popupController = _popuController[PopupID];
                if (popupController.isShow)
                {
                    popupController.Hide();
                    _countPopupShow--;
                    popupController.isShow = false;
                    //if (_countPopupShow <= 0)
                    //{
                    //    LockWorldCkick.Instance.UnLock();
                    //}
                }

            }
        }

        public void HideAllPopup()
        {
            foreach (var popup in _popuController.Values)
            {
                if (popup.gameObject.activeSelf)
                {
                    popup.isShow = false;
                    popup.Hide();
                }
            }
            _countPopupShow = 0;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}