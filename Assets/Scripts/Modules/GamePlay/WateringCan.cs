using UnityEngine;
using TTGJ.Plant;

namespace TTGJ.GamePlay
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class WateringCan : MonoBehaviour
    {
        [SerializeField]
        private float waterEffectRadius = 3f; // 浇水效果半径
        [SerializeField]
        private float waterForce = 5f; // 水的冲击力
        [SerializeField]
        private GameObject waterEffectPrefab; // 浇水效果预制体
        
        private bool isEquipped = false;
        private PlayerController ownerPlayer;
        
        // 当玩家举起洒水壶时调用
        public void OnLift(PlayerController player)
        {
            isEquipped = true;
            ownerPlayer = player;
            
            // 禁用碰撞器和刚体
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            
            // 跟随玩家
            transform.SetParent(player.transform);
            transform.localPosition = new Vector3(1f, 1f, 0f);
            transform.localRotation = Quaternion.identity;
        }
        
        // 当玩家放下洒水壶时调用
        public void OnDrop()
        {
            isEquipped = false;
            ownerPlayer = null;
            
            // 启用碰撞器和刚体
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            
            // 断开父子关系
            transform.SetParent(null);
        }
        
        // 当玩家使用洒水壶时调用
        public void OnUse()
        {
            // 播放浇水效果
            PlayWaterEffect();
            
            // 查找范围内的植物并浇水
            WaterPlantsInRange();
        }
        
        private void PlayWaterEffect()
        {
            if (waterEffectPrefab != null && ownerPlayer != null)
            {
                // 在玩家前方生成浇水效果
                Vector3 waterPosition = ownerPlayer.transform.position + ownerPlayer.transform.forward * 1.5f + Vector3.up * 0.5f;
                GameObject waterEffect = Instantiate(waterEffectPrefab, waterPosition, Quaternion.identity);
                
                // 设置效果朝向
                waterEffect.transform.forward = ownerPlayer.transform.forward;
                
                // 2秒后销毁效果
                Destroy(waterEffect, 2f);
            }
        }
        
        private void WaterPlantsInRange()
        {
            if (ownerPlayer == null) return;
            
            // 查找玩家前方范围内的所有碰撞体
            Vector3 center = ownerPlayer.transform.position + ownerPlayer.transform.forward * 1.5f;
            Collider[] hitColliders = Physics.OverlapSphere(center, waterEffectRadius);
            
            foreach (Collider collider in hitColliders)
            {
                // 检查是否是植物
                PlantBase plant = collider.GetComponent<PlantBase>();
                if (plant != null)
                {
                    // 给植物浇水
                    plant.OnWatering();
                    Debug.Log($"Watered plant: {plant.gameObject.name}");
                    
                    // 给植物添加水的冲击力
                    Rigidbody rb = plant.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic)
                    {
                        Vector3 forceDirection = (plant.transform.position - center).normalized;
                        rb.AddForce(forceDirection * waterForce, ForceMode.Impulse);
                    }
                }
            }
        }
        
        // 全局浇水功能（根据表格要求：到处洒水，删除所有东西取消染色）
        public void GlobalWatering()
        {
            // 找到场景中所有可染色的物体并取消染色
            GameObject[] colorableObjects = GameObject.FindGameObjectsWithTag("Colorable");
            foreach (GameObject obj in colorableObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 恢复原始颜色
                    renderer.material.color = Color.white;
                    Debug.Log($"Removed color from: {obj.name}");
                }
            }
        }
    }
}