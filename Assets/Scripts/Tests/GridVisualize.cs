using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualize : MonoBehaviour
{
    public Grid grid;
    public int gridWidth = 100;   // 网格宽度（X 方向格子数）
    public int gridHeight = 20;  // 网格高度（Z 方向格子数）
    public Color gridColor = new Color(0, 1, 0, 0.3f);  // 绿色半透明
    public bool showCells = true;      // 显示格子中心
    public bool showGridLines = true;  // 显示网格线
    public bool showInGame = true;     // 在游戏中显示（运行时）
    public float lineHeight = 0.01f;   // 网格线高度
    
    private void Awake()
    {
        if (grid == null)
            grid = GetComponentInParent<Grid>();
    }
    
    void OnDrawGizmos()
    {
        if (grid == null) return;
        
        Gizmos.color = gridColor;
        
        if (showCells)
        {
            DrawCells();
        }
        
        if (showGridLines)
        {
            DrawGridLines();
        }
    }
    
    // 绘制格子中心点
    void DrawCells()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 cellCenter = grid.CellToWorld(new Vector3Int(x, 0, z));
                // 绘制小立方体表示格子
                Gizmos.DrawWireCube(cellCenter, new Vector3(0.8f, 0.001f, 0.8f));
            }
        }
    }
    
    // 绘制网格线
    void DrawGridLines()
    {
        Vector3 cellSize = grid.cellSize;
        
        // 绘制横向线（沿 X 轴）
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, 0, z));
            Vector3 end = grid.CellToWorld(new Vector3Int(gridWidth, 0, z));
            Gizmos.DrawLine(start, end);
        }
        
        // 绘制纵向线（沿 Z 轴）
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0));
            Vector3 end = grid.CellToWorld(new Vector3Int(x, 0, gridHeight));
            Gizmos.DrawLine(start, end);
        }
    }
    
    // 在游戏运行时也绘制（使用 Debug.DrawLine）
    void Update()
    {
        if (!showInGame || !Application.isPlaying || grid == null)
            return;
            
        if (showGridLines)
        {
            DrawRuntimeGridLines();
        }
    }
    
    void DrawRuntimeGridLines()
    {
        // 绘制横向线
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, 0, z));
            Vector3 end = grid.CellToWorld(new Vector3Int(gridWidth, 0, z));
            start.y = lineHeight;
            end.y = lineHeight;
            Debug.DrawLine(start, end, gridColor);
        }
        
        // 绘制纵向线
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0));
            Vector3 end = grid.CellToWorld(new Vector3Int(x, 0, gridHeight));
            start.y = lineHeight;
            end.y = lineHeight;
            Debug.DrawLine(start, end, gridColor);
        }
    }
}
