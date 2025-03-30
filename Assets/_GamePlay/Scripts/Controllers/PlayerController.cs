using Models;
using Modules;

namespace Controllers
{

    public static class PlayerController
    {
        public static Player GetPlayer()
        {
            return Collection.LoadModel<Player>();
        }

        public static StatusController CanCultivation(int idAgriculture, int indexLand)
        {
            Player player = Collection.LoadModel<Player>();
            if (indexLand >= player.lands.Items.Count)
            {
                return StatusController.InvalidLandIndex;
            }

            var land = player.lands.Items[indexLand];
            if (land.Agriculture != null)
            {
                return StatusController.LandAlreadyOccupied;
            }

            var bagItems = player.bagItems;
            var bagItem = bagItems.Items.Find(x => x.id == idAgriculture);

            if (bagItem == null || bagItem.amount.Value <= 0)
            {
                return StatusController.InsufficientItems;
            }

            return StatusController.Success;
        }

        public static StatusController Cultivation(int idAgriculture, int indexLand)
        {
            StatusController status = CanCultivation(idAgriculture, indexLand);
            if (status != StatusController.Success)
            {
                return status;
            }

            Player player = Collection.LoadModel<Player>();
            var land = player.lands.Items[indexLand];

            Agriculture agriculture = new Agriculture()
            {
                id = idAgriculture,
                cultivationTime = DateTimeHelper.GetTimeStampNow(),
            };

            AddItemInBag(idAgriculture, -1);

            land.Agriculture = agriculture;
            Collection.SaveModel(player);
            return StatusController.Success;
        }

        public static StatusController Harvest(int indexLand)
        {
            if (LandController.CheckAgricultureMaturity(indexLand) != StatusController.Success)
            {
                return StatusController.Immature;
            }

            Player player = Collection.LoadModel<Player>();
            var land = player.lands.Items[indexLand];
            var amount = LandController.GetAmountProduct(indexLand);

            if (amount <= 0)
            {
                return StatusController.NoProducts;
            }

            // Lấy idItem của sản phẩm thu hoạch được
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);

            AddItemInBag(itemData.harvestId, amount);
            land.Agriculture.amountProductPicked.Value += amount;

            Collection.SaveModel(player);
            return StatusController.Success;
        }                

        public static StatusController CanUpgrade()
        {
            Player player = Collection.LoadModel<Player>();

            if (player.money.Value < GameSetting.Instance.upgradeCost)
            {
                return StatusController.NoEnoughMoney;
            }

            return StatusController.Success;
        }

        public static StatusController Upgrade()
        {
            StatusController status = CanUpgrade();
            if (status != StatusController.Success)
            {
                return status;
            }

            Player player = Collection.LoadModel<Player>();

            player.money.Value -= GameSetting.Instance.upgradeCost;
            player.level.Value++;

            Collection.SaveModel(player);
            return StatusController.Success;
        }

        public static void AddItemInBag(int idItem, int amount)
        {
            Player player = Collection.LoadModel<Player>();
            var bagItems = player.bagItems;
            var bagItem = bagItems.Items.Find(x => x.id == idItem);
            if (bagItem != null)
            {
                bagItem.amount.Value += amount;
                if (bagItem.amount.Value <= 0)
                {
                    bagItems.Remove(bagItem);
                }
            }
            else
            {
                bagItem = new BagItem()
                {
                    id = idItem,
                    amount = new BindableAntiCheat<int>(amount),
                };
                player.bagItems.Add(bagItem);
            }
            Collection.SaveModel(player);
        }

        public static StatusController ExpandLand()
        {
            Player player = Collection.LoadModel<Player>();
            if(player.money.Value < GameSetting.Instance.expandLandCost)
            {
                return StatusController.NoEnoughMoney;
            }

            ItemData item = ItemDatas.Instance.items.Find(x => x.type == TypeItem.Land);
            player.money.Value -= GameSetting.Instance.expandLandCost;
            AddItemInBag(item.id, 1);
            return StatusController.Success;
        }
    }
}
