using UnityEngine;
using TTGJ.GamePlay;

namespace TTGJ.Plant
{
    public class KiwiFruitTree : PlantBase
    {
        [SerializeField]
        private GameObject kiwiFruitPrefab; // 猕猴桃预制体
        [SerializeField]
        private float spawnInterval = 5f; 
        [SerializeField]
        private float fruitForce = 2f; // 果实掉落的力量
        
        private float timer = 0f;
        
        public override void OnMature()
        {
            base.OnMature();
            // 成熟后立即开始生成猕猴桃
            timer = spawnInterval; 
        }
        
        private void Update()
        {
            if (currentState != PlantState.Mature)
                return;
            
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnKiwiFruit();
                timer = spawnInterval;
            }
        }
        
        private void SpawnKiwiFruit()
        {
            if (kiwiFruitPrefab != null)
            {
                // 生成猕猴桃
                GameObject kiwi = Instantiate(kiwiFruitPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
                
                // 给猕猴桃一个随机的水平方向力，使其掉落
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f)).normalized;
                Rigidbody rb = kiwi.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(randomDirection * fruitForce, ForceMode.Impulse);
                }
            }
        }
        
        protected override void OnHarvest()
        {
            // 猕猴桃树不能移动，但仍然需要响应收获操作
            Debug.Log("猕猴桃树不能被移动");
        }
    }
}