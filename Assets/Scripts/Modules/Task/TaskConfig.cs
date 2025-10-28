using System;

namespace TTGJ.Task
{
    public enum TaskState { 
        NotAccept = 0,
        NotComplete = 1,
        NotReward = 2,
    }
    public class TaskConfig
    {
        public int TaskId { get; set; }
        public TaskState TaskState { get; set; }
    }
}