using UnityEngine;
using UnityEngine.UI;
using TTGJ.Build;

namespace TTGJ.Tests
{
    /// <summary>
    /// 建筑测试UI控制器
    /// 用于在测试场景中快速测试建筑放置系统
    /// </summary>
    public class BuildTestUI : MonoBehaviour
    {
        [Header("系统引用")]
        [SerializeField]
        private PlacementSystem placementSystem;

        [Header("UI按钮")]
        [SerializeField]
        private Button placeFloorButton;
        [SerializeField]
        private Button placeFurniture1Button;
        [SerializeField]
        private Button placeFurniture2Button;
        [SerializeField]
        private Button removeButton;
        [SerializeField]
        private Button clearAllButton;

        [Header("对象ID配置")]
        [SerializeField]
        private int floorID = 0;
        [SerializeField]
        private int furniture1ID = 1;
        [SerializeField]
        private int furniture2ID = 2;

        [Header("调试信息")]
        [SerializeField]
        private Text debugText;

        private void Start()
        {
            if (placementSystem == null)
            {
                Debug.LogError("PlacementSystem 未分配！");
                return;
            }

            SetupButtons();
            UpdateDebugText("就绪 - 选择一个操作");
        }

        private void SetupButtons()
        {
            if (placeFloorButton != null)
            {
                placeFloorButton.onClick.AddListener(() => OnPlaceObject(floorID, "地板"));
            }

            if (placeFurniture1Button != null)
            {
                placeFurniture1Button.onClick.AddListener(() => OnPlaceObject(furniture1ID, "家具1"));
            }

            if (placeFurniture2Button != null)
            {
                placeFurniture2Button.onClick.AddListener(() => OnPlaceObject(furniture2ID, "家具2"));
            }

            if (removeButton != null)
            {
                removeButton.onClick.AddListener(OnRemoveMode);
            }

            if (clearAllButton != null)
            {
                clearAllButton.onClick.AddListener(OnClearAll);
            }
        }

        private void OnPlaceObject(int objectID, string objectName)
        {
            try
            {
                Debug.Log("OnPlaceObject: " + objectID + " " + objectName + " " + placementSystem);
                placementSystem.StartPlacement(objectID);
                UpdateDebugText($"放置模式: {objectName} (ID: {objectID})\n左键放置，ESC退出");
            }
            catch (System.Exception e)
            {
                UpdateDebugText($"错误: {e.Message}");
                Debug.LogError($"放置对象失败: {e.Message}");
            }
        }

        private void OnRemoveMode()
        {
            placementSystem.StartRemoving();
            UpdateDebugText("移除模式\n左键移除对象，ESC退出");
        }

        private void OnClearAll()
        {
            // 重新加载场景来清空所有对象
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            UpdateDebugText("已清空所有对象");
        }

        private void UpdateDebugText(string message)
        {
            if (debugText != null)
            {
                debugText.text = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
            }
            Debug.Log($"BuildTest: {message}");
        }

        private void Update()
        {
            // 显示帮助信息
            if (Input.GetKeyDown(KeyCode.H))
            {
                UpdateDebugText(
                    "快捷键帮助:\n" +
                    "1 - 放置地板\n" +
                    "2 - 放置家具1\n" +
                    "3 - 放置家具2\n" +
                    "R - 移除模式\n" +
                    "ESC - 退出当前模式\n" +
                    "H - 显示帮助");
            }

            // 快捷键
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnPlaceObject(floorID, "地板");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnPlaceObject(furniture1ID, "家具1");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                OnPlaceObject(furniture2ID, "家具2");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                OnRemoveMode();
            }
        }
    }
}

