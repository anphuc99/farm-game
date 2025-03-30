using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InitItem
{
    public int id;
    public int amount;
}

[CreateAssetMenu(fileName = "GameSetting", menuName = "data/GameSetting")]
public class GameSetting : ScriptableObject
{
    public static GameSetting Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<GameSetting>("GameSetting");
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
}

