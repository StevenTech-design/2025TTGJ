using UnityEngine;

namespace TTGJ.Modules.TaskSystem
{
    /// <summary>任务事件处理器，用于处理游戏中各种可能触发任务进度更新的事件
    public class TaskEventHandler : MonoBehaviour
    {
        /// <summary> 当玩家获得物品时调用此方法更新任务进度
        public static void OnPlayerGainItem(int itemId, int quantity)
        {
            Debug.Log($"玩家获得物品: ID={itemId}, 数量={quantity}");
            TaskManager.Instance.UpdateTaskProgress(itemId, quantity);
        }

        /// <summary> 当玩家与NPC对话时调用此方法
        public static void OnPlayerTalkToNPC(int npcId)
        {
            Debug.Log($"玩家与NPC对话: NPC ID={npcId}");
            
            // 检查是否有与该NPC相关的对话任务需要完成
            CheckDialogTasks(npcId);
        }

        /// <summary> 检查对话任务
        private static void CheckDialogTasks(int npcId)
        {
            // 获取当前任务列表
            var currentTasks = TaskManager.Instance.GetCurrentTasks();
            
            foreach (var task in currentTasks)
            {
                // 检查是否是对话类任务（GoalCount为0或空）并且NPC匹配
                if ((string.IsNullOrEmpty(task.GoalCount) || task.GoalCount == "0") && task.NpcId == npcId)
                {
                    // 对话任务直接完成
                    TaskManager.Instance.CompleteTask(task.TaskId);
                    break;
                }
            }
        }

        /// <summary>当玩家完成某个目标时调用

        public static void OnPlayerCompleteObjective(string objectiveType, int objectiveId, int quantity = 1)
        {
            Debug.Log($"玩家完成目标: 类型={objectiveType}, ID={objectiveId}, 数量={quantity}");
            
            // 根据不同类型的目标更新相应的任务进度
            switch (objectiveType)
            {
                case "Item":
                    OnPlayerGainItem(objectiveId, quantity);
                    break;
                case "NPC":
                    OnPlayerTalkToNPC(objectiveId);
                    break;
                default:
                    Debug.LogWarning($"未知的目标类型: {objectiveType}");
                    break;
            }
        }
    }
}