using UnityEngine;
using System.Collections.Generic;

namespace TTGJ.GamePlay
{
    public class SeaDestroyer : MonoBehaviour
    {
        [SerializeField]
        private float sinkSpeed = 1f; // 下沉速度
        
        [SerializeField]
        private float destroyDelay = 3f; // 销毁延迟时间
        
        [SerializeField]
        private GameObject splashEffectPrefab; // 溅水效果预制体
        
        // 存储正在被销毁的物体
        private Dictionary<GameObject, float> objectsToDestroy = new Dictionary<GameObject, float>();
        
        private void OnTriggerEnter(Collider other)
        {
            // 如果物体已经在销毁列表中，跳过
            if (objectsToDestroy.ContainsKey(other.gameObject))
                return;
            
            // 播放溅水效果
            PlaySplashEffect(other.transform.position);
            
            // 添加物体到销毁列表
            objectsToDestroy.Add(other.gameObject, Time.time + destroyDelay);
            
            Debug.Log($"Object {other.gameObject.name} entered the sea and will be destroyed");
        }
        
        private void OnTriggerStay(Collider other)
        {
            // 如果物体在销毁列表中，让它下沉
            if (objectsToDestroy.ContainsKey(other.gameObject))
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    // 应用向下的力使其下沉
                    rb.AddForce(Vector3.down * sinkSpeed, ForceMode.Force);
                }
                else
                {
                    // 如果没有刚体，直接移动位置
                    other.transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
                }
            }
        }
        
        private void Update()
        {
            // 检查并销毁应该被销毁的物体
            List<GameObject> objectsToRemove = new List<GameObject>();
            
            foreach (var item in objectsToDestroy)
            {
                if (Time.time >= item.Value)
                {
                    objectsToRemove.Add(item.Key);
                }
            }
            
            // 移除并销毁物体
            foreach (GameObject obj in objectsToRemove)
            {
                objectsToDestroy.Remove(obj);
                Destroy(obj);
                Debug.Log($"Object {obj.name} was destroyed in the sea");
            }
        }
        
        private void PlaySplashEffect(Vector3 position)
        {
            if (splashEffectPrefab != null)
            {
                GameObject effect = Instantiate(splashEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
        
        // 特殊处理：物体在空中漂浮（根据表格中的"A要在空中漂浮"）
        public void MakeObjectFloatInAir(GameObject obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 使物体在空中漂浮
                rb.useGravity = false;
                rb.isKinematic = true;
                
                // 给物体一个向上的力或设置位置
                Vector3 floatPosition = new Vector3(obj.transform.position.x, obj.transform.position.y + 2f, obj.transform.position.z);
                obj.transform.position = floatPosition;
                
                Debug.Log($"Object {obj.name} is now floating in the air");
            }
        }
    }
}