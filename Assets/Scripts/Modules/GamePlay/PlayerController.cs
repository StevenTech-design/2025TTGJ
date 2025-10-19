using System.Collections.Generic;
using TTGJ.Plant;
using TTGJ.Interactable;
using UnityEngine;
using TTGJ.Framework;

namespace TTGJ.GamePlay
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        public float speed = 3.5f;
        [SerializeField]
        private float dropForce = 10;
        [SerializeField]
        private float dropAngle = 60f;
        [SerializeField]
        private float interactDistance = 2f; // 交互距离

        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private List<GameObject> _liftList = new();
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
            _rigidbody = GetComponentInChildren<Rigidbody>();
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
                collider.enabled = false;
            }
            if (target.TryGetComponent<MeshFilter>(out var filter))
            {
                targetHeight = filter.sharedMesh.bounds.size.y * target.transform.localScale.y;
            }

            if (target.TryGetComponent<Liftable>(out var liftable))
            {
                liftable.OnLift();
            }

            _liftList.Add(target);

            target.transform.SetParent(liftArea);
            float height = targetHeight / 2 + _currentHeight;
            target.transform.localRotation = Quaternion.identity;
            target.transform.localPosition = new Vector3(0, height, 0);
            _currentHeight += targetHeight + listOffset;
        }

        public void ToDrop()
        {
            if (_liftList.Count <= 0) return;
            Vector3 dropDirection = (transform.forward + Vector3.up).normalized;
            while (_liftList.Count > 0)
            {
                GameObject target = _liftList[0];
                _liftList.RemoveAt(0);
                target.transform.SetParent(null);
                if (target.TryGetComponent<Liftable>(out var liftable))
                {
                    liftable.OnDrop(dropDirection, dropForce);
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
        public bool CanDrop()
        {
            return _liftList.Count > 0;
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

        public void ToPlant(Field field)
        {
            if (field.IsPlanted()) return;
            PlantBase targetPlant = null;
            foreach (var plant in _liftList)
            {
                if (!plant.TryGetComponent<PlantBase>(out var plantBase))
                {
                    continue;
                }
                targetPlant = plantBase;
                break;
            }
            if (targetPlant == null || targetPlant.GetCurrentState() != PlantState.Seed) return;
            GameObject seed = GameObject.Instantiate(targetPlant.gameObject);
            seed.GetComponent<Collider>().enabled = true;
            field.ToPlanting(seed.GetComponent<PlantBase>());
        }
        public bool CanEat()
        {
            foreach (var list in _liftList)
            {
                if (list.TryGetComponent<IEatable>(out var eatable) && eatable.CanEat())
                {
                    return true;
                }
            }
            return false;
        }
        public void ToEat()
        {
            PlantBase targetPlant = null;
            foreach (var plant in _liftList)
            {
                if (!plant.TryGetComponent<PlantBase>(out var plantBase)
                    || plantBase.GetCurrentState() != PlantState.Harvest)
                {
                    continue;
                }
                targetPlant = plantBase;
                break;
            }
            if (targetPlant == null) return;
            targetPlant.OnEat();
            ObjectPoolManager.Instance.ReturnGameObjectToPool(targetPlant.gameObject);
            RefleshLiftQueue();
        }
        private void RefleshLiftQueue()
        {
            _currentHeight = 0;

            for (int i = _liftList.Count - 1; i >= 0; i--)
            {
                if (_liftList[i] == null)
                {
                    _liftList.RemoveAt(i);
                    continue;
                }

                float targetHeight = 0;
                if (_liftList[i].TryGetComponent<MeshFilter>(out var filter))
                {
                    targetHeight = filter.sharedMesh.bounds.size.y * _liftList[i].transform.localScale.y;
                }

                float height = targetHeight / 2 + _currentHeight;
                _liftList[i].transform.SetLocalPositionAndRotation(new Vector3(0, height, 0), Quaternion.identity);
                _currentHeight += targetHeight + listOffset;
            }
        }
    }
}


