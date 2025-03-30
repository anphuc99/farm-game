using Controllers;
using Models;
using Modules;
using NUnit.Framework;

[TestFixture]
public class ShopControllerTest
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

    [Test]
    public void TestCanBuyProduct()
    {
        Player player = Collection.LoadModel<Player>();

        player.money.Value = 0;
        Assert.AreEqual(StatusController.NoEnoughMoney, ShopController.CanBuyProduct(IdItemTest3));

        player.money.Value = 400;
        Assert.AreEqual(StatusController.Success, ShopController.CanBuyProduct(IdItemTest3));
    }

    [Test]
    public void TestBuyProduct()
    {
        Player player = Collection.LoadModel<Player>();

        // Thử mua sản phẩm khi không đủ tiền
        player.money.Value = 0;
        Assert.AreNotEqual(StatusController.Success, ShopController.BuyProduct(IdItemTest3));

        // Mua sản phẩm khi có đủ tiền
        player.money.Value = 400;
        Assert.AreEqual(StatusController.Success, ShopController.BuyProduct(IdItemTest3));

        BagItem item = player.bagItems.Items.Find(x => x.id == IdItemTest3);
        Assert.IsNotNull(item);
        Assert.AreEqual(10, item.amount.Value);

        // Mua sản phẩm thêm lần nữa
        player.money.Value = 400;
        Assert.AreEqual(StatusController.Success, ShopController.BuyProduct(IdItemTest3));
        item = player.bagItems.Items.Find(x => x.id == IdItemTest3);
        Assert.IsNotNull(item);
        Assert.AreEqual(20, item.amount.Value);
    }
}
