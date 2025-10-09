using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TTGJ.Build;

namespace TTGJ.Editor
{
    /// <summary>
    /// 自动设置建筑测试场景的编辑器工具
    /// </summary>
    public class BuildTestSceneSetup : EditorWindow
    {
        private ObjectsDatabaseSO database;
        private GameObject gridPrefab;
        private GameObject cellPrefab;
        private Material previewMaterial;

        [MenuItem("TTGJ/Build Test/Setup Scene Wizard")]
        static void ShowWindow()
        {
            var window = GetWindow<BuildTestSceneSetup>("建筑测试场景设置向导");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("建筑测试场景设置", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "这个工具会帮你在当前场景中快速设置建筑测试系统。\n" +
                "确保你已经创建了 ObjectsDatabase 和必要的预制体。",
                MessageType.Info);

            EditorGUILayout.Space();

            // 配置字段
            database = (ObjectsDatabaseSO)EditorGUILayout.ObjectField(
                "Objects Database", database, typeof(ObjectsDatabaseSO), false);

            gridPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Grid Prefab", gridPrefab, typeof(GameObject), false);

            cellPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Cell Prefab", cellPrefab, typeof(GameObject), false);

            previewMaterial = (Material)EditorGUILayout.ObjectField(
                "Preview Material", previewMaterial, typeof(Material), false);

            EditorGUILayout.Space();

            // 设置按钮
            GUI.enabled = database != null;
            if (GUILayout.Button("1. 创建地面平面", GUILayout.Height(30)))
            {
                CreateGroundPlane();
            }

            if (GUILayout.Button("2. 创建建筑系统", GUILayout.Height(30)))
            {
                CreateBuildSystem();
            }

            if (GUILayout.Button("3. 创建测试 UI", GUILayout.Height(30)))
            {
                CreateTestUI();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("一键设置完整场景", GUILayout.Height(40)))
            {
                SetupCompleteScene();
            }

            GUI.enabled = true;
        }

        void CreateGroundPlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "GroundPlane";
            plane.transform.position = Vector3.zero;
            plane.transform.localScale = new Vector3(5, 1, 5);

            // 设置材质
            Renderer renderer = plane.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.8f, 0.8f, 0.7f);
            renderer.material = mat;

