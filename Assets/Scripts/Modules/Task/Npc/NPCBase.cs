using UnityEngine;
using TTGJ.Interactable;
using TTGJ.Task;
using System.Collections.Generic;
using cfg;

namespace TTGJ.Task
{
    public class NPCBase : Liftable
    {
        [SerializeField]
        protected NPCType npcType;
        protected int currentTaskId;
        protected task currentTask;

        protected List<int>currentItemIds = new List<int>();

        protected bool CheckFinishTask() {
           // currentTask.goal
            return false;
        }

        

    }
}