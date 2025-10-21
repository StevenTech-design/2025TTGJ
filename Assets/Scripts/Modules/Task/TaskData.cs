using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Modules.TaskSystem
{
    /// summary：任务数据类，用于存储任务的基本信息
 
    [System.Serializable]
    public class TaskData
    {
        public int id;                 // 任务唯一ID
        public string name;            // 任务名称
        public int npcId;              // 任务NPC ID
        public string goalCount;       // 任务要求（格式：道具ID#数量）
        public string reward;          // 奖励（格式：道具ID#数量）
        public string acceptPlotId;    // 接受时剧情ID
        public string progressPlotId;  // 进行中剧情ID
        public string completePlotId;  // 完成时剧情ID
        public int nextGoalId;         // 关联的下一个任务ID
    }

    /// summary：剧情数据类，用于存储剧情类信息
    [System.Serializable]
    public class PlotData
    {
        public string id;              // 剧情唯一ID
        public string name;            // 剧情备注
        public int npcId;              // 对话NPC ID
        public string textTime;        // 剧情文字#时间
        public string emoTime;         // 剧情表情#时间
        public int nextId;             // 下个剧情ID
    }

    /// summary：NPC对话数据类，用于存储NPC的碎碎念

    [System.Serializable]
    public class NpcDialogData
    {
        public string id;              // NPC唯一ID
        public string name;            // 碎碎念备注
        public int npcId;              // NPC类型ID (0甜菜 1羊 2猴子 3绿羊)
        public string textTime;        // 对话#时间
        public string emoTime;         // 表情#时间
    }
}