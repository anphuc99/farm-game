using Controllers;
using Models;
using Modules;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class PlayerControllerTest
{
    private int IdItemTest = 1001;
    private int IdItemTest2 = 1002;
    private int IdItemTest3 = 1004;

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

    [Test]
    public void TestGetPlayer()
    {
        Player player = PlayerController.GetPlayer();
        Assert.IsNotNull(player);
        Assert.AreEqual(GameSetting.Instance.initialMoney, player.money.Value);
        Assert.AreEqual(GameSetting.Instance.initialLandPlots, player.lands.Items.Count);
        Assert.AreEqual(GameSetting.Instance.initialWorkers, player.workers.Items.Count);
    }

    [Test]
    public void TestCanCultivation()
    {
        Player player = Collection.LoadModel<Player>();

        // Kiểm tra trường hợp hợp lệ
        Assert.AreEqual(StatusController.Success, PlayerController.CanCultivation(IdItemTest, 0));

        // Kiểm tra trường hợp chỉ số đất vượt quá giới hạn
        Assert.AreEqual(StatusController.InvalidLandIndex, PlayerController.CanCultivation(IdItemTest2, 4));

        // Sau khi trồng cây tại vị trí 0, lại không thể trồng thêm tại cùng vị trí khác
        PlayerController.Cultivation(IdItemTest, 0);
        Assert.AreEqual(StatusController.LandAlreadyOccupied, PlayerController.CanCultivation(IdItemTest2, 0));
    }

    [Test]
    public void TestCultivation()
    {
        Player player = Collection.LoadModel<Player>();
        List<Land> lands = player.lands.Items;

        BagItem bagItem = player.bagItems.Items.Find(x => x.id == IdItemTest);
        int curAmount = bagItem.amount.Value;

        // Thực hiện trồng cây hợp lệ
        Assert.AreEqual(StatusController.Success, PlayerController.Cultivation(IdItemTest, 0));

        // Thử trồng cây với id không phù hợp tại vị trí đã có cây
        Assert.AreNotEqual(StatusController.Success, PlayerController.Cultivation(IdItemTest2, 0));

        Assert.IsNotNull(lands[0].Agriculture);
        Assert.AreEqual(IdItemTest, lands[0].Agriculture.id);

        bagItem = player.bagItems.Items.Find(x => x.id == IdItemTest);
        Assert.AreEqual(curAmount - 1, bagItem.amount.Value);
    }

    [Test]
    public void TestHarvest()
    {
        Player player = Collection.LoadModel<Player>();
        List<Land> lands = player.lands.Items;

        // Trồng cây trước khi thu hoạch
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity(IdItemTest) + GetTimeProduction(IdItemTest));

        Assert.AreEqual(StatusController.Success, PlayerController.Harvest(0));

        BagItem item = player.bagItems.Items.Find(x => x.id == GetHarvestId(IdItemTest));
        Assert.IsNotNull(item);
        Assert.AreEqual(0, LandController.GetAmountProduct(0));
    }    

    [Test]
    public void TestCanUpgrade()
    {
        Player player = Collection.LoadModel<Player>();

        // Nếu không đủ tiền thì nâng cấp sẽ trả về NoEnoughMoney
        Assert.AreEqual(StatusController.NoEnoughMoney, PlayerController.CanUpgrade());

        player.money.Value = GameSetting.Instance.upgradeCost;
        Assert.AreEqual(StatusController.Success, PlayerController.CanUpgrade());
    }

    [Test]
    public void TestUpgrade()
    {
        Player player = Collection.LoadModel<Player>();

        // Nâng cấp thất bại nếu không đủ tiền
        Assert.AreNotEqual(StatusController.Success, PlayerController.Upgrade());

        player.money.Value = GameSetting.Instance.upgradeCost;
        Assert.AreEqual(StatusController.Success, PlayerController.Upgrade());

        Assert.AreEqual(1, player.level.Value);
        Assert.AreEqual(0, player.money.Value);
    }

    [Test]
    public void TestExpendLand()
    {
        StatusController status = PlayerController.ExpandLand();
        Assert.AreEqual(status, StatusController.NoEnoughMoney);
        Player player = Collection.LoadModel<Player>();        
        player.money.Value = 500;
        status = PlayerController.ExpandLand();
        Assert.AreEqual(status, StatusController.Success);
        
        var item = player.bagItems.Items.Find(x=>x.Type == TypeItem.Land);
        Assert.IsNotNull(item);
    }
}
