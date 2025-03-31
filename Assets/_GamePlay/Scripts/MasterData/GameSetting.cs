using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InitItem
{
    public int id;
    public int amount;
}

public class GameSetting 
{
    public static GameSetting Instance
    {
        get
        {
            if(instance == null)
            {
                string json = Resources.Load<TextAsset>("GameSetting").text;
                instance = JsonConvert.DeserializeObject<GameSetting>(json);
            }
            return instance;
        }
    }

    private static GameSetting instance;

    public int initialMoney;
    public int initialLandPlots;
    public int initialWorkers;
    public List<InitItem> initialItems;

    public int upgradeCost;
    public int upgradeProductivityPercent;

    public int expandLandCost;
    public int hireWorkCost;
    public int workerWorkDuration;

    public int moneyToCompleteGame;
}