            Selection.activeGameObject = plane;
            Debug.Log("✓ 地面平面已创建。请手动设置 Layer 为 Placement");
        }

        void CreateBuildSystem()
        {
            // 确保有 EventSystem
            UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("✓ EventSystem 已创建");
            }
            
            // 创建主对象
            GameObject buildSystem = new GameObject("BuildSystem");
            buildSystem.transform.position = Vector3.zero;

            // 添加组件
            var placementSystem = buildSystem.AddComponent<PlacementSystem>();
            var inputManager = buildSystem.AddComponent<InputManager>();
            var previewSystem = buildSystem.AddComponent<PreviewSystem>();
            var objectPlacer = buildSystem.AddComponent<ObjectPlacer>();
            var soundFeedback = buildSystem.AddComponent<SoundFeedback>();

            // 查找或创建相机
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                camObj.transform.position = new Vector3(0, 10, -10);
                camObj.transform.rotation = Quaternion.Euler(45, 0, 0);
            }

            // 查找或创建 Grid
            Grid grid = FindObjectOfType<Grid>();
            if (grid == null)
            {
                GameObject gridObj = new GameObject("Grid");
                grid = gridObj.AddComponent<Grid>();
            }

            // 创建网格可视化
            GameObject gridVisualization = null;
            if (gridPrefab != null)
            {
                gridVisualization = Instantiate(gridPrefab, buildSystem.transform);
                gridVisualization.name = "GridVisualization";
            }
            else
            {
                gridVisualization = new GameObject("GridVisualization");
                gridVisualization.transform.SetParent(buildSystem.transform);
            }

            // 创建 Cell Indicator
            GameObject cellIndicator = null;
            if (cellPrefab != null)
            {
                cellIndicator = Instantiate(cellPrefab, previewSystem.transform);
                cellIndicator.name = "CellIndicator";
            }

            // 配置组件（使用序列化）
            SerializedObject inputManagerSO = new SerializedObject(inputManager);
            inputManagerSO.FindProperty("sceneCamera").objectReferenceValue = cam;
            inputManagerSO.FindProperty("placementLayermask").intValue = LayerMask.GetMask("Default");
            inputManagerSO.ApplyModifiedProperties();

            SerializedObject placementSystemSO = new SerializedObject(placementSystem);
            placementSystemSO.FindProperty("inputManager").objectReferenceValue = inputManager;
            placementSystemSO.FindProperty("grid").objectReferenceValue = grid;
            placementSystemSO.FindProperty("database").objectReferenceValue = database;
            placementSystemSO.FindProperty("gridVisualization").objectReferenceValue = gridVisualization;
            placementSystemSO.FindProperty("preview").objectReferenceValue = previewSystem;
            placementSystemSO.FindProperty("objectPlacer").objectReferenceValue = objectPlacer;
            placementSystemSO.FindProperty("soundFeedback").objectReferenceValue = soundFeedback;
            placementSystemSO.ApplyModifiedProperties();

            if (cellIndicator != null && previewMaterial != null)
            {
                SerializedObject previewSystemSO = new SerializedObject(previewSystem);
                previewSystemSO.FindProperty("cellIndicator").objectReferenceValue = cellIndicator;
                previewSystemSO.FindProperty("previewMaterialPrefab").objectReferenceValue = previewMaterial;
                previewSystemSO.ApplyModifiedProperties();
            }

            Selection.activeGameObject = buildSystem;
            Debug.Log("✓ 建筑系统已创建");
        }

        void CreateTestUI()
        {
            // 查找或创建 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建 UI 面板
            GameObject panel = new GameObject("TestPanel");
            panel.transform.SetParent(canvas.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 0.5f);
            panelRect.sizeDelta = new Vector2(200, 0);
            panelRect.anchoredPosition = new Vector2(10, 0);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);

            // 创建垂直布局
            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 10;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;

            // 创建按钮
            Button placeFloorBtn = CreateButton("Place Floor", panel.transform);
            Button placeFurn1Btn = CreateButton("Place Furniture 1x1", panel.transform);
            Button placeFurn2Btn = CreateButton("Place Furniture 2x2", panel.transform);
            Button removeBtn = CreateButton("Remove", panel.transform);
            Button clearBtn = CreateButton("Clear All", panel.transform);

            // 创建调试文本
            GameObject debugTextObj = new GameObject("DebugText");
            debugTextObj.transform.SetParent(canvas.transform, false);
            RectTransform debugRect = debugTextObj.AddComponent<RectTransform>();
            debugRect.anchorMin = new Vector2(0, 1);
            debugRect.anchorMax = new Vector2(1, 1);
            debugRect.pivot = new Vector2(0.5f, 1);
            debugRect.sizeDelta = new Vector2(-20, 100);
            debugRect.anchoredPosition = new Vector2(0, -10);

            Text debugText = debugTextObj.AddComponent<Text>();
            debugText.text = "按 H 显示帮助";
            debugText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            debugText.fontSize = 14;
            debugText.color = Color.white;
            debugText.alignment = TextAnchor.UpperLeft;

            // 添加 BuildTestUI 组件
            PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();
            if (placementSystem != null)
            {
                GameObject buildSystem = placementSystem.gameObject;
                var testUI = buildSystem.AddComponent<TTGJ.Tests.BuildTestUI>();

                SerializedObject testUISO = new SerializedObject(testUI);
                testUISO.FindProperty("placementSystem").objectReferenceValue = placementSystem;
                testUISO.FindProperty("placeFloorButton").objectReferenceValue = placeFloorBtn;
                testUISO.FindProperty("placeFurniture1Button").objectReferenceValue = placeFurn1Btn;
                testUISO.FindProperty("placeFurniture2Button").objectReferenceValue = placeFurn2Btn;
                testUISO.FindProperty("removeButton").objectReferenceValue = removeBtn;
                testUISO.FindProperty("clearAllButton").objectReferenceValue = clearBtn;
                testUISO.FindProperty("debugText").objectReferenceValue = debugText;
                testUISO.ApplyModifiedProperties();
            }

            Debug.Log("✓ 测试 UI 已创建");
        }

        Button CreateButton(string text, Transform parent)
        {
            GameObject btnObj = new GameObject(text);
            btnObj.transform.SetParent(parent, false);

            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 40);

            Image image = btnObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.8f, 1f);

            Button button = btnObj.AddComponent<Button>();
            
            // 创建文本子对象
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            Text btnText = textObj.AddComponent<Text>();
            btnText.text = text;
            btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            btnText.fontSize = 14;
            btnText.color = Color.white;
            btnText.alignment = TextAnchor.MiddleCenter;

            return button;
        }

        void SetupCompleteScene()
        {
            if (database == null)
            {
                EditorUtility.DisplayDialog("错误", "请先设置 Objects Database!", "确定");
                return;
            }

            CreateGroundPlane();
            CreateBuildSystem();
            CreateTestUI();

            EditorUtility.DisplayDialog(
                "完成",
                "场景设置完成！\n\n" +
                "下一步：\n" +
                "1. 将地面的 Layer 设置为 Placement\n" +
                "2. 在 Project Settings → Tags and Layers 中创建 Placement 层（如果没有）\n" +
                "3. 运行场景进行测试",
                "确定");
        }
    }
}



