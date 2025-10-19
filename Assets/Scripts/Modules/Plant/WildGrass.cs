using UnityEngine;
using TTGJ.GamePlay;
using System.Collections.Generic;

namespace TTGJ.Plant
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class WildGrass : PlantBase
    {
        // 固定刷新点列表
        [SerializeField]
        private List<Transform> refreshPoints = new List<Transform>();
        
        [SerializeField]
        private float refreshInterval = 10f; // 刷新间隔
        
        [SerializeField]
        private GameObject grassPrefab; // 草的预制体
        
        private float timer = 0f;
        private bool isHarvested = false;
        
        public override void OnMature()
        {
            base.OnMature();
            // 成熟后开始计时刷新
            timer = refreshInterval;
        }
        
        private void Update()
        {
            if (currentState != PlantState.Mature || isHarvested)
                return;
            
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                RefreshGrass();
                timer = refreshInterval;
            }
        }
        
        private void RefreshGrass()
        {
            if (refreshPoints.Count > 0 && grassPrefab != null)
            {
                // 选择一个随机的刷新点
                int randomIndex = Random.Range(0, refreshPoints.Count);
                Transform point = refreshPoints[randomIndex];
                
                // 检查该位置是否已有草
                Collider[] existingGrasses = Physics.OverlapSphere(point.position, 0.5f);
                bool hasGrass = false;
                foreach (Collider col in existingGrasses)
                {
                    if (col.GetComponent<WildGrass>() != null)
                    {
                        hasGrass = true;
                        break;
                    }
                }
                
                // 如果没有草，则生成新的草
                if (!hasGrass)
                {
                    Instantiate(grassPrefab, point.position, Quaternion.identity);
                }
            }
        }
        
        protected override void OnHarvest()
        {
            // 无敌撞体
            collider.isTrigger = true;
            rigidbody.isKinematic = true;
            isHarvested = true;
        }
        
        // 当玩家吃掉草时调用
        public void OnEat()
        {
            // 触发NPC群叫效果
            TriggerNPCGroupCall();
            
            // 销毁当前草对象
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }
        
        private void TriggerNPCGroupCall()
        {
            // 找到场景中所有NPC并触发对话群叫
            // 这里简化实现，实际项目中可能需要更复杂的NPC系统
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            foreach (GameObject npc in npcs)
            {
                // 检查是否是羊，如果是羊则允许正常对话
                if (!npc.CompareTag("Sheep"))
                {
                    // 触发群叫效果
                    Debug.Log($"NPC {npc.name} is making noise after eating grass");
                }
            }
        }
    }
}