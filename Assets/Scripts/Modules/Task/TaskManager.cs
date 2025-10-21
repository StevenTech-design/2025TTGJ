using System.Collections.Generic;
using UnityEngine;
using TTGJ.Framework;

namespace TTGJ.Modules.TaskSystem
{
    /// <summary>
    /// 任务管理器，负责任务系统的核心逻辑
    /// </summary>
    public class TaskManager : Singleton<TaskManager>
    {
        // 任务数据字典，通过任务ID索引
        private Dictionary<int, TaskData> taskDataDic = new Dictionary<int, TaskData>();
        // 剧情数据字典
        private Dictionary<string, PlotData> plotDataDic = new Dictionary<string, PlotData>();
        // NPC对话数据字典
        private Dictionary<string, NpcDialogData> npcDialogDataDic = new Dictionary<string, NpcDialogData>();
        
        // 当前已接任务列表
        private List<UserTask> currentTasks = new List<UserTask>();
        // 已完成任务列表
        private List<int> completedTaskIds = new List<int>();

        /// <summary>
        /// 初始化任务数据
        /// </summary>
        public void Initialize()
        {
            LoadTaskData();
            LoadPlotData();
            LoadNpcDialogData();
            LoadUserTaskProgress();
        }

        /// <summary>
        /// 加载任务数据
        /// </summary>
        private void LoadTaskData()
        {
            // 根据数据表信息加载任务数据
            AddTaskData(new TaskData {
                id = 1001, 
                name = "初来乍到", 
                npcId = 0, 
                goalCount = "0", 
                reward = "1001#10", 
                acceptPlotId = "M_0001", 
                progressPlotId = "M_0001_1", 
                completePlotId = "M_0001_2", 
                nextGoalId = 1002
            });

            AddTaskData(new TaskData {
                id = 1002, 
                name = "种十个土豆", 
                npcId = 0, 
                goalCount = "2001#10", 
                reward = "1013#1", 
                acceptPlotId = "M_0002", 
                progressPlotId = "M_0002_1", 
                completePlotId = "M_0002_2", 
                nextGoalId = 1003
            });

            AddTaskData(new TaskData {
                id = 1003, 
                name = "清理十个杂草", 
                npcId = 0, 
                goalCount = "2016#10", 
                reward = "1004#10", 
                acceptPlotId = "M_0003", 
                progressPlotId = "M_0003_1", 
                completePlotId = "M_0003_2", 
                nextGoalId = 1004
            });
        }

        // /// <summary>
        // /// 加载剧情数据
        // /// </summary>
        // private void LoadPlotData()
        // {
        //     // 根据数据表信息加载剧情数据
        //     AddPlotData(new PlotData {
        //         id = "M_0001", 
        //         name = "接受初始任务", 
        //         npcId = 0, 
        //         textTime = "先到农场试试手吧！#2", 
        //         emoTime = "微笑#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0001_1", 
        //         name = "进行中剧情", 
        //         npcId = 0, 
        //         textTime = "成为农场主，种植是关键！#2", 
        //         emoTime = "思考#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0001_2", 
        //         name = "完成初始任务", 
        //         npcId = 0, 
        //         textTime = "你已经是合格的农场主了！#2", 
        //         emoTime = "开心#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0002", 
        //         name = "接受种植任务", 
        //         npcId = 0, 
        //         textTime = "想成为合格的农场主，就先种10个土豆吧！#2", 
        //         emoTime = "鼓励#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0002_1", 
        //         name = "种植任务进行中", 
        //         npcId = 0, 
        //         textTime = "真奇怪，甜菜种子总是给我更多收获！#2", 
        //         emoTime = "疑惑#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0002_2", 
        //         name = "完成种植任务", 
        //         npcId = 0, 
        //         textTime = "你种好了，可以让我作物生长更快！#2", 
        //         emoTime = "满意#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0003", 
        //         name = "接受清理任务", 
        //         npcId = 0, 
        //         textTime = "我讨厌杂草，它们总是给我更多麻烦！#2", 
        //         emoTime = "生气#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0003_1", 
        //         name = "清理任务进行中", 
        //         npcId = 0, 
        //         textTime = "清洁是时代发展的重要因素，真好！#2", 
        //         emoTime = "认真#1", 
        //         nextId = 0
        //     });

