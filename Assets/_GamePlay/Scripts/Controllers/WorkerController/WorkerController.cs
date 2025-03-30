using Models;
using Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
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

            WorkerOptimaze workerOptimazeJob = new WorkerOptimaze();

            List<BagItem> bagSeeds = player.bagItems.Items.Where(x=>x.Type == TypeItem.Seeds).ToList();
            var workers = player.workers.Items;
            var lands = player.lands.Items;
            long timeStampNow = DateTimeHelper.GetTimeStampNow();
            long lastTimePlayGame = player.lastTimePlayGame;
            long timeGameBreak = timeStampNow - lastTimePlayGame;

            workerOptimazeJob.bagItems = new BagItemStruct[bagSeeds.Count];
            workerOptimazeJob.bagNewItem = new BagItemStruct[bagSeeds.Count];
            workerOptimazeJob.workers = new WorkerStruct[workers.Count];
            workerOptimazeJob.lands = new LandStruct[lands.Count];
            workerOptimazeJob.player = new PlayerStruct() { level = player.level.Value };
            workerOptimazeJob.timeStampNow = lastTimePlayGame;
            workerOptimazeJob.timeBreak = timeGameBreak;
            workerOptimazeJob.workerWorkDuration = GameSetting.Instance.workerWorkDuration;


            for (int i = 0; i < bagSeeds.Count; i++)
            {
                var bag = bagSeeds[i];
                ItemData itemData = ItemDatas.Instance.items.Find(x=>x.id == bag.id);
                var bagItemStruct = new BagItemStruct()
                {
                    id = bag.id,
                    indexBag = i,
                    amount = bag.amount.Value,
                    timeToMaturity = itemData.timeToMaturity,
                    productionTime = itemData.productionTime,
                    productingLimit = itemData.productingLimit,
                    timeUntilDeathAfterLimit = itemData.timeUntilDeathAfterLimit,
                    harvestId = itemData.harvestId,
                };
                workerOptimazeJob.bagItems[i] = bagItemStruct;

                var bagNewItem = new BagItemStruct()
                {
                    id = itemData.harvestId,
                    indexBag = i,
                };

                workerOptimazeJob.bagNewItem[i] = bagNewItem;
            }


            for(int i = 0;i < workers.Count;i++)
            {
                var worker = workers[i];
                WorkerStruct workerStruct = new WorkerStruct()
                {
                    index = i,
                    stateWorker = StateWorker.None,
                    workingInLand = -1,                    
                };
                workerOptimazeJob.workers[i] = workerStruct;
            }

            for(int i = 0; i < lands.Count; i++)
            {
                var land = lands[i];
                LandStruct landStruct = new LandStruct();
                landStruct.id = land.id;
                landStruct.index = i;
                if(land.Agriculture != null)
                {
                    landStruct.hasAgriculture = true;
                    landStruct.agricultureId = land.Agriculture.id;
                    landStruct.cultivationTime = land.Agriculture.cultivationTime;
                    landStruct.amountProductPicked = land.Agriculture.amountProductPicked.Value;
                }
                workerOptimazeJob.lands[i] = landStruct;
            }
            
            if(Application.isPlaying)
            {
                await Task.Run(()=> workerOptimazeJob.Execute());
            }
            else
            {
                workerOptimazeJob.Execute();
            }

            var bagItems = player.bagItems.Items;

            for(int i = 0;i < workerOptimazeJob.bagItems.Length;i++)
            {
                var bag = workerOptimazeJob.bagItems[i];
                var bagItem = bagItems.Find(x=>x.id == bag.id);
                if(bag.amount > 0)
                {
                    bagItem.amount.Value = bag.amount;
                }
                else
                {
                    player.bagItems.Remove(bagItem);
                }
            }

            for (int i = 0; i < workerOptimazeJob.bagNewItem.Length; i++)
            {
                var bag = workerOptimazeJob.bagNewItem[i];
                PlayerController.AddItemInBag(bag.id, bag.amount);
            }

            for(int i = 0; i < workerOptimazeJob.lands.Length; i++)
            {
                var land = lands[i];
                var landStruct = workerOptimazeJob.lands[i];
                if (landStruct.hasAgriculture)
                {
                    land.Agriculture = new Agriculture()
                    {
                        id = landStruct.agricultureId,
                        amountProductPicked = new BindableAntiCheat<int>(landStruct.amountProductPicked),
                        cultivationTime = landStruct.cultivationTime,
                    };
                }
                else
                {
                    land.Agriculture = null;
                }
            }

            for(int i = 0; i < workers.Count; i++)
            {
                var worker = workers[i];                
                worker.stateWorker.Value = StateWorker.None;
                worker.workingInLand = -1;                
            }

            //Player player = Collection.LoadModel<Player>();
            //var workers = player.workers;

            //long lastTimePlayGame = player.lastTimePlayGame;
            //long timeStampNow = DateTimeHelper.GetTimeStampNow();
            //long timeGameBreak = timeStampNow - lastTimePlayGame;          

            //if (Application.isPlaying)
            //{
            //    var i = ItemDatas.Instance;

            //    if(timeGameBreak > SECOND_ONE_DAY)
            //    {
            //        timeGameBreak -= SECOND_ONE_DAY / 2;
            //        timeGameBreak = Working(timeGameBreak);
            //        await Task.Run(() =>
            //        {
            //            WorkingSimulation(timeGameBreak + SECOND_ONE_DAY/2, player);
            //        });
            //    }
            //    else
            //    {
            //        await Task.Run(() =>
            //        {
            //            WorkingSimulation(timeGameBreak, player);
            //        });
            //    }                                               
            //}
            //else
            //{
            //    timeGameBreak = Working(timeGameBreak);
            //    WorkingSimulation(timeGameBreak, player);
            //}

            //player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
            //Collection.SaveModel(player, true);
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