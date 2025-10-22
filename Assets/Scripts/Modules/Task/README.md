### 任务系统使用说明

## 系统概述

这个任务系统实现了一个完整的游戏任务框架，支持任务的接受、进度跟踪、完成和奖励发放等功能。系统基于提供的数据结构设计，包括任务数据、剧情数据和NPC对话数据。

## 主要组件

1. **TaskData.cs** - 定义了任务、剧情和NPC对话的数据结构
2. **TaskManager.cs** - 任务系统的核心管理器，负责任务的加载、接受、进度更新和完成逻辑
3. **UserTask.cs** - 管理玩家当前进行中的任务状态
4. **TaskPanel.cs** - 任务面板UI类，用于显示任务列表
5. **TaskItemUI.cs** - 单个任务项的UI类
6. **TaskSystemInitializer.cs** - 任务系统初始化器
7. **TaskEventHandler.cs** - 任务事件处理器，用于处理游戏中触发任务进度更新的事件

## 集成步骤

1. **添加任务系统初始化器**
   - 在游戏场景中创建一个空物体
   - 挂载`TaskSystemInitializer`脚本
   - 确保这个物体在游戏启动时会被加载

2. **创建任务UI**
   - 创建任务面板UI预制体，路径设置为`Assets/Res/UI/TaskPanel.prefab`
   - 确保包含以下组件引用：
     - taskListContent: 任务列表的容器
     - taskItemPrefab: 任务项预制体
     - closeButton: 关闭按钮

3. **创建任务项UI预制体**
   - 确保包含以下组件：
     - taskNameText: 任务名称文本
     - taskDescriptionText: 任务描述文本
     - progressSlider: 进度条
     - progressText: 进度文本
     - taskIcon: 任务图标
     - completedMark: 完成标记

## 使用方法

### 1. 显示任务面板
```csharp
using TTGJ.UI;
using TTGJ.Modules.TaskSystem;

// 显示任务面板
TaskPanel taskPanel = await UIManager.Instance.ShowPanel<TaskPanel>();
```

### 2. 更新任务进度
```csharp
using TTGJ.Modules.TaskSystem;

// 当玩家获得物品时更新任务进度
TaskEventHandler.OnPlayerGainItem(itemId, quantity);

// 当玩家与NPC对话时检查任务
TaskEventHandler.OnPlayerTalkToNPC(npcId);

// 通用的目标完成方法
TaskEventHandler.OnPlayerCompleteObjective("Item", itemId, quantity);
```

### 3. 接受任务
```csharp
// 接受特定ID的任务
TaskManager.Instance.AcceptTask(taskId);
```

### 4. 完成任务
```csharp
// 完成特定ID的任务
TaskManager.Instance.CompleteTask(taskId);
```

### 5. 获取任务信息
```csharp
// 获取当前任务列表
var currentTasks = TaskManager.Instance.GetCurrentTasks();

// 获取已完成任务列表
var completedTaskIds = TaskManager.Instance.GetCompletedTaskIds();

// 获取特定任务的数据
var taskData = TaskManager.Instance.GetTaskData(taskId);
```

## 数据结构说明

### 任务数据格式
- **id**: 任务唯一ID
- **name**: 任务名称
- **npcId**: 任务NPC ID
- **goalCount**: 任务要求（格式：道具ID#数量）
- **reward**: 奖励（格式：道具ID#数量）
- **acceptPlotId**: 接受时剧情ID
- **progressPlotId**: 进行中剧情ID
- **completePlotId**: 完成时剧情ID
- **nextGoalId**: 关联的下一个任务ID

### 剧情数据格式
- **id**: 剧情唯一ID
- **name**: 剧情备注
- **npcId**: 对话NPC ID
- **textTime**: 剧情文字#时间
- **emoTime**: 剧情表情#时间
- **nextId**: 下个剧情ID

### NPC对话数据格式
- **id**: NPC唯一ID
- **name**: 碎碎念备注
- **npcId**: NPC类型ID (0甜菜 1羊 2猴子 3绿羊)
- **textTime**: 对话#时间
- **emoTime**: 表情#时间

## 扩展建议

1. **添加任务条件系统** - 可以扩展任务系统，添加前置条件和触发条件
2. **实现任务分类** - 主线任务、支线任务、日常任务等
3. **添加任务奖励动画** - 当玩家完成任务时显示奖励获得的动画效果
4. **实现自动寻路** - 点击任务项可以自动导航到任务目标
5. **任务进度保存优化** - 可以使用更高效的数据序列化方式保存任务进度

## 注意事项

1. 目前任务数据是硬编码在TaskManager中的，实际使用时应该从配置文件或数据库加载
2. 任务奖励发放目前只是打印日志，需要根据游戏的背包系统进行实际实现
3. 剧情触发功能需要根据游戏的剧情系统进行扩展
4. UI相关功能需要配合Unity的UI系统进行实现