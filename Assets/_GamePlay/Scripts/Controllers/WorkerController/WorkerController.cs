using Models;
using Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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