using UnityEngine;
using TTGJ.GamePlay;
using System.Collections;

namespace TTGJ.Plant
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class KiwiFruit : MonoBehaviour
    {
        [SerializeField]
        private float bounceSoundInterval = 0.5f; // 弹跳音效间隔
        
        private float lastBounceTime;
        private Rigidbody rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            lastBounceTime = -bounceSoundInterval; // 确保第一次碰撞就能播放音效
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // 保持弹跳音效
            if (Time.time - lastBounceTime >= bounceSoundInterval)
            {
                // 这里添加弹跳音效的播放代码
                Debug.Log("Kiwi fruit bounce sound");
                lastBounceTime = Time.time;
            }
        }
        
        // 当玩家吃掉猕猴桃时调用
        public void OnEat(PlayerController player)
        {
            // 启动角色变成猕猴桃的效果
            //StartCoroutine(KiwiTransformCoroutine(player));
            
            // 销毁当前猕猴桃对象
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }
        
        private IEnumerator KiwiTransformCoroutine(PlayerController player)
        {
            // 保存玩家的原始状态
            Vector3 originalScale = player.transform.localScale;
            //bool originalCanMove = player.CanMove;
            
            // 改变玩家状态，使其变成猕猴桃并滚动前进
            player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // 变大
            //player.CanMove = true;
            
            // 持续30秒
            float duration = 30f;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                // 让玩家自动向前滚动
                player.transform.Translate(Vector3.forward * 5f * Time.deltaTime);
                player.transform.Rotate(Vector3.right * 180f * Time.deltaTime); // 滚动效果
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // 恢复玩家的原始状态
            player.transform.localScale = originalScale;
            //player.CanMove = originalCanMove;
            
            Debug.Log("Player returned to normal after eating kiwi");
        }
    }
}