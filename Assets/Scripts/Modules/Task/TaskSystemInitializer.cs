using UnityEngine;
using TTGJ.Framework;

namespace TTGJ.Modules.TaskSystem
{
    /// <summary>
    /// 任务系统初始化器，用于在游戏启动时初始化任务系统
    /// </summary>
    public class TaskSystemInitializer : MonoBehaviour
    {
        private void Awake()
        {
            // 确保不会重复初始化
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            // 初始化任务管理器
            TaskManager.Instance.Initialize();
            Debug.Log("任务系统初始化完成");
        }
        
        private void OnApplicationQuit()
        {
            // 保存任务进度
            TaskManager.Instance.SaveTaskProgress();
        }
    }
}