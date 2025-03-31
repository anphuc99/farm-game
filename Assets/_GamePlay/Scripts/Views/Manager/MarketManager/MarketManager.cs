using Controllers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class MarketManager : MonoBehaviour
    {
        [SerializeField] private Image _imgAvatar;
        [SerializeField] private TextMeshProUGUI _txtName;
        [SerializeField] private TextMeshProUGUI _txtCoin;
        [SerializeField] private TextMeshProUGUI _txtDesription;
        [SerializeField] private Slider _sliderNumber;
        [SerializeField] private TextMeshProUGUI _txtAmount;

        private ItemData _itemData;
        private BagItem _bagItemSelected;
        private void Awake()
        {
            OnProductDeselect(null);
        }

        private void OnEnable()
        {
            EventManager.Resgister(EventConstant.ON_PRODUCT_SELECTED, OnProductSelected);                        
            EventManager.Resgister(EventConstant.ON_PRODUCT_DESELECTED, OnProductDeselect);                        
        }

        private void OnDisable()
        {
            EventManager.UnResgister(EventConstant.ON_PRODUCT_SELECTED, OnProductSelected);
            EventManager.UnResgister(EventConstant.ON_PRODUCT_DESELECTED, OnProductDeselect);
        }

        private void OnProductDeselect(object obj)
        {
            _imgAvatar.gameObject.SetActive(false);
            _sliderNumber.maxValue = 0;
            _txtCoin.text = "0";
            _txtAmount.text = "0";
            _txtDesription.text = "";
            _txtName.text = "";
            _bagItemSelected = null;
        }

        private void OnProductSelected(object obj)
        {
            BagItem bagItem = (BagItem)obj;
            _bagItemSelected = bagItem;
            ItemData itemData = ItemDatas.Instance.items.Find(x=>x.id == bagItem.id);
            if (itemData != null)
            {
                _imgAvatar.gameObject.SetActive(true);
                _itemData = itemData;
                _imgAvatar.sprite = SpriteHelper.Instance.GetSprite(itemData.avatar);
                _imgAvatar.SetNativeSize();
                _sliderNumber.minValue = 1;
                _sliderNumber.maxValue = bagItem.amount.Value;
                _txtAmount.text = _sliderNumber.value.ToString();
                _txtName.text = itemData.name;
                _txtDesription.text = itemData.description;
                SetMoney();
            }
        }

        public void SetMoney()
        {
            if(_itemData != null && _bagItemSelected != null)
            {
                _txtCoin.text = (_itemData.sellingPrice * _sliderNumber.value).ToString();
                _txtAmount.text = _sliderNumber.value.ToString();
            }
        }

        public void OnButtonSellClick()
        {
            if(_bagItemSelected != null)
            {
                MarketController.SellProduct(_bagItemSelected.id, (int)_sliderNumber.value);
                _sliderNumber.value = 1;
                _sliderNumber.maxValue = _bagItemSelected.amount.Value;
                SetMoney();
            }
        }
    }
}
