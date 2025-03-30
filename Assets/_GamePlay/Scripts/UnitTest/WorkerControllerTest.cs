using Controllers;
using Models;
using Modules;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class WorkerControllerTest
{
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
    public void TestGetWorkers()
    {
        var workers = WorkerController.GetWorkers();
        Assert.AreEqual(workers.Items.Count, 1);
    }

    [Test]
    public void TestGetRemainingTimeWork()
    {
        WorkerController.UpdateWorker(0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(60);
        Assert.AreEqual(WorkerController.GetRemainingTimeWork(0), 60);
    }

    [Test]
    public void TestGetProgressWork()
    {
        WorkerController.UpdateWorker(0);
        DateTimeHelper.dateTimeNow = DateTime.Now.AddSeconds(60);
        Assert.AreEqual(WorkerController.GetProgressWork(0), 50);
    }

    [Test]
    public void TestUpdateWorker()
    {
        var worker = Collection.LoadModel<Player>().workers.Items[0];
        WorkerController.UpdateWorker(0);

        Assert.IsTrue(worker.workingInLand != -1);
    }

    [Test]
    public void TestHireWorker()
    {
        StatusController status = WorkerController.HireWorker();
        Assert.AreEqual(status, StatusController.NoEnoughMoney);
        Player player = Collection.LoadModel<Player>();
        int curWorker = player.workers.Items.Count;
        player.money.Value = 500;
        status = WorkerController.HireWorker();
        Assert.AreEqual(status, StatusController.Success);
        Assert.AreEqual(player.workers.Items.Count, curWorker + 1);
    }

    [Test]
    public void TestAutoWork()
    {
        Player player = Collection.LoadModel<Player>();
        player.lastTimePlayGame = DateTimeHelper.GetTimeStampNow();
        DateTimeHelper.dateTimeNow = DateTime.Now.AddDays(100);
        WorkerController.AutoWork();

        var bagItems = player.bagItems;

        var findSeed = bagItems.Items.Find(x => x.Type == TypeItem.Seeds);
        Assert.IsNull(findSeed);

        var findProduct1 = bagItems.Items.Find(x => x.id == 2001);
        Assert.IsNotNull(findProduct1);
        Assert.AreEqual(findProduct1.amount.Value, 400);

        var findProduct2 = bagItems.Items.Find(x => x.id == 2002);
        Assert.IsNotNull(findProduct2);
        Assert.AreEqual(findProduct2.amount.Value, 400);

        var findProduct3 = bagItems.Items.Find(x => x.id == 2003);
        Assert.IsNotNull(findProduct3);
        Assert.AreEqual(findProduct3.amount.Value, 200);
    }
}
