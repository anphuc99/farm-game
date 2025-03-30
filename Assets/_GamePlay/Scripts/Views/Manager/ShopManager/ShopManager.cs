using Controllers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ShopItemComponent _shopItemComponent;
        [SerializeField] private Transform _content;
        [SerializeField] private Button _buttnBuy;

        private List<ShopItemComponent> _shopItemComponents = new List<ShopItemComponent>();
        private ShopItem _shopItemSelected;
        private void Awake()
        {
            _buttnBuy.interactable = false;
            SetUp();
        }

        private void OnEnable()
        {
            SetStatusButton();
        }

        public void SetUp()
        {
            var shopItems = ShopController.GetShopItem();
            foreach (var shopItem in shopItems)
            {
                var shopItemComponent = Instantiate(_shopItemComponent, _content);
                shopItemComponent.SetUp(shopItem, this);
                _shopItemComponents.Add(shopItemComponent);
            }

        }

        public void OnItemSelected(ShopItemComponent itemComponent, ShopItem shopItem)
        {
            _shopItemSelected = shopItem;
            foreach(var shopItemComponent in _shopItemComponents)
            {
                if(shopItemComponent != itemComponent)
                {
                    shopItemComponent.OnOtherShopSelected();
                }
            }
            SetStatusButton();
        }

        private void SetStatusButton()
        {
            if(_shopItemSelected != null)
            {
                var status = ShopController.CanBuyProduct(_shopItemSelected.idItem);
                if (status == StatusController.Success)
                {
                    _buttnBuy.interactable = true;
                }
                else
                {
                    _buttnBuy.interactable = false;
                }
            }
        }

        public void OnBuy()
        {
            if(_shopItemSelected != null)
            {
                ShopController.BuyProduct(_shopItemSelected.idItem);
                SetStatusButton();
            }
        }
    }
}
