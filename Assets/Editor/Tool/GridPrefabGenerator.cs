using UnityEngine;
using UnityEditor;
using TTGJ.Build;

public class GridPrefabGenerator : EditorWindow
{
    GameObject planePrefab;
    int rows = 10;
    int cols = 10;
    float cellSize = 1f;

    [MenuItem("Tools/Grid Prefab Generator")]
    public static void ShowWindow()
    {
        GetWindow<GridPrefabGenerator>("网格生成器");
    }

    private void OnGUI()
    {
        GUILayout.Label("创建由Plane组成的网格预制体", EditorStyles.boldLabel);

        planePrefab = (GameObject)EditorGUILayout.ObjectField("单格Prefab", planePrefab, typeof(GameObject), false);
        rows = EditorGUILayout.IntField("行数 (Rows)", rows);
        cols = EditorGUILayout.IntField("列数 (Cols)", cols);
        cellSize = EditorGUILayout.FloatField("格子大小", cellSize);

        GUILayout.Space(10);

        if (GUILayout.Button("生成网格"))
        {
            GenerateGrid();
        }
    }

    private void GenerateGrid()
    {
        if (planePrefab == null)
        {
            EditorUtility.DisplayDialog("错误", "请先指定 Plane Prefab！", "确定");
            return;
        }

        GameObject root = new GameObject($"Grid_{rows}x{cols}");
        Undo.RegisterCreatedObjectUndo(root, "Create Grid");

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = new Vector3(c * cellSize, 0, r * cellSize);
                GameObject cell = (GameObject)PrefabUtility.InstantiatePrefab(planePrefab);
                cell.transform.SetParent(root.transform, false);
                cell.transform.localPosition = pos;
                cell.name = $"Cell_{r}_{c}";
                cell.transform.localScale = Vector3.one * (cellSize / 10f); // Plane默认10x10大小
                cell.AddComponent<Cell>();
                cell.GetComponent<Cell>().x = r;
                cell.GetComponent<Cell>().y = c;
            }
        }

        EditorUtility.DisplayDialog("完成", $"生成 {rows * cols} 个格子！", "OK");

        string path = EditorUtility.SaveFilePanelInProject("保存网格Prefab", root.name, "prefab", "选择保存位置");
        if (!string.IsNullOrEmpty(path))
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            DestroyImmediate(root);
            Debug.Log($"✅ 网格Prefab已保存: {path}");
        }
    }
}