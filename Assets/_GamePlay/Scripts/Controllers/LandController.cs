using Models;
using Modules;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Controllers
{
    public static class LandController
    {
        public static BindableList<Land> GetLands()
        {
            return Collection.LoadModel<Player>().lands;
        }

        public static Land GetInfoLand(int indexLand)
        {
            Player player = Collection.LoadModel<Player>();
            if (indexLand < 0 || indexLand >= player.lands.Items.Count)
            {
                return null;
            }
            return player.lands.Items[indexLand];
        }

        public static Agriculture GetInfoAgriculture(int indexLand)
        {
            Land land = GetInfoLand(indexLand);
            return land?.Agriculture;
        }

        /// <summary>
        /// Kiểm tra xem cây trồng tại đất đã trưởng thành chưa.
        /// Nếu indexLand không hợp lệ, trả về InvalidLandIndex;
        /// nếu chưa có cây, trả về NoAgriculture;
        /// nếu chưa đủ thời gian trưởng thành, trả về Immature;
        /// ngược lại trả về Success.
        /// </summary>
        public static StatusController CheckAgricultureMaturity(int indexLand)
        {
            Player player = Collection.LoadModel<Player>();
            if (indexLand < 0 || indexLand >= player.lands.Items.Count)
            {
                return StatusController.InvalidLandIndex;
            }

            Land land = player.lands.Items[indexLand];
            if (land.Agriculture == null)
            {
                return StatusController.NoAgriculture;
            }

            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return StatusController.Other;
            }

            long cultivationTime = land.Agriculture.cultivationTime;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long cultivatedTime = timeStampNow - cultivationTime;

            return cultivatedTime < GetTimeToMaturity(itemData) ? StatusController.Immature : StatusController.Success;
        }

        public static long GetRemainingTimeMaturity(int indexLand)
        {
            Player player = Collection.LoadModel<Player>();
            if (indexLand < 0 || indexLand >= player.lands.Items.Count)
            {
                return -1;
            }

            Land land = player.lands.Items[indexLand];
            if (land.Agriculture == null)
            {
                return -1;
            }
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return -1;
            }

            long cultivationTime = land.Agriculture.cultivationTime;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long remaining = GetTimeToMaturity(itemData) - (timeStampNow - cultivationTime);
            return Math.Max(remaining, 0);
        }

        public static int GetProgressMaturity(int indexLand)
        {
            long remainingTimeMaturity = GetRemainingTimeMaturity(indexLand);
            if (remainingTimeMaturity < 0)
            {
                return -1;
            }
            Land land = GetInfoLand(indexLand);
            if (land == null || land.Agriculture == null)
            {
                return -1;
            }
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return -1;
            }
            return 100 - Mathf.Min(Mathf.CeilToInt((float)remainingTimeMaturity / GetTimeToMaturity(itemData) * 100f), 100);
        }

        public static int GetAmountProduct(int indexLand)
        {
            if (CheckAgricultureMaturity(indexLand) != StatusController.Success)
            {
                return 0;
            }

            Player player = Collection.LoadModel<Player>();
            if (indexLand < 0 || indexLand >= player.lands.Items.Count)
            {
                return 0;
            }
            Land land = player.lands.Items[indexLand];
            int quantityProduced = GetQuantityProduced(indexLand);
            int amount = quantityProduced - land.Agriculture.amountProductPicked.Value;
            return amount;
        }

        public static int GetQuantityProduced(int indexLand)
        {
            if (CheckAgricultureMaturity(indexLand) != StatusController.Success)
            {
                return 0;
            }

            Player player = Collection.LoadModel<Player>();
            Land land = player.lands.Items[indexLand];
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return 0;
            }
            long cultivationTime = land.Agriculture.cultivationTime;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long cultivatedTime = timeStampNow - cultivationTime;

            // Trừ thời gian trưởng thành
            cultivatedTime -= GetTimeToMaturity(itemData);

            // Sản lượng dựa vào thời gian sản xuất thực tế và giới hạn sản xuất
            int amount = (int)Mathf.Min(cultivatedTime / GetProductionTime(itemData), itemData.productingLimit);
            return amount;
        }

        /// <summary>
        /// Kiểm tra xem số lượng sản phẩm đã đạt giới hạn sản xuất hay chưa.
        /// Nếu chưa đạt giới hạn trả về Success; nếu đã đạt giới hạn trả về ProducedLimited.
        /// </summary>
        public static StatusController CheckProducedLimited(int indexLand)
        {
            Land land = GetInfoLand(indexLand);
            if (land == null || land.Agriculture == null)
            {
                return StatusController.NoAgriculture;
            }
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return StatusController.Other;
            }
            int quantityProduced = GetQuantityProduced(indexLand);
            return quantityProduced < itemData.productingLimit ? StatusController.ProducedNoLimited : StatusController.ProducedLimited;
        }

        public static long GetRemainingTimeProduct(int indexLand)
        {
            if (CheckAgricultureMaturity(indexLand) != StatusController.Success)
            {
                return -1;
            }
            if (CheckProducedLimited(indexLand) == StatusController.ProducedLimited)
            {
                return -1;
            }

            Land land = GetInfoLand(indexLand);
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);  
            if (itemData == null)
            {
                return -1;
            }
            long cultivationTime = land.Agriculture.cultivationTime;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long cultivatedTime = timeStampNow - cultivationTime;

            // Trừ thời gian trưởng thành
            cultivatedTime -= GetTimeToMaturity(itemData);

            long time = GetProductionTime(itemData) - cultivatedTime % GetProductionTime(itemData);
            return time;
        }

        public static int GetProgressProduct(int indexLand)
        {
            long remainingTimeProduct = GetRemainingTimeProduct(indexLand);
            if (remainingTimeProduct < 0)
            {
                return -1;
            }
            Land land = GetInfoLand(indexLand);
            if (land == null || land.Agriculture == null)
            {
                return -1;
            }
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return -1;
            }
            return 100 - Mathf.Min(Mathf.CeilToInt((float)remainingTimeProduct / GetProductionTime(itemData) * 100f), 100);
        }

        public static long GetRemainingTimeDead(int indexLand)
        {
            if (CheckAgricultureMaturity(indexLand) != StatusController.Success)
            {
                return -1;
            }
            if (CheckProducedLimited(indexLand) != StatusController.ProducedLimited)
            {
                return -1;
            }

            Land land = GetInfoLand(indexLand);
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return -1;
            }
            long cultivationTime = land.Agriculture.cultivationTime;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long cultivatedTime = timeStampNow - cultivationTime;

            // Trừ thời gian trưởng thành và thời gian sản xuất đã hoàn thành
            cultivatedTime -= GetTimeToMaturity(itemData) + GetProductionTime(itemData) * itemData.productingLimit;

            return (long)Mathf.Max(itemData.timeUntilDeathAfterLimit - cultivatedTime, 0);
        }

        public static int GetProgressDead(int indexLand)
        {
            long remainingTimeDead = GetRemainingTimeDead(indexLand);
            if (remainingTimeDead < 0)
            {
                return -1;
            }
            Land land = GetInfoLand(indexLand);
            if (land == null || land.Agriculture == null)
            {
                return -1;
            }
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
            if (itemData == null)
            {
                return -1;
            }
            return 100 - Mathf.Min(Mathf.CeilToInt((float)remainingTimeDead / itemData.timeUntilDeathAfterLimit * 100f), 100);
        }

        /// <summary>
        /// Kiểm tra có thể xóa cây trồng ở đất được chọn hay không.
        /// Nếu indexLand không hợp lệ hoặc chưa có cây, trả về trạng thái tương ứng.
        /// </summary>
        public static StatusController CanRemoveAgriculture(int indexLand)
        {
            Land land = GetInfoLand(indexLand);
            if (land == null)
            {
                return StatusController.InvalidLandIndex;
            }
            if (land.Agriculture == null)
            {
                return StatusController.NoAgriculture;
            }
            return StatusController.Success;
        }

        public static StatusController RemoveAgriculture(int indexLand)
        {
            var status = CanRemoveAgriculture(indexLand);
            if (status != StatusController.Success)
            {
                return status;
            }
            Land land = GetInfoLand(indexLand);
            land.Agriculture = null;
            Collection.SaveModel(Collection.LoadModel<Player>());
            return StatusController.Success;
        }

        /// <summary>
        /// Thêm đất mới vào game.
        /// Kiểm tra nếu ItemData không phải loại Land hoặc không đủ vật phẩm trong túi thì trả về trạng thái lỗi.
        /// </summary>
        public static StatusController AddLand(int idItem, Vector2 position)
        {
            Player player = Collection.LoadModel<Player>();
            ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == idItem);
            if (itemData == null || itemData.type != TypeItem.Land)
            {
                return StatusController.InvalidItemType;
            }
            var bagItem = player.bagItems.Items.Find(x => x.id == idItem);
            if (bagItem == null || bagItem.amount.Value <= 0)
            {
                return StatusController.InsufficientItems;
            }

            PlayerController.AddItemInBag(idItem, -1);

            player.lands.Add(new Land()
            {
                id = idItem,
                posistion = new System.Numerics.Vector2(position.x, position.y),
            });

            Collection.SaveModel(player);
            return StatusController.Success;
        }

        /// <summary>
        /// Đặt vị trí cho đất tại indexLand.
        /// Nếu indexLand không hợp lệ, trả về InvalidLandIndex.
        /// </summary>
        public static StatusController SetPositionLand(int indexLand, Vector2 position)
        {
            Player player = Collection.LoadModel<Player>();
            if (indexLand < 0 || indexLand >= player.lands.Items.Count)
            {
                return StatusController.InvalidLandIndex;
            }
            Land land = player.lands.Items[indexLand];
            land.posistion = new System.Numerics.Vector2(position.x, position.y);
            Collection.SaveModel(player);
            return StatusController.Success;
        }

        public static StatusController GetStatusAgriculture(int indexLand)
        {
            var status = CheckAgricultureMaturity(indexLand);
            if(status != StatusController.Success && status != StatusController.Immature)
            {
                return status;
            }

            if(status == StatusController.Immature)
            {
                Land land = GetInfoLand(indexLand);
                long remainingTimeMaturity = GetRemainingTimeMaturity(indexLand);
                var itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
                
                if(remainingTimeMaturity < GetTimeToMaturity(itemData) /2)
                {
                    return StatusController.AgricultureHalfMature;
                }
                else
                {
                    return StatusController.AgricultureImmature;
                }

            }

            long timeDead = GetRemainingTimeDead(indexLand);
            if(timeDead == 0)
            {
                return StatusController.AgricultureDead;
            }

            if (CheckProducedLimited(indexLand) == StatusController.ProducedLimited)
            {
                return StatusController.AgricultureMatureLimit;
            }

            return StatusController.AgricultureMature;
        }

        private static long GetTimeToMaturity(ItemData itemData)
        {
            Player player = Collection.LoadModel<Player>();
            return (long)Mathf.Max(itemData.timeToMaturity / (1f + 0.1f * (player.level.Value)),1);
        }

        private static long GetProductionTime(ItemData itemData)
        {
            Player player = Collection.LoadModel<Player>();
            return (long)Mathf.Max(itemData.productionTime / (1f + 0.1f * (player.level.Value)),1);
        }
    }
}
