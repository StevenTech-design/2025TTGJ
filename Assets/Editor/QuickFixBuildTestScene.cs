using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using TTGJ.Build;

namespace TTGJ.Editor
{
    /// <summary>
    /// 快速修复 BuildTest 场景的 Grid 设置
    /// </summary>
    public class QuickFixBuildTestScene
    {
        [MenuItem("TTGJ/Build Test/Quick Fix - Add Grid to Scene")]
        static void AddGridToScene()
        {
            // 查找是否已经有 Grid
            Grid existingGrid = Object.FindObjectOfType<Grid>();
            
            if (existingGrid == null)
            {
                // 创建新的 Grid 对象
                GameObject gridObj = new GameObject("Grid");
                existingGrid = gridObj.AddComponent<Grid>();
                gridObj.transform.position = Vector3.zero;
                
                Debug.Log("✓ 创建了新的 Grid 对象");
            }
            else
            {
                Debug.Log("✓ 场景中已存在 Grid 对象: " + existingGrid.gameObject.name);
            }
            
            // 确保场景中有 EventSystem（UI 系统需要）
            EnsureEventSystem();

            // 查找 BuildSystem 并链接 Grid
            PlacementSystem placementSystem = Object.FindObjectOfType<PlacementSystem>();
            if (placementSystem != null)
            {
                SerializedObject so = new SerializedObject(placementSystem);
                so.FindProperty("grid").objectReferenceValue = existingGrid;
                so.ApplyModifiedProperties();
                
                Debug.Log("✓ Grid 已链接到 PlacementSystem");
                
                // 同时检查其他必需的引用
                FixMissingReferences(placementSystem);
            }
            else
            {
                Debug.LogWarning("场景中没有找到 PlacementSystem");
            }

            EditorUtility.DisplayDialog("完成", 
                "Grid 设置完成！\n\n" +
                "Grid 对象: " + existingGrid.gameObject.name + "\n" +
                "已链接到 PlacementSystem", 
                "确定");
        }

        [MenuItem("TTGJ/Build Test/Quick Fix - Link All References")]
        static void LinkAllReferences()
        {
            PlacementSystem placementSystem = Object.FindObjectOfType<PlacementSystem>();
            if (placementSystem == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中没有找到 PlacementSystem", "确定");
                return;
            }

            // 确保预览材质存在
            EnsurePreviewMaterialExists();
            
            // 确保 EventSystem 存在
            EnsureEventSystem();

            FixMissingReferences(placementSystem);

            EditorUtility.DisplayDialog("完成", 
                "所有引用已检查并修复！\n请查看 Console 了解详情", 
                "确定");
        }

