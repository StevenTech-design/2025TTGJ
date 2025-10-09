using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using TTGJ.Build;

namespace TTGJ.Editor
{
    /// <summary>
    /// 自动创建 BuildTestUI 预制体的工具
    /// </summary>
    public class CreateBuildTestUIPrefab
    {
        [MenuItem("TTGJ/Build Test/Create BuildTestUI Prefab")]
        static void CreatePrefab()
        {
            // 确保场景中有 EventSystem
            UnityEngine.EventSystems.EventSystem eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("✓ EventSystem 已创建");
            }
            
            // 创建 Canvas
            GameObject canvasObj = new GameObject("BuildTestCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();

            // 创建按钮面板
            GameObject panelObj = new GameObject("ButtonPanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.5f);
            panelRect.anchorMax = new Vector2(0, 0.5f);
            panelRect.pivot = new Vector2(0, 0.5f);
            panelRect.sizeDelta = new Vector2(220, 400);
            panelRect.anchoredPosition = new Vector2(20, 0);

            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            VerticalLayoutGroup layout = panelObj.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;

            // 创建标题
            GameObject titleObj = CreateText("Title", "建筑测试", panelObj.transform);
            Text titleText = titleObj.GetComponent<Text>();
            if (titleText != null)
            {
                titleText.fontSize = 20;
                titleText.fontStyle = FontStyle.Bold;
                titleText.alignment = TextAnchor.MiddleCenter;
            }
            LayoutElement titleLayout = titleObj.GetComponent<LayoutElement>();
            if (titleLayout != null) titleLayout.minHeight = 40;

            // 创建按钮
            Button placeFloorBtn = CreateButton("PlaceFloorButton", "放置地板 [1]", panelObj.transform, new Color(0.2f, 0.5f, 0.8f));
            Button placeFurn1Btn = CreateButton("PlaceFurniture1Button", "家具 1x1 [2]", panelObj.transform, new Color(0.2f, 0.6f, 0.3f));
            Button placeFurn2Btn = CreateButton("PlaceFurniture2Button", "家具 2x2 [3]", panelObj.transform, new Color(0.8f, 0.4f, 0.2f));
            Button removeBtn = CreateButton("RemoveButton", "移除 [R]", panelObj.transform, new Color(0.8f, 0.2f, 0.2f));
            Button clearBtn = CreateButton("ClearAllButton", "清空全部", panelObj.transform, new Color(0.5f, 0.1f, 0.5f));

            // 创建调试信息面板
            GameObject debugPanelObj = new GameObject("DebugPanel");
            debugPanelObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform debugPanelRect = debugPanelObj.AddComponent<RectTransform>();
            debugPanelRect.anchorMin = new Vector2(0, 1);
            debugPanelRect.anchorMax = new Vector2(1, 1);
            debugPanelRect.pivot = new Vector2(0.5f, 1);
            debugPanelRect.sizeDelta = new Vector2(-40, 120);
            debugPanelRect.anchoredPosition = new Vector2(0, -20);

            Image debugPanelImage = debugPanelObj.AddComponent<Image>();
            debugPanelImage.color = new Color(0, 0, 0, 0.7f);

            // 创建调试文本
            GameObject debugTextObj = new GameObject("DebugText");
            debugTextObj.transform.SetParent(debugPanelObj.transform, false);
            
            RectTransform debugTextRect = debugTextObj.AddComponent<RectTransform>();
            debugTextRect.anchorMin = Vector2.zero;
            debugTextRect.anchorMax = Vector2.one;
            debugTextRect.sizeDelta = Vector2.zero;
            debugTextRect.anchoredPosition = Vector2.zero;

            Text debugText = debugTextObj.AddComponent<Text>();
            debugText.text = "按 H 显示帮助\n准备就绪";
            //debugText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            debugText.fontSize = 16;
            debugText.color = Color.white;
            debugText.alignment = TextAnchor.UpperLeft;
            debugText.horizontalOverflow = HorizontalWrapMode.Wrap;
            debugText.verticalOverflow = VerticalWrapMode.Overflow;

            // 添加内边距
            RectOffset padding = new RectOffset(15, 15, 15, 15);
            debugTextRect.offsetMin = new Vector2(padding.left, padding.bottom);
            debugTextRect.offsetMax = new Vector2(-padding.right, -padding.top);

            // 创建帮助文本（初始隐藏）
            GameObject helpPanelObj = new GameObject("HelpPanel");
            helpPanelObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform helpPanelRect = helpPanelObj.AddComponent<RectTransform>();
            helpPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            helpPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            helpPanelRect.pivot = new Vector2(0.5f, 0.5f);
            helpPanelRect.sizeDelta = new Vector2(400, 300);
            helpPanelRect.anchoredPosition = Vector2.zero;

            Image helpPanelImage = helpPanelObj.AddComponent<Image>();
            helpPanelImage.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);

            GameObject helpTextObj = new GameObject("HelpText");
            helpTextObj.transform.SetParent(helpPanelObj.transform, false);
            
            RectTransform helpTextRect = helpTextObj.AddComponent<RectTransform>();
            helpTextRect.anchorMin = Vector2.zero;
            helpTextRect.anchorMax = Vector2.one;
            helpTextRect.offsetMin = new Vector2(20, 20);
            helpTextRect.offsetMax = new Vector2(-20, -20);

            Text helpText = helpTextObj.AddComponent<Text>();
            helpText.text = "=== 快捷键帮助 ===\n\n" +
                           "1 - 放置地板\n" +
                           "2 - 放置家具 1x1\n" +
                           "3 - 放置家具 2x2\n" +
                           "R - 移除模式\n" +
                           "ESC - 退出当前模式\n" +
                           "H - 显示/隐藏帮助\n\n" +
                           "左键 - 放置/移除对象\n" +
                           "鼠标移动 - 预览位置\n\n" +
                           "绿色 = 可放置\n" +
                           "红色 = 不可放置";
            //helpText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            helpText.fontSize = 18;
            helpText.color = Color.white;
            helpText.alignment = TextAnchor.MiddleCenter;

            helpPanelObj.SetActive(false); // 初始隐藏

            // 添加 BuildTestUI 组件
            TTGJ.Tests.BuildTestUI buildTestUI = canvasObj.AddComponent<TTGJ.Tests.BuildTestUI>();
            
            // 配置组件引用
            SerializedObject so = new SerializedObject(buildTestUI);
            so.FindProperty("placeFloorButton").objectReferenceValue = placeFloorBtn;
            so.FindProperty("placeFurniture1Button").objectReferenceValue = placeFurn1Btn;
            so.FindProperty("placeFurniture2Button").objectReferenceValue = placeFurn2Btn;
            so.FindProperty("removeButton").objectReferenceValue = removeBtn;
            so.FindProperty("clearAllButton").objectReferenceValue = clearBtn;
            so.FindProperty("debugText").objectReferenceValue = debugText;
            
            // 尝试找到 PlacementSystem
            PlacementSystem placementSystem = Object.FindObjectOfType<PlacementSystem>();
            if (placementSystem != null)
            {
                so.FindProperty("placementSystem").objectReferenceValue = placementSystem;
            }
            
            so.ApplyModifiedProperties();

            // 确保目录存在
            if (!AssetDatabase.IsValidFolder("Assets/Res"))
                AssetDatabase.CreateFolder("Assets", "Res");
            if (!AssetDatabase.IsValidFolder("Assets/Res/UI"))
                AssetDatabase.CreateFolder("Assets/Res", "UI");

            // 保存为预制体
            string prefabPath = "Assets/Res/UI/BuildTestUI.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
            
            Debug.Log("✓ BuildTestUI 预制体已创建: " + prefabPath);

            // 选中预制体
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);

