using Controllers;
using Models;
using Modules;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class MarketControllerTest
{
    private int IdItemTest = 1001;
    private int IdItemTest2 = 1002;
    private int IdItemTest3 = 1004;

    private int GetTimeMaturity(int idItem)
    {
        ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == idItem);
        return itemData.timeToMaturity;
    }

    private int GetTimeProduction(int idItem)
    {
        ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == idItem);
        return itemData.productionTime;
    }

    private int GetHarvestId(int idItem)
    {
        ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == idItem);
        return itemData.harvestId;
    }

    private int GetPriceSell(int idItem)
    {
        ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == GetHarvestId(idItem));
        return itemData.sellingPrice;
    }

    [SetUp]
    public void SetUp()
    {
        DataHelper.Instance.ClearData();
        Collection.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        DateTimeHelper.dateTimeNow = null;
        Collection.Clear();
    }

    [Test]
    public void TestCanSellProduct()
    {
        Player player = Collection.LoadModel<Player>();

        // Trước khi có sản phẩm để bán, nên trả về trạng thái InsufficientItems
        Assert.AreEqual(StatusController.InsufficientItems, MarketController.CanSellProduct(GetHarvestId(IdItemTest), 5));

        // Trồng và thu hoạch để có sản phẩm bán
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity(IdItemTest) + GetTimeProduction(IdItemTest) * 5);
        PlayerController.Harvest(0);

        Assert.AreEqual(StatusController.Success, MarketController.CanSellProduct(GetHarvestId(IdItemTest), 5));
    }

    [Test]
    public void TestSellProduct()
    {
        Player player = Collection.LoadModel<Player>();

        // Thử bán sản phẩm khi chưa có, nên không thành công
        Assert.AreNotEqual(StatusController.Success, MarketController.SellProduct(GetHarvestId(IdItemTest), 5));

        // Trồng và thu hoạch để có sản phẩm bán
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity(IdItemTest) + GetTimeProduction(IdItemTest) * 5);
        PlayerController.Harvest(0);

        Assert.AreEqual(StatusController.Success, MarketController.SellProduct(GetHarvestId(IdItemTest), 5));

        BagItem item = player.bagItems.Items.Find(x => x.id == GetHarvestId(IdItemTest));
        Assert.IsNull(item);
        Assert.AreEqual(GetPriceSell(IdItemTest) * 5, player.money.Value);
    }
}
