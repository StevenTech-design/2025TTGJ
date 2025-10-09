using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 使用 LineRenderer 在游戏中显示网格
/// 更易配置，可以设置线宽、材质等
/// </summary>
public class GridVisualizeLineRenderer : MonoBehaviour
{
    [Header("网格配置")]
    public Grid grid;
    public int gridWidth = 20;
    public int gridHeight = 20;
    
    [Header("显示设置")]
    public Color gridColor = new Color(0, 1, 0, 0.5f);
    public float lineWidth = 0.05f;
    public float lineHeight = 0.01f;
    public Material lineMaterial;
    
    private List<GameObject> lineObjects = new List<GameObject>();
    
    private void Awake()
    {
        if (grid == null)
            grid = GetComponentInParent<Grid>();
    }
    
    private void Start()
    {
        CreateGridLines();
    }
    
    void CreateGridLines()
    {
        // 清除旧的线
        ClearLines();
        
        // 创建横向线
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, 0, z)) - new Vector3(0.5f, 0, 0.5f);
            Vector3 end = grid.CellToWorld(new Vector3Int(gridWidth, 0, z)) - new Vector3(0.5f, 0, 0.5f);
            CreateLine(start, end, $"GridLine_H_{z}");
        }
        
        // 创建纵向线
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0)) - new Vector3(0.5f, 0, 0.5f);
                Vector3 end = grid.CellToWorld(new Vector3Int(x, 0, gridHeight)) - new Vector3(0.5f, 0, 0);
                CreateLine(start, end, $"GridLine_V_{x}");
        }
    }
    
    void CreateLine(Vector3 start, Vector3 end, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        
        // 设置材质
        if (lineMaterial != null)
        {
            lr.material = lineMaterial;
        }
        else
        {
            // 使用默认材质
            lr.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        // 设置线条属性
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        
        // 设置位置
        start.y = lineHeight;
        end.y = lineHeight;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        
        // 禁用阴影
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        
        lineObjects.Add(lineObj);
    }
    
    void ClearLines()
    {
        foreach (var obj in lineObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        lineObjects.Clear();
    }
    
    private void OnDestroy()
    {
        ClearLines();
    }
    
    // 运行时更新网格（如果需要动态改变大小）
    public void UpdateGrid()
    {
        CreateGridLines();
    }
}

