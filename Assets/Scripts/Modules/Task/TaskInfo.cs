using System.Collections;
using System.Collections.Generic;
using TTGJ.Luban;
using UnityEngine;

public class TaskInfo : MonoBehaviour
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private GameObject itemPrefab;
    private List<Item> items = new List<Item>();
    private int currentItemCount = 0;

    public void SetTaskInfo(int taskId) { 
        var task = LubanManager.Instance.GetTask(taskId);
        if (task == null || task.GoalCount.Count == 0) { 
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        for (int i = 0; i < task.GoalCount.Count; i++) {
            GetItem(i).SetItem(task.GoalCount[i].ItemId, task.GoalCount[i].Count);
        }
        currentItemCount = task.GoalCount.Count;
        HideMulItems();
    }
    private Item GetItem(int index) { 
        if (index >= items.Count) { 
            var itemObj = Instantiate(itemPrefab, content);
            items.Add(itemObj.GetComponent<Item>());
        }
        return items[index];
    }
    private void HideMulItems() { 
        for(int i = currentItemCount; i < items.Count; i++) {
            items[i].gameObject.SetActive(false);
        }
    }
}
