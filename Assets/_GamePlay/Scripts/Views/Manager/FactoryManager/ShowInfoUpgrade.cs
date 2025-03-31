using Controllers;
using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class ShowInfoUpgrade : MonoBehaviour
    {
        private const float DELAY_TIME_HIDE = 0.1f;

        public static ShowInfoUpgrade Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtDescription;
        public Vector2 offset;


        private float _delayHide = DELAY_TIME_HIDE;
        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (_delayHide > 0)
            {
                _delayHide -= Time.deltaTime;
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void ShowInfo(Transform factoryPos)
        {
            _delayHide = DELAY_TIME_HIDE;
            transform.position = factoryPos.position + new Vector3(offset.x, offset.y);
            gameObject.SetActive(true);
            UpdateUI();
        }

        public void UpdateUI()
        {
            Player player = PlayerController.GetPlayer();
            _txtLevel.text = string.Format(TextConstant.LEVEL, player.level.Value + 1);
            _txtDescription.text = string.Format(TextConstant.PRODUCTIVITY_INCREASED, player.level.Value * 10);
        }

        public void OnButtonUpgradeClick()
        {
            var status = PlayerController.Upgrade();
            if(status != StatusController.Success)
            {
                PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION, new NoticationParam()
                {
                    title = TextConstant.NOTIFICATION,
                    description = TextConstant.statusController[status],
                });
            }
            else
            {
                UpdateUI();
            }
        }
    }
}
