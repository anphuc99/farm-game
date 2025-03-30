using Models;
using Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static log4net.Appender.RollingFileAppender;

namespace Controllers
{
    public static class WorkerController
    {
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

            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(lastTimePlayGame).LocalDateTime;            

            if (Application.isPlaying)
            {
                var i = ItemDatas.Instance;
                
                await Task.Run(() =>
                {
                    Working(timeGameBreak, player, dateTime);
                });                                
            }
            else
            {
                Working(timeGameBreak, player, dateTime);
            }
            
            player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
            Collection.SaveModel(player, true);
        }

        private static void Working(long timeGameBreak, Player player, DateTime dateTime)
        {
            var workers = player.workers;
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

            return StatusController.Success;
        }
    }
}