        //     AddPlotData(new PlotData {
        //         id = "M_0003_2", 
        //         name = "完成清理任务", 
        //         npcId = 0, 
        //         textTime = "高效率！没有了杂草，农场看起来清爽多了！#2", 
        //         emoTime = "高兴#1", 
        //         nextId = 0
        //     });

        //     // 开场主线剧情
        //     AddPlotData(new PlotData {
        //         id = "G001", 
        //         name = "开场主线", 
        //         npcId = 11000, 
        //         textTime = "欢迎来到尾砂岛！#3", 
        //         emoTime = "无#0", 
        //         nextId = 1
        //     });

        //     // 日常对话剧情
        //     AddPlotData(new PlotData {
        //         id = "F001", 
        //         name = "npc日常对话", 
        //         npcId = 11002, 
        //         textTime = "想念做发型和帽子悲伤表情#3", 
        //         emoTime = "悲伤#1", 
        //         nextId = 0
        //     });
        // }

        /// <summary>
        /// 添加剧情数据到字典
        /// </summary>
        private void AddPlotData(PlotData data)
        {
            if (!plotDataDic.ContainsKey(data.id))
            {
                plotDataDic.Add(data.id, data);
            }
        }

        // /// <summary>
        // /// 加载NPC对话数据
        // /// </summary>
        // private void LoadNpcDialogData()
        // {
        //     // 根据数据表信息加载NPC对话数据
        //     AddNpcDialogData(new NpcDialogData {
        //         id = "N_11000", 
        //         name = "甜菜女士", 
        //         npcId = 0, 
        //         textTime = "0#2", 
        //         emoTime = "表情#1"
        //     });

        //     AddNpcDialogData(new NpcDialogData {
        //         id = "N_11001", 
        //         name = "花椒变成羊", 
        //         npcId = 1, 
        //         textTime = "1#2", 
        //         emoTime = "表情#1"
        //     });

        //     AddNpcDialogData(new NpcDialogData {
        //         id = "N_11002", 
        //         name = "程序员", 
        //         npcId = 2, 
        //         textTime = "2#2", 
        //         emoTime = "表情#1"
        //     });

        //     AddNpcDialogData(new NpcDialogData {
        //         id = "N_11003", 
        //         name = "一只隐藏的羊", 
        //         npcId = 3, 
        //         textTime = "3#2", 
        //         emoTime = "表情#1"
        //     });
        // }

        /// <summary>
        /// 添加NPC对话数据到字典
        /// </summary>
        private void AddNpcDialogData(NpcDialogData data)
        {
            if (!npcDialogDataDic.ContainsKey(data.id))
            {
                npcDialogDataDic.Add(data.id, data);
            }
        }
        
        /// <summary>
        /// 根据剧情ID获取剧情文本
        /// </summary>
        public string GetPlotText(string plotId)
        {
            if (string.IsNullOrEmpty(plotId) || !plotDataDic.ContainsKey(plotId))
            {
                return string.Empty;
            }
            
            PlotData plotData = plotDataDic[plotId];
            // 解析textTime，只返回文本部分
            string[] textParts = plotData.textTime.Split('#');
            return textParts.Length > 0 ? textParts[0] : string.Empty;
        }
        
        /// <summary>
        /// 根据剧情ID获取剧情表情
        /// </summary>
        public string GetPlotEmotion(string plotId)
        {
            if (string.IsNullOrEmpty(plotId) || !plotDataDic.ContainsKey(plotId))
            {
                return string.Empty;
            }
            
            PlotData plotData = plotDataDic[plotId];
            // 解析emoTime，只返回表情部分
            string[] emoParts = plotData.emoTime.Split('#');
            return emoParts.Length > 0 ? emoParts[0] : string.Empty;
        }
        