            // 如果在场景中，可以选择保留或删除
            bool keepInScene = EditorUtility.DisplayDialog(
                "创建成功！",
                "BuildTestUI 预制体已创建！\n\n" +
                "路径: " + prefabPath + "\n\n" +
                "是否在当前场景中保留这个实例？\n" +
                "（预制体已保存，可以随时拖入场景）",
                "保留在场景中",
                "删除（只保留预制体）"
            );

            if (!keepInScene)
            {
                Object.DestroyImmediate(canvasObj);
                Debug.Log("场景实例已删除，预制体已保存");
            }
            else
            {
                Debug.Log("BuildTestUI 已添加到当前场景");
            }
        }

        static Button CreateButton(string name, string text, Transform parent, Color color)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 45);

            Image image = btnObj.AddComponent<Image>();
            image.color = color;

            Button button = btnObj.AddComponent<Button>();
            
            // 设置按钮颜色变化
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(color.r * 1.2f, color.g * 1.2f, color.b * 1.2f);
            colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
            button.colors = colors;
            
            // 创建文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            Text btnText = textObj.AddComponent<Text>();
            btnText.text = text;
            //btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            btnText.fontSize = 16;
            btnText.color = Color.white;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.fontStyle = FontStyle.Bold;

            // 添加 LayoutElement
            LayoutElement layoutElement = btnObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 45;

            return button;
        }

        static GameObject CreateText(string name, string text, Transform parent)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 30);

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            //textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 14;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;

            LayoutElement layoutElement = textObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 30;

            return textObj;
        }

        [MenuItem("TTGJ/Build Test/Add BuildTestUI to Scene")]
        static void AddToScene()
        {
            string prefabPath = "Assets/Res/UI/BuildTestUI.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                bool create = EditorUtility.DisplayDialog(
                    "预制体不存在",
                    "BuildTestUI 预制体不存在。\n是否现在创建？",
                    "创建",
                    "取消"
                );

                if (create)
                {
                    CreatePrefab();
                }
                return;
            }

            // 检查场景中是否已存在
            TTGJ.Tests.BuildTestUI existing = Object.FindObjectOfType<TTGJ.Tests.BuildTestUI>();
            if (existing != null)
            {
                bool replace = EditorUtility.DisplayDialog(
                    "已存在实例",
                    "场景中已经有 BuildTestUI。\n是否替换？",
                    "替换",
                    "取消"
                );

                if (replace)
                {
                    Object.DestroyImmediate(existing.gameObject);
                }
                else
                {
                    return;
                }
            }

            // 实例化到场景
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            
            // 尝试链接 PlacementSystem
            PlacementSystem placementSystem = Object.FindObjectOfType<PlacementSystem>();
            if (placementSystem != null)
            {
                TTGJ.Tests.BuildTestUI ui = instance.GetComponent<TTGJ.Tests.BuildTestUI>();
                if (ui != null)
                {
                    SerializedObject so = new SerializedObject(ui);
                    so.FindProperty("placementSystem").objectReferenceValue = placementSystem;
                    so.ApplyModifiedProperties();
                }
            }

            Selection.activeGameObject = instance;
            Debug.Log("✓ BuildTestUI 已添加到场景");

            EditorUtility.DisplayDialog("完成", "BuildTestUI 已添加到场景！", "确定");
        }
    }
}

