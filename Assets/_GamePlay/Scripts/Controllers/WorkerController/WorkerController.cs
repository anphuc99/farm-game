using Models;
using Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static log4net.Appender.RollingFileAppender;

namespace Controllers
{
    public static class WorkerController
    {
        private const int SECOND_ONE_DAY = 3600 * 1;

        public static BindableList<Worker> GetWorkers()
        {
            return Collection.LoadModel<Player>().workers;
        }

        public static long GetRemainingTimeWork(int index)
        {
            Player player = Collection.LoadModel<Player>();
            Worker worker = player.workers.Items[index];

            if(worker.stateWorker.Value == StateWorker.None)
            {
                return -1;
            }

            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long workedTime = timeStampNow - worker.timeStartWork;

            return (long)Mathf.Max(0, GameSetting.Instance.workerWorkDuration - workedTime);
        }

        public static int GetProgressWork(int index)
        {
            long remainingTimeWork = GetRemainingTimeWork(index);
            if(remainingTimeWork == -1)
            {
                return -1;
            }
            return 100 - Mathf.Min(Mathf.CeilToInt((float)remainingTimeWork / GameSetting.Instance.workerWorkDuration * 100f), 100); ;
        }

        public static bool UpdateWorker(int index)
        {
            Player player = Collection.LoadModel<Player>();
            Worker worker = player.workers.Items[index];
            WorkerBehavior.Update(worker);
            return true;
        }

        public static async Task AutoWork()
        {
            Player player = Collection.LoadModel<Player>();
            var workers = player.workers;

            long lastTimePlayGame = player.lastTimePlayGame;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long timeGameBreak = timeStampNow - lastTimePlayGame;          

            if (Application.isPlaying)
            {
                var i = ItemDatas.Instance;
                
                if(timeGameBreak > SECOND_ONE_DAY)
                {
                    timeGameBreak -= SECOND_ONE_DAY / 2;
                    timeGameBreak = Working(timeGameBreak);
                    await Task.Run(() =>
                    {
                        WorkingSimulation(timeGameBreak + SECOND_ONE_DAY/2, player);
                    });
                }
                else
                {
                    await Task.Run(() =>
                    {
                        WorkingSimulation(timeGameBreak, player);
                    });
                }                                               
            }
            else
            {
                timeGameBreak = Working(timeGameBreak);
                WorkingSimulation(timeGameBreak, player);
            }
            
            player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
            Collection.SaveModel(player, true);
        }

        private static long Working(long timeGameBreak)
        {
            Player player = Collection.LoadModel<Player>();
            int landsCount = player.lands.Items.Count;
            
            // lấy tất cả hạt giống
            List<BagItem> seeds = player.bagItems.Items.Where(x=>x.Type == TypeItem.Seeds).ToList();

            // thu hoạch hết mấy cây hiện đang trồng

            long maxGrowthTime = 0;
            foreach (Land land in player.lands.Items)
            {

                if (land.Agriculture != null)
                {
                    ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == land.Agriculture.id);
                    long timeToMaturity = LandController.GetTimeToMaturity(itemData);
                    long productionTime = LandController.GetProductionTime(itemData);
                    int limited = itemData.productingLimit;
                    int timeToDead = itemData.timeUntilDeathAfterLimit;
                    long totalGrowthTime = timeToMaturity + productionTime * limited + timeToDead;

                    if(totalGrowthTime > maxGrowthTime)
                    {
                        maxGrowthTime = totalGrowthTime;
                    }
                    PlayerController.AddItemInBag(itemData.harvestId, itemData.productingLimit - land.Agriculture.amountProductPicked.Value);
                    land.Agriculture = null;
                }
            }
            timeGameBreak -= maxGrowthTime;

            
            foreach (BagItem bagItem in seeds)
            {
                ItemData itemData = ItemDatas.Instance.items.Find(x=>x.id == bagItem.id);
                long timeToMaturity = LandController.GetTimeToMaturity(itemData);
                long productionTime = LandController.GetProductionTime(itemData);
                int limited = itemData.productingLimit;
                int timeToDead = itemData.timeUntilDeathAfterLimit;
                int amountSeed = bagItem.amount.Value;

                long totalGrowthTime = timeToMaturity + productionTime * limited + timeToDead;

                long requiredTime = (totalGrowthTime * amountSeed) / landsCount;

                if(timeGameBreak > requiredTime)
                {
                    // nếu thời gian offline lớn hơn thì nhận toàn bộ sản phẩm
                    PlayerController.AddItemInBag(itemData.harvestId, amountSeed * limited);
                    PlayerController.AddItemInBag(itemData.id, -amountSeed);
                    timeGameBreak -= requiredTime;
                }
                else
                {
                    long cycle = timeGameBreak / totalGrowthTime;
                    if (cycle > 0)
                    {
                        long planted = cycle * landsCount;
                        requiredTime = (totalGrowthTime * planted) / landsCount;
                        PlayerController.AddItemInBag(itemData.harvestId, (int)planted * limited);
                        PlayerController.AddItemInBag(itemData.id, -(int)planted);
                        timeGameBreak -= requiredTime;                        
                    }
                    break;
                }
            }

            return timeGameBreak;
        }

        private static void WorkingSimulation(long timeGameBreak, Player player)
        {
            var workers = player.workers;            
            DateTime dateTime = DateTimeHelper.TimeSharpToDateTime(DateTimeHelper.GetTimeStampNow() - timeGameBreak);

            foreach(var worker in  workers.Items)
            {
                worker.workingInLand = -1;
                worker.stateWorker.Value = StateWorker.None;
            }            

            for (int i = 0; i <= timeGameBreak; i += GameSetting.Instance.workerWorkDuration)
            {
                DateTimeHelper.dateTimeNow = dateTime.AddSeconds(i);

                foreach (var worker in workers.Items)
                {
                    WorkerBehavior.Update(worker);
                }

                var itemBag = player.bagItems.Items[0];
                if (itemBag.Type != TypeItem.Seeds)
                {
                    if (player.lands.Items.Find(x => x.Agriculture != null) != null)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            DateTimeHelper.dateTimeNow = null;
        }

        public static StatusController HireWorker()
        {
            Player player = Collection.LoadModel<Player>();
            if(player.money.Value < GameSetting.Instance.hireWorkCost)
            {
                return StatusController.NoEnoughMoney;
            }
            player.money.Value -= GameSetting.Instance.hireWorkCost;
            player.workers.Add(new Worker());
            Collection.SaveModel(player);

            return StatusController.Success;
        }
    }
}