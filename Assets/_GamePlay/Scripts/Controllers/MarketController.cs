using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public static class MarketController
    {
        public static StatusController CanSellProduct(int idItem, int amount)
        {
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == idItem);
            if (itemData.type != TypeItem.Product)
            {
                return StatusController.InvalidItemType;
            }

            Player player = Collection.LoadModel<Player>();
            BagItem bagItem = player.bagItems.Items.Find(x => x.id == idItem);
            if (bagItem == null)
            {
                return StatusController.InsufficientItems;
            }

            if (bagItem.amount.Value < amount)
            {
                return StatusController.InsufficientItems;
            }

            return StatusController.Success;
        }

        public static StatusController SellProduct(int idItem, int amount)
        {
            StatusController status = CanSellProduct(idItem, amount);
            if (status != StatusController.Success)
            {
                return status;
            }

            Player player = Collection.LoadModel<Player>();
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == idItem);
            PlayerController.AddItemInBag(idItem, -amount);
            player.money.Value += itemData.sellingPrice * amount;

            Collection.SaveModel(player);
            return StatusController.Success;
        }
    }
}

