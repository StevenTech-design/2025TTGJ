using UnityEngine;
using UnityEditor;

namespace TTGJ.Editor
{
    /// <summary>
    /// 创建建筑预览材质的工具
    /// </summary>
    public class CreatePreviewMaterial
    {
        [MenuItem("TTGJ/Build Test/Create Preview Material")]
        static void CreateMaterial()
        {
            // 创建半透明材质
            Material previewMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            // 设置为透明模式
            previewMaterial.SetFloat("_Surface", 1); // 1 = Transparent
            previewMaterial.SetFloat("_Blend", 0); // 0 = Alpha
            previewMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMaterial.SetFloat("_ZWrite", 0);
            previewMaterial.SetFloat("_AlphaClip", 0);
            
            // 启用透明渲染队列
            previewMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            
            // 设置半透明白色
            previewMaterial.SetColor("_BaseColor", new Color(1, 1, 1, 0.5f));
            previewMaterial.SetColor("_Color", new Color(1, 1, 1, 0.5f));
            
            // 保存材质
            string path = "Assets/Res/Grid/PreviewMaterial.mat";
            AssetDatabase.CreateAsset(previewMaterial, path);
            AssetDatabase.SaveAssets();
            
            Debug.Log("✓ 预览材质已创建: " + path);
            
            // 选中材质
            Selection.activeObject = previewMaterial;
            EditorGUIUtility.PingObject(previewMaterial);
            
            // 自动应用到 PreviewSystem
            ApplyMaterialToPreviewSystem(previewMaterial);
        }

        static void ApplyMaterialToPreviewSystem(Material material)
        {
            TTGJ.Build.PreviewSystem previewSystem = Object.FindObjectOfType<TTGJ.Build.PreviewSystem>();
            if (previewSystem != null)
            {
                SerializedObject so = new SerializedObject(previewSystem);
                so.FindProperty("previewMaterialPrefab").objectReferenceValue = material;
                so.ApplyModifiedProperties();
                
                Debug.Log("✓ 预览材质已自动应用到 PreviewSystem");
                
                EditorUtility.DisplayDialog("完成", 
                    "预览材质创建成功！\n\n" +
                    "材质路径: Assets/Res/Grid/PreviewMaterial.mat\n" +
                    "已自动应用到 PreviewSystem", 
                    "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("完成", 
                    "预览材质创建成功！\n\n" +
                    "请手动将材质拖到 PreviewSystem 的 Preview Material Prefab 字段", 
                    "确定");
            }
        }

        [MenuItem("TTGJ/Build Test/Create Cell Indicator Material")]
        static void CreateCellMaterial()
        {
            // 创建半透明网格材质
            Material cellMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            // 设置为透明模式
            cellMaterial.SetFloat("_Surface", 1);
            cellMaterial.SetFloat("_Blend", 0);
            cellMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            cellMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            cellMaterial.SetFloat("_ZWrite", 0);
            cellMaterial.SetFloat("_AlphaClip", 0);
            cellMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            
            // 设置颜色 - 白色半透明
            cellMaterial.SetColor("_BaseColor", new Color(1, 1, 1, 0.3f));
            cellMaterial.SetColor("_Color", new Color(1, 1, 1, 0.3f));
            
            // 保存材质
            string path = "Assets/Res/Grid/CellIndicatorMaterial.mat";
            AssetDatabase.CreateAsset(cellMaterial, path);
            AssetDatabase.SaveAssets();
            
            Debug.Log("✓ 网格指示器材质已创建: " + path);
            
            Selection.activeObject = cellMaterial;
            EditorGUIUtility.PingObject(cellMaterial);
            
            EditorUtility.DisplayDialog("完成", 
                "网格指示器材质创建成功！\n\n" +
                "材质路径: " + path, 
                "确定");
        }
    }
}