        static void FixMissingReferences(PlacementSystem placementSystem)
        {
            SerializedObject so = new SerializedObject(placementSystem);
            bool hasChanges = false;

            // 修复 Grid
            var gridProp = so.FindProperty("grid");
            if (gridProp.objectReferenceValue == null)
            {
                Grid grid = Object.FindObjectOfType<Grid>();
                if (grid == null)
                {
                    GameObject gridObj = new GameObject("Grid");
                    grid = gridObj.AddComponent<Grid>();
                    Debug.Log("✓ 创建了新的 Grid");
                }
                gridProp.objectReferenceValue = grid;
                hasChanges = true;
                Debug.Log("✓ 链接了 Grid");
            }

            // 修复 InputManager
            var inputManagerProp = so.FindProperty("inputManager");
            if (inputManagerProp.objectReferenceValue == null)
            {
                InputManager inputManager = placementSystem.GetComponent<InputManager>();
                if (inputManager != null)
                {
                    inputManagerProp.objectReferenceValue = inputManager;
                    hasChanges = true;
                    Debug.Log("✓ 链接了 InputManager");
                }
            }

            // 修复 Preview
            var previewProp = so.FindProperty("preview");
            if (previewProp.objectReferenceValue == null)
            {
                PreviewSystem preview = placementSystem.GetComponent<PreviewSystem>();
                if (preview != null)
                {
                    previewProp.objectReferenceValue = preview;
                    hasChanges = true;
                    Debug.Log("✓ 链接了 PreviewSystem");
                }
            }

            // 修复 ObjectPlacer
            var objectPlacerProp = so.FindProperty("objectPlacer");
            if (objectPlacerProp.objectReferenceValue == null)
            {
                ObjectPlacer objectPlacer = placementSystem.GetComponent<ObjectPlacer>();
                if (objectPlacer != null)
                {
                    objectPlacerProp.objectReferenceValue = objectPlacer;
                    hasChanges = true;
                    Debug.Log("✓ 链接了 ObjectPlacer");
                }
            }

            // 修复 SoundFeedback
            var soundFeedbackProp = so.FindProperty("soundFeedback");
            if (soundFeedbackProp.objectReferenceValue == null)
            {
                SoundFeedback soundFeedback = placementSystem.GetComponent<SoundFeedback>();
                if (soundFeedback != null)
                {
                    soundFeedbackProp.objectReferenceValue = soundFeedback;
                    hasChanges = true;
                    Debug.Log("✓ 链接了 SoundFeedback");
                }
            }

            // 修复 GridVisualization
            var gridVisProp = so.FindProperty("gridVisualization");
            if (gridVisProp.objectReferenceValue == null)
            {
                // 尝试查找场景中名为 Grid 的可视化对象
                GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
                foreach (var obj in allObjects)
                {
                    if (obj.name == "Grid" && obj != Object.FindObjectOfType<Grid>()?.gameObject)
                    {
                        gridVisProp.objectReferenceValue = obj;
                        hasChanges = true;
                        Debug.Log("✓ 链接了 GridVisualization: " + obj.name);
                        break;
                    }
                }

                // 如果还没找到，创建一个简单的
                if (gridVisProp.objectReferenceValue == null)
                {
                    GameObject gridVis = new GameObject("GridVisualization");
                    gridVis.transform.SetParent(placementSystem.transform);
                    gridVisProp.objectReferenceValue = gridVis;
                    hasChanges = true;
                    Debug.Log("✓ 创建了新的 GridVisualization");
                }
            }

            if (hasChanges)
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(placementSystem);
                Debug.Log("=== 所有引用已更新 ===");
            }
            else
            {
                Debug.Log("=== 所有引用都已正确设置 ===");
            }

            // 显示当前状态
            Debug.Log("\n当前 PlacementSystem 状态:");
            Debug.Log("- Grid: " + (so.FindProperty("grid").objectReferenceValue != null ? "✓" : "✗"));
            Debug.Log("- InputManager: " + (so.FindProperty("inputManager").objectReferenceValue != null ? "✓" : "✗"));
            Debug.Log("- Preview: " + (so.FindProperty("preview").objectReferenceValue != null ? "✓" : "✗"));
            Debug.Log("- ObjectPlacer: " + (so.FindProperty("objectPlacer").objectReferenceValue != null ? "✓" : "✗"));
            Debug.Log("- Database: " + (so.FindProperty("database").objectReferenceValue != null ? "✓" : "✗"));
            Debug.Log("- GridVisualization: " + (so.FindProperty("gridVisualization").objectReferenceValue != null ? "✓" : "✗"));
        }

        [MenuItem("TTGJ/Build Test/Check Scene Status")]
        static void CheckSceneStatus()
        {
            Debug.Log("=== 检查 BuildTest 场景状态 ===\n");

            // 检查 PlacementSystem
            PlacementSystem placementSystem = Object.FindObjectOfType<PlacementSystem>();
            if (placementSystem == null)
            {
                Debug.LogError("✗ 场景中没有 PlacementSystem");
            }
            else
            {
                Debug.Log("✓ 找到 PlacementSystem");
                
                SerializedObject so = new SerializedObject(placementSystem);
                Debug.Log("\n必需组件状态:");
                CheckReference(so, "grid", "Grid");
                CheckReference(so, "inputManager", "InputManager");
                CheckReference(so, "preview", "PreviewSystem");
                CheckReference(so, "objectPlacer", "ObjectPlacer");
                CheckReference(so, "database", "ObjectsDatabaseSO");
                CheckReference(so, "gridVisualization", "GridVisualization");
                CheckReference(so, "soundFeedback", "SoundFeedback (可选)");
            }

            // 检查 Grid
            Grid grid = Object.FindObjectOfType<Grid>();
            if (grid == null)
            {
                Debug.LogWarning("✗ 场景中没有 Grid 对象");
            }
            else
            {
                Debug.Log("✓ 找到 Grid: " + grid.gameObject.name);
            }

            // 检查地面
            GameObject ground = GameObject.Find("GroundPlane");
            if (ground == null)
            {
                Debug.LogWarning("✗ 场景中没有 GroundPlane");
            }
            else
            {
                Debug.Log("✓ 找到 GroundPlane (Layer: " + LayerMask.LayerToName(ground.layer) + ")");
                if (ground.layer != 6) // 6 通常是自定义层
                {
                    Debug.LogWarning("  警告: GroundPlane 的 Layer 可能需要设置为 Placement");
                }
            }

            // 检查相机
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("✗ 场景中没有主相机");
            }
            else
            {
                Debug.Log("✓ 找到主相机: " + cam.gameObject.name);
            }

