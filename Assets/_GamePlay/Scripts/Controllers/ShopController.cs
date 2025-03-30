using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Controllers
{
    public static class ShopController
    {
        public static List<ShopItem> GetShopItem()
        {
            return ShopData.Instance.items;
        }

        public static StatusController CanBuyProduct(int idItem)
        {
            Player player = Collection.LoadModel<Player>();
            var shopItemData = ShopData.Instance.items.Find(x => x.idItem == idItem);
            if (shopItemData == null)
            {
                return StatusController.ShopItemNotFound;
            }

            if (player.money.Value < shopItemData.price)
            {
                return StatusController.NoEnoughMoney;
            }
            return StatusController.Success;
        }

        public static StatusController BuyProduct(int idItem)
        {
            StatusController status = CanBuyProduct(idItem);
            if (status != StatusController.Success)
            {
                return status;
            }

            Player player = Collection.LoadModel<Player>();
            var shopItemData = ShopData.Instance.items.Find(x => x.idItem == idItem);
            player.money.Value -= shopItemData.price;

            PlayerController.AddItemInBag(idItem, shopItemData.quantityPerPurchase);
            Collection.SaveModel(player);

            return StatusController.Success;
        }
    }
}


