using NUnit.Framework;
using Modules;
using System.IO;
using Models;
using UnityEngine;
[TestFixture]
public class DataHelperTests
{
    private string saveFilePath;

    [Test]
    public void TestSetValue()
    {
        // Arrange: lấy instance và gán giá trị
        DataHelper helper = DataHelper.Instance;
        helper.Set("testKey", 42);

        // Act: lấy lại giá trị
        int result = helper.Get<int>("testKey", 0);

        // Assert: kiểm tra giá trị phải bằng 42
        Assert.AreEqual(42, result);
    }

    [Test]
    public void TestGetValue()
    {
        // Arrange: lấy instance và gán giá trị
        DataHelper helper = DataHelper.Instance;

        helper.Set("testKey", 42);

        // Act: lấy lại giá trị
        int result = helper.Get<int>("testKey", 0);

        // Assert: kiểm tra giá trị phải bằng 42
        Assert.AreEqual(42, result);
    }

    [Test]
    public void TestSaveValue()
    {
        // Xác định đường dẫn file lưu dựa trên persistentDataPath
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.dat");

        // Xóa file nếu tồn tại
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        // Arrange: lấy instance và gán giá trị
        DataHelper helper = DataHelper.Instance;
        helper.Save();

        Assert.True(File.Exists(saveFilePath));        
    }

    [Test]
    public void TestSavePlayer()
    {
        DataHelper.Instance.ClearData();

        Player player = Collection.LoadModel<Player>();

        player.money.Value = 1000;
        Collection.SaveModel(player, true);
        DataHelper.Instance.ClearCache();
        Collection.Clear();

        player = Collection.LoadModel<Player>();

        Assert.AreEqual(player.money.Value, 1000);
    }
}
