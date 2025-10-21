using System;
using UnityEngine;

namespace TTGJ.Modules.TaskSystem
{
    /// <summary>
    /// 用户任务类，用于管理玩家当前进行中的任务状态
    /// </summary>
    [System.Serializable]
    public class UserTask
    {
        public int TaskId { get; private set; }
        public string TaskName { get; private set; }
        public int NpcId { get; private set; }
        public string GoalCount { get; private set; }
        public string Reward { get; private set; }
        public string AcceptPlotId { get; private set; }
        public string ProgressPlotId { get; private set; }
        public string CompletePlotId { get; private set; }
        public int NextGoalId { get; private set; }
        
        public int CurrentProgress { get; private set; }
        public int RequiredProgress { get; private set; }
        public bool IsCompleted { get { return CurrentProgress >= RequiredProgress; } }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserTask(TaskData data)
        {
            TaskId = data.id;
            TaskName = data.name;
            NpcId = data.npcId;
            GoalCount = data.goalCount;
            Reward = data.reward;
            AcceptPlotId = data.acceptPlotId;
            ProgressPlotId = data.progressPlotId;
            CompletePlotId = data.completePlotId;
            NextGoalId = data.nextGoalId;
            
            // 解析所需进度
            ParseRequiredProgress();
            CurrentProgress = 0;
        }
        
        /// <summary>
        /// 解析任务所需进度
        /// </summary>
        private void ParseRequiredProgress()
        {
            if (string.IsNullOrEmpty(GoalCount) || GoalCount == "0")
            {
                // 对话类任务或无需进度的任务
                RequiredProgress = 1;
                CurrentProgress = 1; // 自动完成
                return;
            }
            
            string[] goalParts = GoalCount.Split('#');
            if (goalParts.Length == 2 && int.TryParse(goalParts[1], out int required))
            {
                RequiredProgress = required;
            }
            else
            {
                RequiredProgress = 1;
            }
        }
        
        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdateProgress(int amount)
        {
            if (!IsCompleted)
            {
                CurrentProgress = Mathf.Min(CurrentProgress + amount, RequiredProgress);
                Debug.Log($"任务 {TaskName} 进度更新: {CurrentProgress}/{RequiredProgress}");
            }
        }
        
        /// <summary>
        /// 获取进度百分比
        /// </summary>
        public float GetProgressPercentage()
        {
            if (RequiredProgress == 0)
                return 0;
            return (float)CurrentProgress / RequiredProgress;
        }
        
        /// <summary>
        /// 获取任务描述文本
        /// </summary>
        public string GetTaskDescription()
        {
            if (string.IsNullOrEmpty(GoalCount) || GoalCount == "0")
            {
                return $"与NPC对话完成任务";
            }
            
            string[] goalParts = GoalCount.Split('#');
            if (goalParts.Length == 2)
            {
                string itemName = GetItemName(int.Parse(goalParts[0]));
                return $"收集{itemName} × {goalParts[1]}";
            }
            return TaskName;
        }
        
        /// <summary>
        /// 根据道具ID获取道具名称
        /// </summary>
        private string GetItemName(int itemId)
        {
            // 这里应该从道具配置中获取名称，现在使用简单的映射
            switch (itemId)
            {
                case 2001: return "土豆";
                case 2016: return "杂草";
                default: return "物品";
            }
        }
        
        /// <summary>
        /// 获取接受任务时的剧情文本
        /// </summary>
        public string GetAcceptPlotText()
        {
            return TaskManager.Instance.GetPlotText(AcceptPlotId);
        }
        
        /// <summary>
        /// 获取任务进行中的剧情文本
        /// </summary>
        public string GetProgressPlotText()
        {
            return TaskManager.Instance.GetPlotText(ProgressPlotId);
        }
        
        /// <summary>
        /// 获取任务完成时的剧情文本
        /// </summary>
        public string GetCompletePlotText()
        {
            return TaskManager.Instance.GetPlotText(CompletePlotId);
        }
    }
}