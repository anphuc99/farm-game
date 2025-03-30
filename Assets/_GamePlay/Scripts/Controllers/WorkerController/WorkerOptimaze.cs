using Models;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Controllers
{
    public struct PlayerStruct
    {
        public int level;
    }

    public struct BagItemStruct
    {
        public int id;
        public int indexBag;
        public int amount;
        public int timeToMaturity;
        public int productionTime;
        public int productingLimit;
        public int timeUntilDeathAfterLimit;
        public int harvestId;
    }

    public struct LandStruct
    {
        public int id;
        public int index;
        public int indexBag;
        public bool hasAgriculture;
        public int agricultureId;
        public int timeToMaturity;
        public int productionTime;
        public int productingLimit;
        public int timeUntilDeathAfterLimit;
        public long cultivationTime;
        public int amountProductPicked;
        public int harvestId;
    }

    public struct WorkerStruct
    {
        public int index;
        public StateWorker stateWorker;
        public int workingInLand;
        public long timeStartWork;
    }

    public struct WorkerOptimaze
    {
        public BagItemStruct[] bagItems;
        public BagItemStruct[] bagNewItem;
        public WorkerStruct[] workers;
        public LandStruct[] lands;
        public PlayerStruct player;
        public long timeStampNow;
        public long timeBreak;
        public int workerWorkDuration;

        public void Execute()
        {
            for (int i = 0; i < timeBreak; i+= workerWorkDuration)
            {
                timeStampNow+= workerWorkDuration;
                for(int j = 0; j < workers.Length; j++)
                {                    
                    UpdateWork(j);
                }
            }
        }

        #region Working

        private bool CanCultivation(int indexLand)
        {            
            var land = lands[indexLand];
            if (land.hasAgriculture)
            {
                return false;
            }

            for (int i = 0; i < bagItems.Length; i++)
            {
                if (bagItems[i].amount > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void Cultivation(int indexBag , int indexLand)
        {
            var land = lands[indexLand];
            BagItemStruct bagItem = bagItems[indexBag];
            land.hasAgriculture = true;
            land.agricultureId = bagItem.id;
            land.cultivationTime = timeStampNow;
            land.timeToMaturity = bagItem.timeToMaturity;
            land.productionTime = bagItem.productionTime;
            land.productingLimit = bagItem.productingLimit;
            land.harvestId = bagItem.harvestId;
            land.indexBag = indexBag;
            land.amountProductPicked = 0;
            lands[indexLand] = land;
            bagItem.amount--;
            bagItems[indexBag] = bagItem;
        }

        private int GetQuantityProduced(LandStruct land)
        {            
            long cultivationTime = land.cultivationTime;            
            long cultivatedTime = timeStampNow - cultivationTime;

            // Trừ thời gian trưởng thành
            cultivatedTime -= GetTimeToMaturity(land);

            if (cultivatedTime < 0) 
            { 
                return 0; 
            }

            // Sản lượng dựa vào thời gian sản xuất thực tế và giới hạn sản xuất
            int amount = (int)Mathf.Min(cultivatedTime / GetProductionTime(land), land.productingLimit);
            return amount;
        }

        private bool CheckAgricultureDead(LandStruct land)
        {
            long cultivationTime = land.cultivationTime;
            long cultivatedTime = timeStampNow - cultivationTime;

            // Trừ thời gian trưởng thành và thời gian sản xuất đã hoàn thành
            cultivatedTime -= GetTimeToMaturity(land) + GetProductionTime(land) * land.productingLimit;
            return cultivatedTime >= 0;
        }

        private int GetAmountProduct(LandStruct land)
        {
            return GetQuantityProduced(land) - land.amountProductPicked;
        }

        private bool CanHarvest(LandStruct land)
        {
            return GetAmountProduct(land) > 0;
        }

        private void Harvest(int indexLand)
        {
            LandStruct land = lands[indexLand];
            int amount = GetAmountProduct(land);
            land.amountProductPicked += amount;
            AddItemInBag(land.indexBag, amount);
            lands[indexLand] = land;
        }

        private void Destroy(int indexLand)
        {
            LandStruct land = lands[indexLand];
            land.hasAgriculture = false;
            lands[indexLand] = land;
        }

        private void AddItemInBag(int indexBag, int amount)
        {
            var bag = bagNewItem[indexBag];
            bag.amount += amount;
            bagNewItem[indexBag] = bag;
        }

        private long GetTimeToMaturity(LandStruct land)
        {
            return (long)Mathf.Max(land.timeToMaturity / (1f + 0.1f * (player.level)), 1);
        }

        private long GetProductionTime(LandStruct land)
        {            
            return (long)Mathf.Max(land.productionTime / (1f + 0.1f * (player.level)), 1);
        }
        #endregion

        #region Worker

        private void UpdateWork(int indexWorker)
        {
            WorkerStruct worker = workers[indexWorker];
            if(worker.stateWorker == StateWorker.None)
            {
                Work(worker);
                worker = workers[indexWorker];
            }

            long dateTimeNow = timeStampNow;
            long timeStartWork = worker.timeStartWork;

            if (dateTimeNow >= timeStartWork + workerWorkDuration)
            {
                FinishWork(worker);
            }
        }

        private void Work(WorkerStruct worker)
        {
            for (int i = 0; i < lands.Length; i++)
            {
                var state = CheckLandCanWork(i);
                if (state != StateWorker.None && CheckCanWork(i))
                {
                    worker.workingInLand = i;
                    worker.timeStartWork = timeStampNow;
                    worker.stateWorker = state;
                    workers[worker.index] = worker;
                    return;
                }
            }
            worker.workingInLand = -1;
            worker.stateWorker = StateWorker.None;

            workers[worker.index] = worker;
        }

        private StateWorker CheckLandCanWork(int indexLand)
        {
            var land = lands[indexLand];
            if (CanCultivation(indexLand))
            {
                return StateWorker.Cultivation;
            }

            if (land.hasAgriculture && GetAmountProduct(land) > 0)
            {
                return StateWorker.Harvest;
            }

            if (land.hasAgriculture && CheckAgricultureDead(land))
            {
                return StateWorker.CleanDeadAgriculture;
            }
            return StateWorker.None;
        }

        private bool CheckCanWork(int indexLand)
        {
            for(int i = 0; i < workers.Length; i++)
            {
                var worker = workers[i];
                if (worker.workingInLand == indexLand)
                {
                    return false;
                }
            }            
            return true;
        }

        private void FinishWork(WorkerStruct worker)
        {
            switch (worker.stateWorker)
            {
                case StateWorker.Cultivation:

                    for(int i = 0; i < bagItems.Length; i++)
                    {
                        var bag = bagItems[i];
                        if(bag.amount > 0)
                        {
                            Cultivation(bag.indexBag, worker.workingInLand);
                            break;
                        }
                    }                    
                    break;
                case StateWorker.Harvest:
                    Harvest(worker.workingInLand);
                    break;
                case StateWorker.CleanDeadAgriculture:
                    Destroy(worker.workingInLand);
                    break;
            }
            Work(worker);
        }

        #endregion
    }
}


