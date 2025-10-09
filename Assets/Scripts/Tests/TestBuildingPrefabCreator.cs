using UnityEngine;

namespace TTGJ.Tests
{
    /// <summary>
    /// 在编辑器中创建简单的测试建筑预制体
    /// </summary>
    public class TestBuildingPrefabCreator : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("TTGJ/Build Test/Create Test Floor Prefab")]
        static void CreateFloorPrefab()
        {
            CreateTestPrefab("TestFloor", new Vector3(1, 0.1f, 1), Color.gray, "Assets/Res/Grid/TestFloor.prefab");
        }

        [UnityEditor.MenuItem("TTGJ/Build Test/Create Test Furniture 1x1 Prefab")]
        static void CreateFurniture1x1Prefab()
        {
            CreateTestPrefab("TestFurniture1x1", new Vector3(0.8f, 1, 0.8f), Color.blue, "Assets/Res/Grid/TestFurniture1x1.prefab");
        }

        [UnityEditor.MenuItem("TTGJ/Build Test/Create Test Furniture 2x2 Prefab")]
        static void CreateFurniture2x2Prefab()
        {
            CreateTestPrefab("TestFurniture2x2", new Vector3(1.8f, 1.5f, 1.8f), Color.red, "Assets/Res/Grid/TestFurniture2x2.prefab");
        }

        [UnityEditor.MenuItem("TTGJ/Build Test/Create Test Furniture 1x2 Prefab")]
        static void CreateFurniture1x2Prefab()
        {
            CreateTestPrefab("TestFurniture1x2", new Vector3(0.8f, 1.2f, 1.8f), Color.green, "Assets/Res/Grid/TestFurniture1x2.prefab");
        }

        static void CreateTestPrefab(string name, Vector3 size, Color color, string path)
        {
            // 创建游戏对象
            GameObject prefabObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefabObj.name = name;
            prefabObj.transform.localScale = size;

            // 设置材质颜色
            Renderer renderer = prefabObj.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            renderer.material = mat;

            // 移除碰撞体（因为建筑系统使用网格放置，不需要碰撞体）
            Collider collider = prefabObj.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }

            // 保存为预制体
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(prefabObj, path);
            Debug.Log($"创建测试预制体: {path}");

            // 删除场景中的临时对象
            DestroyImmediate(prefabObj);
        }

        [UnityEditor.MenuItem("TTGJ/Build Test/Create Ground Plane")]
        static void CreateGroundPlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "GroundPlane";
            plane.transform.position = Vector3.zero;
            plane.transform.localScale = new Vector3(5, 1, 5); // 50x50 单位的地面

            // 设置到 Placement 层
            plane.layer = LayerMask.NameToLayer("Default");

            // 设置材质
            Renderer renderer = plane.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.8f, 0.8f, 0.7f);
            renderer.material = mat;

            UnityEditor.Selection.activeGameObject = plane;
            Debug.Log("创建地面平面，请将其 Layer 设置为 Placement");
        }
#endif
    }
}

