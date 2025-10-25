using System.Collections;
using System.Collections.Generic;
using TTGJ.Framework;
using UnityEngine;

namespace TTGJ.Task
{
    public class TaskManager : Singleton<TaskManager>
    {
        private List<int> _currentTaskIds = new List<int>();

        public void AddTask(int taskId)
        {
            if (_currentTaskIds.Contains(taskId))
            {
                return;
            }
            _currentTaskIds.Add(taskId);
        }

        public void FinishTask(int taskId)
        {
            if (!_currentTaskIds.Contains(taskId))
            {
                return;
            }
            _currentTaskIds.Remove(taskId);
        }

    }
}
