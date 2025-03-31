using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LoadDataInGoogleSheet : EditorWindow
{
    private const string RESOURCE_PAHT = "_GamePlay/Resources";

    private const string SHEET_URL = "https://docs.google.com/spreadsheets/d/1o-7CEvB6TsEh-1QzkeaYI2V7VcVW2Aka0UihCf1askA/edit?gid=0#gid=0";

    private static string sheetId = "1o-7CEvB6TsEh-1QzkeaYI2V7VcVW2Aka0UihCf1askA";
    private static string gidItemData = "0";
    private static string gidShopData = "2099645620";
    private static string gidGameSetting = "2130947611";
    private static string gidInitialItems = "978729287";
    [MenuItem("Game Fram/Google Sheet/Load Data From Google Sheet")]
    public static async void LoadGoogleSheet()
    {
        await Task.WhenAll(
            LoadDataItem(),
            LoadShopData(),
            LoadGameSetting()
        );
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Notification", "Data loaded successfully", "Ok");

    }

    [MenuItem("Game Fram/Google Sheet/Open Google Sheet")]
    public static void OpenGoogleSheet()
    {
        Application.OpenURL(SHEET_URL);
    }

    private static async Task LoadDataItem()
    {
        List<ItemData> datas = new List<ItemData>();
        var items = await ReadGoogleSheets.GetTable(sheetId, gidItemData);

        for (int i = 1; i < items.Count; i++)
        {
            var item = items[i];

            ItemData data = new ItemData();
            data.id = int.TryParse(item[0], out var id) ? id : 0;
            data.type = Enum.TryParse<TypeItem>(item[1], out var type) ? type : default;
            data.name = item[2];
            data.avatar = item[3];
            data.description = item[4];
            data.timeToMaturity = int.TryParse(item[5], out var timeToMaturity) ? timeToMaturity : 0;
            data.productionTime = int.TryParse(item[6], out var productionTime) ? productionTime : 0;
            data.productingLimit = int.TryParse(item[7], out var productingLimit) ? productingLimit : 0;
            data.timeUntilDeathAfterLimit = int.TryParse(item[8], out var timeUntilDeathAfterLimit) ? timeUntilDeathAfterLimit : 0;
            data.harvestId = int.TryParse(item[9], out var harvestId) ? harvestId : 0;
            data.sellingPrice = int.TryParse(item[10], out var sellingPrice) ? sellingPrice : 0;
            datas.Add(data);
        }

        ItemDatas itemDatas = new ItemDatas();
        itemDatas.items = datas;

        string json = JsonConvert.SerializeObject(itemDatas, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, RESOURCE_PAHT, "ItemDatas.json");

        File.WriteAllText(path, json);
    }

    private static async Task LoadShopData()
    {
        List<ShopItem> datas = new List<ShopItem>();
        var items = await ReadGoogleSheets.GetTable(sheetId, gidShopData);

        for (int i = 1; i < items.Count; i++)
        {
            var item = items[i];

            ShopItem data = new ShopItem();
            data.idItem = int.TryParse(item[0], out var id) ? id : 0;
            data.price = int.TryParse(item[1], out int price) ? price : default;
            data.quantityPerPurchase = int.TryParse(item[2], out var quantityPerPurchase) ? quantityPerPurchase : default;
            datas.Add(data);
        }

        ShopData shopData = new ShopData();
        shopData.items = datas;

        string json = JsonConvert.SerializeObject(shopData, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, RESOURCE_PAHT, "ShopData.json");

        File.WriteAllText(path, json);
    }

    private static async Task LoadGameSetting()
    {
        GameSetting gameSetting = new GameSetting();
        var items = await ReadGoogleSheets.GetTable(sheetId, gidGameSetting);
        gameSetting.initialMoney = int.TryParse(items[0][1], out var money) ? money : 0;
        gameSetting.initialLandPlots = int.TryParse(items[1][1], out var land) ? land : 3;
        gameSetting.initialWorkers = int.TryParse(items[2][1], out var workers) ? workers : 1;
        gameSetting.upgradeCost = int.TryParse(items[3][1], out var upgradeCost) ? upgradeCost : 500;
        gameSetting.upgradeProductivityPercent = int.TryParse(items[4][1], out var productivityPercent) ? productivityPercent : 10;
        gameSetting.expandLandCost = int.TryParse(items[5][1], out var expandLandCost) ? expandLandCost : 500;
        gameSetting.hireWorkCost = int.TryParse(items[6][1], out var hireWorkCost) ? hireWorkCost : 500;
        gameSetting.workerWorkDuration = int.TryParse(items[7][1], out var workerWorkDuration) ? workerWorkDuration : 120;
        gameSetting.moneyToCompleteGame = int.TryParse(items[8][1], out var moneyToCompleteGame) ? moneyToCompleteGame : 1000000;

        var initialItems = await ReadGoogleSheets.GetTable(sheetId, gidInitialItems);
        List<InitItem> initItems = new List<InitItem>();
        for (int i = 1; i < initialItems.Count; i++)
        {
            var initialItem = initialItems[i];
            InitItem initItem = new InitItem()
            {
                id = int.TryParse(initialItem[0], out var id) ? id : 0,
                amount = int.TryParse(initialItem[1], out var amount) ? amount : 0,
            };
            initItems.Add(initItem);
        }

        gameSetting.initialItems = initItems;
        string json = JsonConvert.SerializeObject(gameSetting, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, RESOURCE_PAHT, "GameSetting.json");

        File.WriteAllText(path, json);
    }
}
