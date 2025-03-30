using Controllers;
using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Views
{
    public class ShowInfoLandManager : MonoBehaviour
    {
        private const float DELAY_TIME_HIDE = 0.1f;

        public static ShowInfoLandManager Instance { get; private set; }

        [SerializeField] private Image _avatar;
        [SerializeField] private TextMeshProUGUI _txtTitle;
        [SerializeField] private TextMeshProUGUI _txtTime;
        [SerializeField] private Image _progress;
        [SerializeField] private Button _btnHarvest;
        [SerializeField] private Button _btnDestroy;
        [SerializeField] private TextMeshProUGUI _txtHarvest;
        [SerializeField] private TextMeshProUGUI _txtDestroy;
        [SerializeField] private Vector2 _offset;

        private float _delayHide = DELAY_TIME_HIDE;        
        private int indexLand;
        private Land land;  

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

        public void ShowInfo(int indexLand, Transform transformLand)
        {
            _delayHide = DELAY_TIME_HIDE;
            land = LandController.GetInfoLand(indexLand);
            this.indexLand = indexLand;
            gameObject.transform.position = transformLand.position + new Vector3(_offset.x, _offset.y);
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(UpdateStatusEverySecond());
        }

        public IEnumerator UpdateStatusEverySecond()
        {
            while (true)
            {
                UpdateInfo();
                yield return new WaitForSeconds(1);                
            }
        }
        #region Update Status
        private void UpdateInfo()
        {
            var status = LandController.GetStatusAgriculture(indexLand);            
            switch(status)
            {
                case StatusController.AgricultureImmature:
                case StatusController.AgricultureHalfMature:
                    ShowInfoAgricultureImmature();
                    break;
                case StatusController.AgricultureMature: 
                    ShowInfoAgricultureMature(); 
                    break;
                case StatusController.AgricultureMatureLimit:
                    ShowInfoAgricultureMatureLimit();
                    break;
                case StatusController.AgricultureDead:
                    ShowInfoAgricultureDead();
                    break;
            }
        }

        private void ShowInfoNoAgriculture()
        {

        }

        private void ShowInfoAgricultureImmature()
        {
            var itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            itemData = ItemDatas.Instance.items.Find(x => x.id == itemData.harvestId);
            _avatar.sprite = itemData.avatar;
            _avatar.SetNativeSize();
            _txtTitle.text = TextConstant.TIME_TO_MATURE;
            var progress = LandController.GetProgressMaturity(indexLand);
            _progress.fillAmount = progress/100f;
            long timeRemainingMaturity = LandController.GetRemainingTimeMaturity(indexLand);            
            _txtTime.text = DateTimeHelper.TimeSharpToDateTime(timeRemainingMaturity).ToString(@"m\m\ ss\s");
            _btnHarvest.gameObject.SetActive(false);
        }

        private void ShowInfoAgricultureMature()
        {
            var itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            itemData = ItemDatas.Instance.items.Find(x => x.id == itemData.harvestId);
            _avatar.sprite = itemData.avatar;
            _avatar.SetNativeSize();
            int production = LandController.GetAmountProduct(indexLand);
            _txtTitle.text = string.Format(TextConstant.PRODUCTION, production);
            var progress = LandController.GetProgressProduct(indexLand);
            _progress.fillAmount = progress/100f;
            long timeRemainingMaturity = LandController.GetRemainingTimeProduct(indexLand);
            _txtTime.text = DateTimeHelper.TimeSharpToDateTime(timeRemainingMaturity).ToString(@"m\m\ ss\s");

            _btnHarvest.gameObject.SetActive(true);
            if (production > 0)
            {
                _btnHarvest.interactable = true;
            }
            else
            {
                _btnHarvest.interactable = false;
            }
        }

        private void ShowInfoAgricultureMatureLimit()
        {
            var itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            itemData = ItemDatas.Instance.items.Find(x => x.id == itemData.harvestId);
            _avatar.sprite = itemData.avatar;
            _avatar.SetNativeSize();
            int production = LandController.GetAmountProduct(indexLand);
            _txtTitle.text = string.Format(TextConstant.PRODUCTION_LIMITED, production);
            var progress = LandController.GetProgressDead(indexLand);
            _progress.fillAmount = progress / 100f;
            long timeRemainingMaturity = LandController.GetRemainingTimeDead(indexLand);
            _txtTime.text = DateTimeHelper.TimeSharpToDateTime(timeRemainingMaturity).ToString(@"m\m\ ss\s");

            _btnHarvest.gameObject.SetActive(true);
            if (production > 0)
            {
                _btnHarvest.interactable = true;
            }
            else
            {
                _btnHarvest.interactable = false;
            }
        }

        private void ShowInfoAgricultureDead()
        {
            var itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            itemData = ItemDatas.Instance.items.Find(x => x.id == itemData.harvestId);
            _avatar.sprite = itemData.avatar;
            _txtTitle.text = TextConstant.AGRICULTURE_DEAD;
            _progress.fillAmount = 0;
            _txtTime.text = "00m 00s";
            _btnHarvest.gameObject.SetActive(false);
        }
        #endregion

        #region Button Action

        public void ButtonDestroyOnClick()
        {
            PopupManager.Instance.ShowPopup(PopupConstant.POPUP_NOTIFICATION_YES_NO, new NoticationYesNoParam()
            {
                title = TextConstant.NOTIFICATION,
                description = TextConstant.DESTROY_AGRICULTURE,
                btnYes = new NoticationYesNoParam.btn()
                {
                    text = TextConstant.YES,
                    onClick = () =>
                    {
                        LandController.RemoveAgriculture(indexLand);
                        gameObject.SetActive(false);
                        PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION_YES_NO);                        
                    }
                },
                btnNo = new NoticationYesNoParam.btn()
                {
                    text = TextConstant.NO,
                    onClick = () => PopupManager.Instance.HidePopup(PopupConstant.POPUP_NOTIFICATION_YES_NO)
                }
            });
        }

        public void ButtonHarvestOnClick()
        {
            PlayerController.Harvest(indexLand);
            UpdateInfo();
        }
        #endregion
    }
}
