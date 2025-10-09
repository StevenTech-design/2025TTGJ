using UnityEngine;

/// <summary>
/// 在运行时（Game 视图）显示网格
/// 使用 GL 绘制，性能优秀
/// </summary>
public class GridVisualizeRuntime : MonoBehaviour
{
    [Header("网格配置")]
    public Grid grid;
    public int gridWidth = 20;
    public int gridHeight = 20;
    
    [Header("显示设置")]
    public Color gridColor = new Color(0, 1, 0, 0.5f);
    public bool showInGame = true;  // 是否在游戏中显示
    public float lineHeight = 0.01f; // 网格线高度
    
    [Header("材质")]
    public Material lineMaterial;  // 用于绘制线的材质
    
    private void Awake()
    {
        if (grid == null)
            grid = GetComponentInParent<Grid>();
            
        // 如果没有指定材质，创建默认材质
        if (lineMaterial == null)
        {
            CreateLineMaterial();
        }
    }
    
    void CreateLineMaterial()
    {
        // 创建一个简单的不受光照影响的材质
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }
    
    void OnRenderObject()
    {
        if (!showInGame || grid == null || lineMaterial == null)
            return;
            
        lineMaterial.SetPass(0);
        
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        
        GL.Begin(GL.LINES);
        GL.Color(gridColor);
        
        DrawGridLines();
        
        GL.End();
        GL.PopMatrix();
    }
    
    void DrawGridLines()
    {
        // 绘制横向线（沿 X 轴）
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, 0, z));
            Vector3 end = grid.CellToWorld(new Vector3Int(gridWidth, 0, z));
            
            start.y = lineHeight;
            end.y = lineHeight;
            
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
        }
        
        // 绘制纵向线（沿 Z 轴）
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0));
            Vector3 end = grid.CellToWorld(new Vector3Int(x, 0, gridHeight));
            
            start.y = lineHeight;
            end.y = lineHeight;
            
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
        }
    }
    
    // 编辑器中也显示（可选）
    void OnDrawGizmos()
    {
        if (grid == null) return;
        
        Gizmos.color = gridColor;
        
        // 绘制网格线
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, 0, z));
            Vector3 end = grid.CellToWorld(new Vector3Int(gridWidth, 0, z));
            Gizmos.DrawLine(start, end);
        }
        
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0));
            Vector3 end = grid.CellToWorld(new Vector3Int(x, 0, gridHeight));
            Gizmos.DrawLine(start, end);
        }
    }
}

