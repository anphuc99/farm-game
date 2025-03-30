using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public interface IPopupScript
    {
        public string PopupID { get; set; }
        public object param { get; set; }
    }

    public interface IPopupBeginShow
    {
        public void OnBeginShow();
    }

    public interface IPopupEndShow
    {
        public void OnEndShow();
    }

    public interface IPopupBeginHide
    {
        public void OnBeginHide();
    }

    public interface IPopupEndHide
    {
        public void OnEndHide();
    }

    public class PopupController : MonoBehaviour
    {
        public GameObject View => _view;

        public string PopupID = "POPUP_";
        [SerializeField]
        private Image _blur;
        [SerializeField]
        private GameObject _view;

        [NonSerialized]
        public bool isShow;
        public object param;
        public IPopupScript popupScript;
        public IPopupAnimation popupAnimation;

        public Action onBeginShow;
        public Action onEndShow;
        public Action onBeginHide;
        public Action onEndHide;

        private void Awake()
        {
            try
            {
                popupScript = GetComponent<IPopupScript>();
                popupAnimation = GetComponent<IPopupAnimation>();
                popupScript.PopupID = PopupID;

            }
            catch
            {
                Debug.Log(gameObject.name);
            }

        }

        private void OnDisable()
        {
            ClearEvent();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (_blur != null)
            {
                _blur.gameObject.SetActive(true);
            }
            popupScript.param = param;
            if (popupAnimation != null)
            {
                gameObject.SetActive(true);
                if (popupScript is IPopupBeginShow)
                {
                    ((IPopupBeginShow)popupScript).OnBeginShow();
                }
                onBeginShow?.Invoke();
                popupAnimation.OnShow(_view, OnCallBackShowAnimation);
            }
            else
            {
                gameObject.SetActive(true);
                if (popupScript is IPopupBeginShow)
                {
                    ((IPopupBeginShow)popupScript).OnBeginShow();
                }
                onBeginShow?.Invoke();
                if (popupScript is IPopupEndShow)
                {
                    ((IPopupEndShow)popupScript).OnEndShow();
                }
                onEndShow?.Invoke();
            }
        }

        private void OnCallBackShowAnimation()
        {
            if (popupScript is IPopupEndShow)
            {
                ((IPopupEndShow)popupScript).OnEndShow();
            }
            onEndShow?.Invoke();
        }

        public void Hide()
        {
            if (popupAnimation != null)
            {
                if (popupScript is IPopupBeginHide)
                {
                    ((IPopupBeginHide)popupScript).OnBeginHide();
                }
                onBeginHide?.Invoke();
                popupAnimation.OnHide(_view, OnCallBackHideAnimation);
            }
            else
            {
                if (popupScript is IPopupBeginHide)
                {
                    ((IPopupBeginHide)popupScript).OnBeginHide();
                }
                onBeginHide?.Invoke();
                if (popupScript is IPopupEndHide)
                {
                    ((IPopupEndHide)popupScript).OnEndHide();
                }
                onEndHide?.Invoke();
                gameObject.SetActive(false);
                if (_blur != null)
                {
                    _blur.gameObject.SetActive(false);
                }
                gameObject.SetActive(false);
            }
        }

        private void OnCallBackHideAnimation()
        {
            if (popupScript is IPopupEndHide)
            {
                ((IPopupEndHide)popupScript).OnEndHide();
            }
            onEndHide?.Invoke();
            gameObject.SetActive(false);
            if (_blur != null)
            {
                _blur.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        private void ClearEvent()
        {
            onBeginShow = null;
            onEndShow = null;
            onBeginHide = null;
            onEndHide = null;
        }
    }

}

