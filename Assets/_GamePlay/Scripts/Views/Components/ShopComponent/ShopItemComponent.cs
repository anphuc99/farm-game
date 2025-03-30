using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ShopItemComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _selectedObject;
        [SerializeField] private Image _imgAvatar;
        [SerializeField] private TextMeshProUGUI _txtMoney;
        [SerializeField] private TextMeshProUGUI _txtQuantityPerPurchase;

        private ShopItem _shopItem;
        private ShopManager _shopManager;
        public void SetUp(ShopItem shopItem, ShopManager shopManager)
        {
            _shopItem = shopItem;
            _shopManager = shopManager;

            ItemData itemData = ItemDatas.Instance.items.Find(x=>x.id == shopItem.idItem);
            _imgAvatar.sprite = itemData.avatar;
            _txtMoney.text = shopItem.price.ToString();        
            _txtQuantityPerPurchase.text = "+" + shopItem.quantityPerPurchase.ToString();
        }        

        public void OnSelected()
        {
            _selectedObject.SetActive(true);
            _shopManager.OnItemSelected(this, _shopItem);
        }

        public void OnOtherShopSelected()
        {
            _selectedObject.SetActive(false);            
        }

    }
}
