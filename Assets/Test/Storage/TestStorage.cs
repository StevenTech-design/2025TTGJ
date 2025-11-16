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
            StorageManager.Instance.Initialize("TestSaveData");
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
            // Arrange
            var inventory = new InventoryData();
            inventory.AddItem(1001, 100);
            inventory.AddItem(1002, 200);

            // Act - 保存
            StorageManager.Instance.SaveJson(TEST_KEY, inventory);
            yield return null;

            // Act - 加载
            InventoryData data = null;
            StorageManager.Instance.LoadJson(TEST_KEY,out data);

            // Assert
            Assert.IsNotNull(data);
        }
    }
}