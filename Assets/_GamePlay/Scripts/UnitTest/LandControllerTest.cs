using Controllers;
using Models;
using Modules;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class LandControllerTest
{
    private int IdItemTest = 1001;
    private int IdItemTest2 = 3001;

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

    private int GetTimeMaturity()
    {
        ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == IdItemTest);
        return itemData.timeToMaturity;
    }

    private int GetTimeProduction()
    {
        ItemData itemData = ItemDatas.Instance.items.Find(x => x.id == IdItemTest);
        return itemData.productionTime;
    }

    [Test]
    public void TestGetLands()
    {
        var lands = LandController.GetLands();
        Assert.AreEqual(3, lands.Items.Count);
    }

    [Test]
    public void TestGetInfoLand()
    {
        // Kiểm tra sau khi trồng cây thành công trên đất 0
        var cultivationStatus = PlayerController.Cultivation(IdItemTest, 0);
        Assert.AreEqual(StatusController.Success, cultivationStatus);

        var lands = Collection.LoadModel<Player>().lands;
        Assert.IsNotNull(lands.Items[0].Agriculture);
        Assert.AreEqual(IdItemTest, lands.Items[0].Agriculture.id);
    }

    [Test]
    public void TestGetInfoAgriculture()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        var agriculture = LandController.GetInfoAgriculture(0);
        Assert.IsNotNull(agriculture);
        Assert.AreEqual(IdItemTest, agriculture.id);
    }

    [Test]
    public void TestCheckAgricultureMaturity()
    {
        PlayerController.Cultivation(IdItemTest, 0);

        // Chưa đủ thời gian trưởng thành
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() - 10);
        Assert.AreEqual(StatusController.Immature, LandController.CheckAgricultureMaturity(0));

        // Đủ thời gian trưởng thành
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity());
        Assert.AreEqual(StatusController.Success, LandController.CheckAgricultureMaturity(0));

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + 10);
        Assert.AreEqual(StatusController.Success, LandController.CheckAgricultureMaturity(0));
    }

    [Test]
    public void TestGetRemainingTimeMaturity()
    {
        PlayerController.Cultivation(IdItemTest, 0);

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() - 10);
        Assert.AreEqual(10, LandController.GetRemainingTimeMaturity(0));

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity());
        Assert.AreEqual(0, LandController.GetRemainingTimeMaturity(0));

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + 10);
        Assert.AreEqual(0, LandController.GetRemainingTimeMaturity(0));

        Player player = Collection.LoadModel<Player>();
        player.level.Value = 5;
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity()*0.75);
        Assert.AreEqual(0, LandController.GetRemainingTimeMaturity(0));
        
        player.level.Value = 10;
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() * 0.5);
        Assert.AreEqual(0, LandController.GetRemainingTimeMaturity(0));
    }

    [Test]
    public void TestGetProgressMaturity()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() / 2);
        Assert.AreEqual(50, LandController.GetProgressMaturity(0));
    }

    [Test]
    public void TestGetQuantityProduced()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10);
        Assert.AreEqual(10, LandController.GetQuantityProduced(0));

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 100);
        Assert.AreEqual(40, LandController.GetQuantityProduced(0));

        Player player = Collection.LoadModel<Player>();
        player.level.Value = 5;
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10);
        Assert.AreEqual(15, LandController.GetQuantityProduced(0));
        
        player.level.Value = 10;
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10);
        Assert.AreEqual(20, LandController.GetQuantityProduced(0));

    }

    [Test]
    public void TestGetAmountProduct()
    {
        PlayerController.Cultivation(IdItemTest, 0);

        // Khi chưa trưởng thành: không có sản phẩm để thu hoạch
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() - 1);
        Assert.AreEqual(0, LandController.GetAmountProduct(0));

        // Sau khi đã trưởng thành: số sản phẩm thu hoạch được bằng số lượng sản xuất
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10);
        Assert.AreEqual(10, LandController.GetAmountProduct(0));

        // Giả lập đã thu hoạch 5 sản phẩm
        Player player = Collection.LoadModel<Player>();
        var land = player.lands.Items[0];
        land.Agriculture.amountProductPicked.Value = 5;
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10);
        Assert.AreEqual(5, LandController.GetAmountProduct(0));
    }

    [Test]
    public void TestCheckProducedLimited()
    {
        PlayerController.Cultivation(IdItemTest, 0);

        // Chưa đạt giới hạn sản xuất
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10);
        Assert.AreEqual(StatusController.ProducedNoLimited, LandController.CheckProducedLimited(0));

        // Đạt giới hạn sản xuất
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 100);
        Assert.AreEqual(StatusController.ProducedLimited, LandController.CheckProducedLimited(0));
    }

    [Test]
    public void TestGetRemainingTimeProduct()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10 + 100);
        Assert.AreEqual(GetTimeProduction() - 100, LandController.GetRemainingTimeProduct(0));
    }

    [Test]
    public void TestGetProgressProduct()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 10 + GetTimeProduction() / 2);
        Assert.AreEqual(50, LandController.GetProgressProduct(0));
    }

    [Test]
    public void TestGetRemainingTimeDead()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 40 + 1800);
        Assert.AreEqual(1800, LandController.GetRemainingTimeDead(0));

        Player player = Collection.LoadModel<Player>();
        player.level.Value = 10;
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity()/2 + GetTimeProduction() * 20 + 1800);
        Assert.AreEqual(1800, LandController.GetRemainingTimeDead(0));
    }

    [Test]
    public void TestGetProgressDead()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 40 + 900);
        Assert.AreEqual(25, LandController.GetProgressDead(0));
    }

    [Test]
    public void TestRemoveAgriculture()
    {
        PlayerController.Cultivation(IdItemTest, 0);
        var status = LandController.RemoveAgriculture(0);
        Assert.AreEqual(StatusController.Success, status);

        var lands = Collection.LoadModel<Player>().lands;
        var land = lands.Items[0];
        Assert.IsNull(land.Agriculture);
    }

    [Test]
    public void TestAddLand()
    {
        Player player = Collection.LoadModel<Player>();
        int countLand = player.lands.Items.Count;
        // Trường hợp không có vật phẩm trong túi: không thành công
        var addStatus = LandController.AddLand(IdItemTest2, Vector2.zero);
        Assert.AreNotEqual(StatusController.Success, addStatus);

        // Thêm vật phẩm cần thiết vào túi
        player.bagItems.Add(new BagItem()
        {
            id = IdItemTest2,
            amount = new BindableAntiCheat<int>(1)
        });

        addStatus = LandController.AddLand(IdItemTest2, Vector2.zero);
        Assert.AreEqual(StatusController.Success, addStatus);
        Assert.AreEqual(countLand + 1, player.lands.Items.Count);

        var bagItem2 = player.bagItems.Items.Find(x => x.id == IdItemTest2);
        Assert.IsNull(bagItem2);
    }

    [Test]
    public void TestSetPositionLand()
    {
        Player player = Collection.LoadModel<Player>();
        var status = LandController.SetPositionLand(0, new Vector2(3, 3));
        Assert.AreEqual(StatusController.Success, status);

        var land = player.lands.Items[0];
        Assert.AreEqual(new System.Numerics.Vector2(3, 3), land.posistion);
    }

    [Test]
    public void TestGetStatusAgriculture()
    {
        Player player = Collection.LoadModel<Player>();
        PlayerController.Cultivation(IdItemTest, 0);

        Assert.AreEqual(LandController.GetStatusAgriculture(0), StatusController.AgricultureImmature);

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(31);

        Assert.AreEqual(LandController.GetStatusAgriculture(0), StatusController.AgricultureHalfMature);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(61);

        Assert.AreEqual(LandController.GetStatusAgriculture(0), StatusController.AgricultureMature);

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 40 + 3500);

        Assert.AreEqual(LandController.GetStatusAgriculture(0), StatusController.AgricultureMatureLimit);

        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(GetTimeMaturity() + GetTimeProduction() * 40 + 3600);

        Assert.AreEqual(LandController.GetStatusAgriculture(0), StatusController.AgricultureDead);
    }
}
