using System.Collections.Generic;
using TTGJ.Plant;
using Unity.VisualScripting;
using UnityEngine;

namespace TTGJ.GamePlay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float speed = 3.5f;
        [SerializeField]
        private float dropForce = 10;
        [SerializeField]
        private float dropAngle = 60f;
        [SerializeField]
        private float interactDistance = 2f; // 交互距离

        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private Queue<GameObject> _liftQueue = new();
        [SerializeField]
        private float listOffset = 0.2f;
        private float _currentHeight = 0;
        
        // 控制玩家是否可以移动
        public bool CanMove { get; set; } = true;
        
        // 当前装备的洒水壶
        private WateringCan equippedWateringCan;
        
        private void Update()
        { 
            HandleInput();
        }


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void ToMove(Vector3 direction)
        {
            if (!CanMove) return;
            
            transform.rotation = Quaternion.LookRotation(direction);
            _rigidbody.MovePosition(_rigidbody.position + speed * Time.fixedDeltaTime * transform.forward);
        }
        public void ToLift(GameObject target)
        {
            float targetHeight = 0;
            if (target.TryGetComponent<Collider>(out var collider))
            {
                targetHeight = collider.bounds.size.y;
                collider.enabled = false;
            }
            if(target.TryGetComponent<Liftable>(out var liftable))
            {
                liftable.OnLift();
            }
            
            _liftQueue.Enqueue(target);

            target.transform.SetParent(liftArea);
            float height = targetHeight / 2 + _currentHeight;
            Debug.Log("height: " + height);
            target.transform.localPosition = new Vector3(0, height, 0);
            _currentHeight += targetHeight + listOffset;
        }

        public void ToDrop()
        {
            if (_liftQueue.Count <= 0) return;
            Vector3 dropDirection = Quaternion.AngleAxis(dropAngle, transform.right) * transform.forward;
            while (_liftQueue.Count > 0)
            {
                GameObject target = _liftQueue.Dequeue();
                target.transform.SetParent(null);
                if (target.TryGetComponent<Liftable>(out var liftable))
                {
                    liftable.OnDrop(dropDirection.normalized, dropForce);
                }
                
                // 如果是洒水壶，处理装备逻辑
                if (target.TryGetComponent<WateringCan>(out var wateringCan))
                {
                    equippedWateringCan = null;
                    wateringCan.OnDrop();
                }
            }
            _currentHeight = 0;
        }
        
        // 处理键盘输入
        private void HandleInput()
        {
            // K键：吃/使用物品
            if (Input.GetKeyDown(KeyCode.K))
            {
                EatObject();
            }
            
            // J键：浇水
            if (Input.GetKeyDown(KeyCode.J) && equippedWateringCan != null)
            {
                equippedWateringCan.OnUse();
            }
            
            // O键：全局浇水
            if (Input.GetKeyDown(KeyCode.O) && equippedWateringCan != null)
            {
                equippedWateringCan.GlobalWatering();
            }
        }
        
        // 吃物体的方法
        private void EatObject()
        {
            // 射线检测前方的物体
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                GameObject target = hit.collider.gameObject;
                
                // 检查是否是猕猴桃
                if (target.TryGetComponent<KiwiFruit>(out var kiwiFruit))
                {
                    kiwiFruit.OnEat(this);
                }
                // 检查是否是野草
                else if (target.TryGetComponent<WildGrass>(out var wildGrass))
                {
                    wildGrass.OnEat();
                }
            }
        }
        
        // 装备洒水壶
        public void EquipWateringCan(WateringCan wateringCan)
        {
            if (equippedWateringCan != null)
            {
                equippedWateringCan.OnDrop();
            }
            
            equippedWateringCan = wateringCan;
            wateringCan.OnLift(this);
        }

        public void ToPlant(Field field) { 
            GameObject[] plants = _liftQueue.ToArray();
            PlantBase targetPlant = null;
            foreach (var plant in plants) { 
                if(!plant.TryGetComponent<PlantBase>(out var plantBase)) { 
                    continue;
                }
                targetPlant = plantBase;
                break;
            }
            if(targetPlant == null) return;
            GameObject seed = GameObject.Instantiate(targetPlant.gameObject);
            seed.GetComponent<Collider>().enabled = true;
            field.ToToPlanting(seed.GetComponent<PlantBase>());
        }
    }
}


