using UnityEngine;
using UnityEngine.UI;

namespace TTGJ.Modules.TaskSystem
{
    /// <summary>
    /// 任务项UI类，用于显示单个任务的详细信息
    /// </summary>
    public class TaskItemUI : MonoBehaviour
    {
        [Header("任务项UI组件")]
        [SerializeField] private Text taskNameText;
        [SerializeField] private Text taskDescriptionText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text progressText;
        [SerializeField] private Image taskIcon;
        [SerializeField] private GameObject completedMark;
        
        private UserTask currentTask;
        
        /// <summary>
        /// 设置任务项数据
        /// </summary>
        public void Setup(UserTask task)
        {
            currentTask = task;
            
            // 更新UI显示
            UpdateTaskUI();
        }
        
        /// <summary>
        /// 更新任务UI显示
        /// </summary>
        private void UpdateTaskUI()
        {
            if (currentTask == null)
                return;
            
            // 设置任务名称
            if (taskNameText != null)
            {
                taskNameText.text = currentTask.TaskName;
            }
            
            // 设置任务描述
            if (taskDescriptionText != null)
            {
                taskDescriptionText.text = currentTask.GetTaskDescription();
            }
            
            // 设置进度条
            if (progressSlider != null)
            {
                progressSlider.value = currentTask.GetProgressPercentage();
            }
            
            // 设置进度文本
            if (progressText != null)
            {
                progressText.text = $"{currentTask.CurrentProgress}/{currentTask.RequiredProgress}";
            }
            
            // 设置完成标记
            if (completedMark != null)
            {
                completedMark.SetActive(currentTask.IsCompleted);
            }
            
            // 根据任务状态设置UI样式
            SetTaskItemStyle();
        }
        
        /// <summary>
        /// 设置任务项样式
        /// </summary>
        private void SetTaskItemStyle()
        {
            // 根据任务是否完成设置不同的样式
            Color nameColor = currentTask.IsCompleted ? Color.gray : Color.white;
            Color descColor = currentTask.IsCompleted ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.9f, 0.9f, 0.9f);
            
            if (taskNameText != null)
            {
                taskNameText.color = nameColor;
            }
            
            if (taskDescriptionText != null)
            {
                taskDescriptionText.color = descColor;
            }
        }
        
        /// <summary>
        /// 任务项点击事件（可选实现）
        /// </summary>
        public void OnTaskItemClick()
        {
            // 这里可以添加点击任务项的交互逻辑
            // 例如：显示任务详情、自动寻路到任务目标等
            Debug.Log($"点击了任务: {currentTask.TaskName}");
        }
    }
}