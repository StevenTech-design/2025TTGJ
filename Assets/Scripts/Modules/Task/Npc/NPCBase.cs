using UnityEngine;
using TTGJ.Interactable;
using TTGJ.Task;
using System.Collections.Generic;
using cfg;
using TTGJ.Luban;
using TTGJ.Framework;
using System;

namespace TTGJ.Task
{
    public class NPCBase : Liftable
    {
        [SerializeField]
        protected NPCType npcType;
        [SerializeField]
        protected Plot plot;
        [SerializeField]
        protected TaskInfo taskInfo;
        protected task currentTask;
        private bool isInit = false;
        [SerializeField]
        private int InitTaskId = 1001;
        protected TaskConfig currentTaskConfig;

        protected Dictionary<int, int> currentItemCount = new Dictionary<int, int>();

        protected virtual void OnTriggerEnter(Collider other) {
            Init();
            if(other.gameObject.layer == LayerMask.NameToLayer("Player") ) {
                ToCompleteTask();
                return;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.TryGetComponent<Liftable>(out var liftable) || currentTask == null) { 
                return;
            }
            
            if (!CheckNeedItem((int)liftable.itemType, currentTask.GoalCount)) {
                return;
            }
            if (!currentItemCount.ContainsKey((int)liftable.itemType)){
                currentItemCount.Add((int)liftable.itemType, 0);
            }
            currentItemCount[(int)liftable.itemType]++;
            Debug.Log($"Receive id:{liftable.itemType} count:{currentItemCount[(int)liftable.itemType]}");
            taskInfo.UpdateTaskProcess(currentItemCount);
            Destroy(liftable.gameObject);
            ToCompleteTask(currentTaskConfig.TaskId);
        }

        protected bool CheckNeedItem(int itemId, List<ItemConfig> goalCount) { 
            if(goalCount == null || goalCount.Count == 0) {
                return false;
            }
            foreach(var item in goalCount) {
                if(item.ItemId == itemId) {
                    return true;
                }
            }
            return false;
        }

        protected void ToAcceptTask(int taskId)
        {
            currentTask = LubanManager.Instance.GetTask(taskId);
            Debug.Log("ToAcceptTask: " + taskId + currentTask);
            plot.onPlotComplete = () => { 
                taskInfo.SetTaskInfo(taskId);
                currentTaskConfig.TaskState = TaskState.NotComplete;
                ToCompleteTask(taskId);
            };
            plot.SetPlot(currentTask.AcceptDialog);
        }
        protected bool CheckFinishTask() {
            if (currentTask.GoalCount.Count == 0) {
                return true;
            }
            if(currentItemCount.Count != currentTask.GoalCount.Count) {
                return false;
            }
            foreach(var item in currentTask.GoalCount) {
                if(currentItemCount.ContainsKey(item.ItemId) && currentItemCount[item.ItemId] != item.Count) {
                    return false;
                }
            }
            Debug.Log("Finish task");
            return true;
        }
        protected void ToCompleteTask(int taskId)
        {
            currentTask = LubanManager.Instance.GetTask(taskId);
            Debug.Log("ToCompleteTask: " + taskId + currentTask + currentTask.ProgressHint);
            plot.onPlotComplete = () => { 
                if(CheckFinishTask()) {
                    Debug.Log("CheckFinishTask: true " );
                    currentTaskConfig.TaskState = TaskState.NotReward;
                    ToRewardTask();
                }
            };
            plot.SetPlot(currentTask.ProgressHint);
        }
        protected virtual void ToRewardTask() { 
            plot.SetPlot(currentTask.CompleteDialog);
            plot.onPlotComplete = () => { 
                ClaimRewardTask();
            };
        }

        protected virtual void ClaimRewardTask() {
            for (int i = 0; i < currentTask.Reward.Count; i++) { 
                var itemConfig = LubanManager.Instance.GetItemNew(currentTask.Reward[i].ItemId);
                string path = itemConfig.Model;
                Debug.Log("ClaimRewardTask: " + path + " " + currentTask.Reward[i].ItemId);
                var item = Instantiate(StResources.Instance.LoadByResources<GameObject>(path));
                item.transform.position = transform.position + transform.forward * (i + 1);
                item.transform.localScale = Vector3.one;
            }
            currentTaskConfig.TaskId = currentTask.NextGoalId;
            currentTaskConfig.TaskState = TaskState.NotAccept;
            var nextTask = LubanManager.Instance.GetTask(currentTaskConfig.TaskId);
            if (nextTask.Npc != (int)npcType) {
                return;
            }
            ToAcceptTask(currentTaskConfig.TaskId);
        }

        protected virtual void ToCompleteTask() {
            if (currentTaskConfig.TaskState == TaskState.NotAccept) { 
                ToAcceptTask(currentTaskConfig.TaskId);
            }
            if (currentTaskConfig.TaskState == TaskState.NotComplete) {
                CheckFinishTask();
            }
            if (currentTaskConfig.TaskState == TaskState.NotReward) {
                ToRewardTask();
            }
        }
        private void Init() { 
            if(isInit) {
                return;
            }
            currentTaskConfig = new TaskConfig() { 
                TaskId = InitTaskId,
                TaskState = TaskState.NotAccept,
            };
            isInit = true;
        }
    }
}