using UnityEngine;

namespace TTGJ.GamePlay
{
    public class LotusLeaf : MonoBehaviour
    {
        [SerializeField]
        private Renderer leafRenderer; // 莲叶的渲染器
        
        [SerializeField]
        private float transparency = 0.5f; // 透明度
        
        [SerializeField]
        private bool showPumpkinHead = true; // 是否显示南瓜头
        
        [SerializeField]
        private Color tomatoColor = Color.red; // 番茄染色颜色
        
        [SerializeField]
        private string billiardsIslandTag = "BilliardsIsland";
        
        private void Start()
        {
            // 设置莲叶为透明平台
            if (leafRenderer != null)
            {
                Color color = leafRenderer.material.color;
                color.a = transparency;
                leafRenderer.material.color = color;
                leafRenderer.material.shader = Shader.Find("Transparent/Diffuse");
            }
            
            // 不可交互
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // 处理不同物体接触莲叶的逻辑
            HandleObjectInteraction(other.gameObject);
        }
        
        private void OnTriggerStay(Collider other)
        {
            // 持续处理物体与莲叶的交互
            UpdateObjectInteraction(other.gameObject);
        }
        
        private void HandleObjectInteraction(GameObject obj)
        {
            // 南瓜头可见
            if (showPumpkinHead && obj.CompareTag("PumpkinHead"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
            
            // 番茄可染色
            if (obj.CompareTag("Tomato"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = tomatoColor;
                    Debug.Log("Tomato dyed red on lotus leaf");
                }
            }
            
            // 检查是否是台球岛相关物体
            CheckBilliardsIslandPath(obj);
        }
        
        private void UpdateObjectInteraction(GameObject obj)
        {
            // 可以在这里添加持续更新的逻辑
        }
        
        // 检查台球岛道路
        private void CheckBilliardsIslandPath(GameObject obj)
        {
            // 如果是台球岛相关物体，确保道路可见
            if (obj.CompareTag("Path") && IsNearBilliardsIsland())
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
                Debug.Log("Path to billiards island is visible");
            }
        }
        
        // 检查是否靠近台球岛
        private bool IsNearBilliardsIsland()
        {
            GameObject[] islands = GameObject.FindGameObjectsWithTag(billiardsIslandTag);
            foreach (GameObject island in islands)
            {
                if (Vector3.Distance(transform.position, island.transform.position) < 20f) // 20米范围内
                {
                    return true;
                }
            }
            return false;
        }
    }
}