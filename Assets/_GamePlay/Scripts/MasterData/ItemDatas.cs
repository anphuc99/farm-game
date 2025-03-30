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
    public string description;
    public Sprite avatar;
    public int timeToMaturity;
    public int productionTime;
    public int productingLimit;
    public int timeUntilDeathAfterLimit = 3600;
    public int harvestId;    
    public int sellingPrice;
}

[CreateAssetMenu(fileName = "ItemDatas", menuName = "data/ItemDatas")]
public class ItemDatas : ScriptableObject
{
    public static ItemDatas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ItemDatas>("ItemDatas");
            }
            return instance;
        }
    }

    private static ItemDatas instance;

    public List<ItemData> items;
}
