using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItem
{
    public int idItem;
    public int price;
    public int quantityPerPurchase;
}

[CreateAssetMenu(fileName = "ShopData", menuName = "data/shopData")]
public class ShopData : ScriptableObject
{
    public static ShopData Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<ShopData>("ShopData");
            }
            return instance;
        }
    }

    private static ShopData instance;

    public List<ShopItem> items;
}
