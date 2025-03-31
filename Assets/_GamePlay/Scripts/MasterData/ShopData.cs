using Newtonsoft.Json;
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

public class ShopData
{
    public static ShopData Instance
    {
        get
        {
            if(instance == null)
            {
                string json = Resources.Load<TextAsset>("ShopData").text;
                instance = JsonConvert.DeserializeObject<ShopData>(json);
            }
            return instance;
        }
    }

    private static ShopData instance;

    public List<ShopItem> items;
}
