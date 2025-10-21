using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TTGJ.UI;

namespace TTGJ.Modules.TaskSystem
{
    /// <summary>
    /// 任务面板UI类，用于显示和管理任务列表
    /// </summary>
    public class TaskPanel : UIPanel
    {
        [Header("UI引用")]
        [SerializeField] private Transform taskListContent;
        [SerializeField] private GameObject taskItemPrefab;
        [SerializeField] private Button closeButton;
        
        private List<TaskItemUI> taskItems = new List<TaskItemUI>();
        
        private void Awake()
        {
            // 注册关闭按钮事件
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
        
        private void OnEnable()
        {
            // 当面板激活时，刷新任务列表
            RefreshTaskList();
        }
        
        public override void Show()
        {
            base.Show();
            gameObject.SetActive(true);
            RefreshTaskList();
        }
        
        public override void Hide()
        {
            base.Hide();
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 刷新任务列表
        /// </summary>
        public void RefreshTaskList()
        {
            // 清除现有任务项
            ClearTaskItems();
            
            // 获取当前任务列表
            List<UserTask> tasks = TaskManager.Instance.GetCurrentTasks();
            
            // 创建任务项
            foreach (var task in tasks)
            {
                CreateTaskItem(task);
            }
            
            // 如果没有任务，显示提示
            if (tasks.Count == 0)
            {
                ShowNoTaskTip();
            }
        }
        
        /// <summary>
        /// 创建任务项UI
        /// </summary>
        private void CreateTaskItem(UserTask task)
        {
            if (taskItemPrefab != null)
            {
                GameObject itemObj = Instantiate(taskItemPrefab, taskListContent);
                TaskItemUI taskItem = itemObj.GetComponent<TaskItemUI>();
                
                if (taskItem != null)
                {
                    taskItem.Setup(task);
                    taskItems.Add(taskItem);
                }
                else
                {
                    Debug.LogError("TaskItemUI组件未找到!");
                    Destroy(itemObj);
                }
            }
        }
        
        /// <summary>
        /// 清除所有任务项
        /// </summary>
        private void ClearTaskItems()
        {
            foreach (var item in taskItems)
            {
                Destroy(item.gameObject);
            }
            taskItems.Clear();
            
            // 清除所有子对象（包括可能的提示文本）
            for (int i = 0; i < taskListContent.childCount; i++)
            {
                Destroy(taskListContent.GetChild(i).gameObject);
            }
        }
        
        /// <summary>
        /// 显示无任务提示
        /// </summary>
        private void ShowNoTaskTip()
        {
            GameObject tipObj = new GameObject("NoTaskTip");
            tipObj.transform.SetParent(taskListContent, false);
            
            Text tipText = tipObj.AddComponent<Text>();
            tipText.text = "当前没有任务，请寻找NPC接取任务";
            tipText.alignment = TextAnchor.MiddleCenter;
            tipText.color = Color.gray;
            
            // 设置RectTransform
            RectTransform rt = tipObj.GetComponent<RectTransform>();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
        }
        
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void OnCloseButtonClick()
        {
            Hide();
        }
    }
}