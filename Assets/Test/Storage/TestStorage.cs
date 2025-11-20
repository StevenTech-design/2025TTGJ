// 文件: Assets/Tests/PlayMode/InventoryStoragePlayModeTests.cs
using System.Collections;
using NUnit.Framework;
using TTGJ.Storage;
using TTGJ.Inventory;
using UnityEngine;
using UnityEngine.TestTools;

namespace TTGJ.Tests.PlayMode
{
    /// <summary>
    /// Inventory 存储功能 PlayMode 测试
    /// </summary>
    public class InventoryStoragePlayModeTests
    {
        private const string TEST_KEY = "test_inventory_playmode";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            StorageManager.Instance.Init();
            if (StorageManager.Instance.Exists(TEST_KEY))
            {
                StorageManager.Instance.Delete(TEST_KEY);
            }
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (StorageManager.Instance.Exists(TEST_KEY))
            {
                StorageManager.Instance.Delete(TEST_KEY);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_SaveAndLoad_InPlayMode()
        {
            // Arrange - 创建测试数据
            var originalInventory = new InventoryData();
            originalInventory.AddItem(1001, 100);
            originalInventory.AddItem(1002, 200);
            originalInventory.AddItem(1003, 50);

            // 生成原始序列化字符串用于比较
            string originalJson = originalInventory.Serialize(originalInventory);

            // Act - 保存
            bool saveResult = StorageManager.Instance.SaveJson(TEST_KEY, originalInventory);
            Assert.IsTrue(saveResult, "保存应该成功");
            yield return null;

            // Act - 加载
            InventoryData loadedInventory = null;
            StorageManager.Instance.LoadJson(TEST_KEY, out loadedInventory);

            // Assert - 基本检查
            Assert.IsNotNull(loadedInventory, "加载的数据不应该为 null");

            // Assert - 通过序列化字符串比较验证数据完整性
            string loadedJson = loadedInventory.Serialize(loadedInventory);
            Assert.AreEqual(originalJson, loadedJson, "保存和加载后的数据应该完全一致");

            // Assert - 进一步验证：加载的数据应该能正确累加物品（测试数据完整性）
            bool addResult = loadedInventory.AddItem(1001, 50); // 尝试添加已存在的物品
            Assert.IsTrue(addResult, "应该能成功添加已存在的物品");

            // 再次序列化，应该与原始数据不同（因为数量增加了）
            string modifiedJson = loadedInventory.Serialize(loadedInventory);
            Assert.AreNotEqual(originalJson, modifiedJson, "修改后的数据应该与原始数据不同");
        }
    }
}