            Debug.Log("\n=== 检查完成 ===");
        }

        static void CheckReference(SerializedObject so, string propertyName, string displayName)
        {
            var prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                if (prop.objectReferenceValue != null)
                {
                    Debug.Log($"  ✓ {displayName}: {prop.objectReferenceValue.name}");
                }
                else
                {
                    Debug.LogWarning($"  ✗ {displayName}: 未设置");
                }
            }
        }

        static void EnsurePreviewMaterialExists()
        {
            // 检查是否已经存在预览材质
            string materialPath = "Assets/Res/Grid/PreviewMaterial.mat";
            Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            
            if (existingMaterial == null)
            {
                Debug.Log("创建预览材质...");
                
                // 创建材质
                Material previewMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                
                // 设置为透明模式
                previewMaterial.SetFloat("_Surface", 1);
                previewMaterial.SetFloat("_Blend", 0);
                previewMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                previewMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                previewMaterial.SetFloat("_ZWrite", 0);
                previewMaterial.SetFloat("_AlphaClip", 0);
                previewMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                
                previewMaterial.SetColor("_BaseColor", new Color(1, 1, 1, 0.5f));
                previewMaterial.SetColor("_Color", new Color(1, 1, 1, 0.5f));
                
                // 确保目录存在
                if (!AssetDatabase.IsValidFolder("Assets/Res"))
                    AssetDatabase.CreateFolder("Assets", "Res");
                if (!AssetDatabase.IsValidFolder("Assets/Res/Grid"))
                    AssetDatabase.CreateFolder("Assets/Res", "Grid");
                
                AssetDatabase.CreateAsset(previewMaterial, materialPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log("✓ 预览材质已创建: " + materialPath);
                
                // 应用到 PreviewSystem
                PreviewSystem previewSystem = Object.FindObjectOfType<PreviewSystem>();
                if (previewSystem != null)
                {
                    SerializedObject so = new SerializedObject(previewSystem);
                    so.FindProperty("previewMaterialPrefab").objectReferenceValue = previewMaterial;
                    so.ApplyModifiedProperties();
                    Debug.Log("✓ 预览材质已应用到 PreviewSystem");
                }
            }
            else
            {
                Debug.Log("✓ 预览材质已存在");
                
                // 确保已应用到 PreviewSystem
                PreviewSystem previewSystem = Object.FindObjectOfType<PreviewSystem>();
                if (previewSystem != null)
                {
                    SerializedObject so = new SerializedObject(previewSystem);
                    var materialProp = so.FindProperty("previewMaterialPrefab");
                    if (materialProp.objectReferenceValue == null)
                    {
                        materialProp.objectReferenceValue = existingMaterial;
                        so.ApplyModifiedProperties();
                        Debug.Log("✓ 预览材质已应用到 PreviewSystem");
                    }
                }
            }
        }

        static void EnsureEventSystem()
        {
            // 检查是否已有 EventSystem
            UnityEngine.EventSystems.EventSystem eventSystem = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            
            if (eventSystem == null)
            {
                Debug.Log("创建 EventSystem...");
                
                // 创建 EventSystem
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                Debug.Log("✓ EventSystem 已创建");
            }
            else
            {
                Debug.Log("✓ EventSystem 已存在");
            }
        }
    }
}

