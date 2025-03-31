using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeItem
{
    Seeds,
    Product,
    Land
}

[Serializable]
public class ItemData
{
    public int id;
    public TypeItem type;
    public string name;
    public string avatar;
    public string description;    
    public int timeToMaturity;
    public int productionTime;
    public int productingLimit;
    public int timeUntilDeathAfterLimit = 3600;
    public int harvestId;    
    public int sellingPrice;
}


public class ItemDatas
{
    public static ItemDatas Instance
    {
        get
        {
            if (instance == null)
            {
                string json = Resources.Load<TextAsset>("ItemDatas").text;   
                
                instance = JsonConvert.DeserializeObject<ItemDatas>(json);

            }
            return instance;
        }
    }

    private static ItemDatas instance;

    public List<ItemData> items;
}
