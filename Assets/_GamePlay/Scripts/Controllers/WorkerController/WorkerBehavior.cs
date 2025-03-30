using Controllers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorkerBehavior
{
    private static Player player;
    public static void Update(Worker worker)
    {        
        player = Collection.LoadModel<Player>();
        if(worker.stateWorker.Value == StateWorker.None)
        {
            Work(worker);
            return;
        }
        long dateTimeNow = DateTimeHelper.GetTimeStampNow();
        long timeStartWork = worker.timeStartWork;

        if(dateTimeNow >= timeStartWork + GameSetting.Instance.workerWorkDuration)
        {
            FinishWork(worker);
        }
    }

    private static void Work(Worker worker)
    {
        var lands = player.lands;
        for(int i = 0; i < lands.Items.Count; i++)
        {
            var state = CheckLandCanWork(i);
            if(state != StateWorker.None && CheckCanWork(state, i))
            {
                worker.workingInLand = i;
                worker.timeStartWork = DateTimeHelper.GetTimeStampNow();
                worker.stateWorker.Value = state;
                return;
            }
        }
        worker.workingInLand = -1;
        worker.stateWorker.Value = StateWorker.None;
    }

    private static StateWorker CheckLandCanWork(int indexLand)
    {
       var land = player.lands.Items[indexLand];
       if(land.Agriculture == null)
       {
            return StateWorker.Cultivation;
       }

       if(LandController.GetAmountProduct(indexLand) > 0)
       {
            return StateWorker.Harvest;
       }

       if(LandController.GetProgressDead(indexLand) >= 100)
       {
            return StateWorker.CleanDeadAgriculture;
       }
       return StateWorker.None;
    }

    private static bool CheckCanWork(StateWorker stateWorker, int indexLand)
    {
        var workers = player.workers;
        foreach(var worker in workers.Items)
        {
            if (worker.workingInLand == indexLand)
            {
                return false;
            }
        }        

        List<ItemData> itemData = ItemDatas.Instance.items;

        if (stateWorker == StateWorker.Cultivation)
        {
            var bags = player.bagItems;
            bool check = false;
            foreach(var bag in bags.Items)
            {
                var findItem = itemData.Find(x=>x.id == bag.id && x.type == TypeItem.Seeds);
                if(findItem != null)
                {
                    check = true;
                    break;
                }
            }

            if (!check)
            {
                return false;
            }
        }

        return true;
    }

    private static void FinishWork(Worker worker)
    {
        switch (worker.stateWorker.Value)
        {
            case StateWorker.Cultivation:
                var bagItem = player.bagItems.Items.Find(x=>x.Type == TypeItem.Seeds);
                PlayerController.Cultivation(bagItem.id, worker.workingInLand);
                break;
            case StateWorker.Harvest:
                PlayerController.Harvest(worker.workingInLand);
                break;
            case StateWorker.CleanDeadAgriculture:
                LandController.RemoveAgriculture(worker.workingInLand);
                break; 
        }
        Work(worker);
    }
}