        /// <summary>
        /// 根据NPC ID获取NPC对话数据
        /// </summary>
        public NpcDialogData GetNpcDialogData(int npcId)
        {
            foreach (var dialog in npcDialogDataDic.Values)
            {
                if (dialog.npcId == npcId)
                {
                    return dialog;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加任务数据到字典
        /// </summary>
        private void AddTaskData(TaskData data)
        {
            if (!taskDataDic.ContainsKey(data.id))
            {
                taskDataDic.Add(data.id, data);
            }
        }

        /// <summary>
        /// 加载剧情数据
        /// </summary>
        private void LoadPlotData()
        {
            // 模拟加载剧情数据
            AddPlotData(new PlotData {
                id = "G001", 
                name = "开场主线", 
                npcId = 111000, 
                textTime = "欢迎来到尾砂岛！#无", 
                emoTime = "", 
                nextId = 0
            });

            AddPlotData(new PlotData {
                id = "F001", 
                name = "npc日常对话", 
                npcId = 111002, 
                textTime = "想念做发型和帽子#悲伤表情", 
                emoTime = "", 
                nextId = 0
            });
        }

        // /// <summary>
        // /// 添加剧情数据到字典
        // /// </summary>
        // private void AddPlotData(PlotData data)
        // {
        //     if (!plotDataDic.ContainsKey(data.id))
        //     {
        //         plotDataDic.Add(data.id, data);
        //     }
        // }

        /// <summary>
        /// 加载NPC对话数据
        /// </summary>
        private void LoadNpcDialogData()
        {
            // 模拟加载NPC对话数据
            AddNpcDialogData(new NpcDialogData {
                id = "N_111000", 
                name = "甜菜女士", 
                npcId = 0, 
                textTime = "0", 
                emoTime = ""
            });

            AddNpcDialogData(new NpcDialogData {
                id = "N_111001", 
                name = "花椰菜变成羊", 
                npcId = 1, 
                textTime = "1", 
                emoTime = ""
            });

            AddNpcDialogData(new NpcDialogData {
                id = "N_111002", 
                name = "程序猿", 
                npcId = 2, 
                textTime = "2", 
                emoTime = ""
            });

            AddNpcDialogData(new NpcDialogData {
                id = "N_111003", 
                name = "一只隐藏的羊", 
                npcId = 3, 
                textTime = "3", 
                emoTime = ""
            });
        }

        // /// <summary>
        // /// 添加NPC对话数据到字典
        // /// </summary>
        // private void AddNpcDialogData(NpcDialogData data)
        // {
        //     if (!npcDialogDataDic.ContainsKey(data.id))
        //     {
        //         npcDialogDataDic.Add(data.id, data);
        //     }
        // }

        /// <summary>
        /// 加载玩家任务进度
        /// </summary>
        private void LoadUserTaskProgress()
        {
            // 这里应该从PlayerPrefs或保存文件加载进度
            // 初始状态下，添加第一个任务
            if (currentTasks.Count == 0)
            {
                AcceptTask(1);
            }
        }

        /// <summary>
        /// 接受任务
        /// </summary>
        public bool AcceptTask(int taskId)
        {
            if (taskDataDic.TryGetValue(taskId, out TaskData taskData))
            {
                // 检查是否已经接受或完成过该任务
                if (IsTaskAccepted(taskId) || IsTaskCompleted(taskId))
                {
                    return false;
                }

                // 创建新的用户任务
                UserTask newTask = new UserTask(taskData);
                currentTasks.Add(newTask);

                // 触发接受任务剧情
                if (!string.IsNullOrEmpty(taskData.acceptPlotId))
                {
                    TriggerPlot(taskData.acceptPlotId);
                }

                Debug.Log($"接受任务: {taskData.name}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查任务是否已接受
        /// </summary>
        private bool IsTaskAccepted(int taskId)
        {
            return currentTasks.Exists(t => t.TaskId == taskId);
        }

        /// <summary>
        /// 检查任务是否已完成
        /// </summary>
        public bool IsTaskCompleted(int taskId)
        {
            return completedTaskIds.Contains(taskId);
        }

        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdateTaskProgress(int itemId, int quantity)
        {
            foreach (var task in currentTasks)
            {
                if (!task.IsCompleted && !string.IsNullOrEmpty(task.GoalCount))
                {
                    string[] goalParts = task.GoalCount.Split('#');
                    if (goalParts.Length == 2 && int.TryParse(goalParts[0], out int requiredItemId) && requiredItemId == itemId)
                    {
                        task.UpdateProgress(quantity);
                        
                        // 检查任务是否完成
                        if (task.IsCompleted)
                        {
                            CompleteTask(task.TaskId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        public bool CompleteTask(int taskId)
        {
            UserTask task = currentTasks.Find(t => t.TaskId == taskId);
            if (task != null && task.IsCompleted)
            {
                // 移除当前任务
                currentTasks.Remove(task);
                // 添加到已完成列表
                completedTaskIds.Add(taskId);

                // 触发完成任务剧情
                if (!string.IsNullOrEmpty(task.CompletePlotId))
                {
                    TriggerPlot(task.CompletePlotId);
                }

                // 发放奖励
                GrantReward(task.Reward);

                // 自动接受下一个任务
                if (task.NextGoalId > 0)
                {
                    AcceptTask(task.NextGoalId);
                }

                Debug.Log($"完成任务: {task.TaskName}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发放任务奖励
        /// </summary>
        private void GrantReward(string rewardData)
        {
            if (string.IsNullOrEmpty(rewardData))
                return;

            // 解析奖励数据（格式：道具ID#数量）
            string[] rewardParts = rewardData.Split('#');
            if (rewardParts.Length == 2 && int.TryParse(rewardParts[0], out int itemId) && int.TryParse(rewardParts[1], out int quantity))
            {
                // 这里应该调用背包系统添加物品
                Debug.Log($"获得奖励: 道具ID={itemId}, 数量={quantity}");
            }
        }

        /// <summary>
        /// 触发剧情
        /// </summary>
        private void TriggerPlot(string plotId)
        {
            if (plotDataDic.TryGetValue(plotId, out PlotData plot))
            {
                // 这里应该调用剧情系统播放剧情
                Debug.Log($"触发剧情: {plot.name}, 内容: {plot.textTime}");
            }
        }

        /// <summary>
        /// 获取当前任务列表
        /// </summary>
        public List<UserTask> GetCurrentTasks()
        {
            return new List<UserTask>(currentTasks);
        }

        /// <summary>
        /// 获取已完成任务列表
        /// </summary>
        public List<int> GetCompletedTaskIds()
        {
            return new List<int>(completedTaskIds);
        }

        /// <summary>
        /// 获取任务数据
        /// </summary>
        public TaskData GetTaskData(int taskId)
        {
            if (taskDataDic.TryGetValue(taskId, out TaskData data))
            {
                return data;
            }
            return null;
        }

        /// <summary>
        /// 保存任务进度
        /// </summary>
        public void SaveTaskProgress()
        {
            // 这里应该将任务进度保存到PlayerPrefs或文件中
            // 保存当前任务ID列表
            PlayerPrefs.SetInt("TaskCount", currentTasks.Count);
            for (int i = 0; i < currentTasks.Count; i++)
            {
                PlayerPrefs.SetInt($"Task_{i}_Id", currentTasks[i].TaskId);
                PlayerPrefs.SetInt($"Task_{i}_Progress", currentTasks[i].CurrentProgress);
            }

            // 保存已完成任务ID列表
            PlayerPrefs.SetInt("CompletedTaskCount", completedTaskIds.Count);
            for (int i = 0; i < completedTaskIds.Count; i++)
            {
                PlayerPrefs.SetInt($"CompletedTask_{i}", completedTaskIds[i]);
            }

            PlayerPrefs.Save();
        }
    }
}