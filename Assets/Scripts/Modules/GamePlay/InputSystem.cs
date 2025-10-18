using System.Collections.Generic;
using UnityEngine;
using TTGJ.Plant;
using TTGJ.Npc;
using TTGJ.Framework;
using TTGJ.Interactable;

namespace TTGJ.GamePlay
{
    [System.Serializable]
    public struct MoveKeyCode
    {
        public KeyCode forward;
        public KeyCode back;
        public KeyCode left;
        public KeyCode right;
    }
    public class InputSystem : MonoSingleton<InputSystem>
    {
        [SerializeField]
        private MoveKeyCode currentMoveKeyCode = new MoveKeyCode()
        {
            forward = KeyCode.W,
            back = KeyCode.S,
            left = KeyCode.A,
            right = KeyCode.D
        };
        [SerializeField]
        private PlayerController player;
        [SerializeField]
        private LayerMask pickableLayers = ~0; // 默认全部
        [SerializeField]
        private float rayDistance = 1f;
        [SerializeField]
        private float playerOffsetHeight = 0.2f;
        
        [Header("Debug Visualization")]
        [SerializeField] private bool showDetectionBox = true;
        [SerializeField] private Color boxColor = Color.green;

        private Vector3 GetPlayerOffsetForward()
        {
            return player.transform.forward + new Vector3(0, playerOffsetHeight, 0);
        }
        private Vector3 GetPlayerOffsetPosition()
        {
            return player.transform.position + new Vector3(0, playerOffsetHeight, 0);
        }

        private void Update()
        {
            CheckPlayerLift();
            CheckPlayerDrop();
            CheckPlayerEat();
            CheckInteraction();
        }
        private void FixedUpdate()
        {
            CheckPlayerMove();
        }
        private void CheckPlayerLift()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (player == null) return;
                
                // 使用Box检测替代射线检测
                Vector3 boxCenter = GetPlayerOffsetPosition() + GetPlayerOffsetForward() * 0.5f; // 前方0.5米
                Vector3 boxSize = new Vector3(1f, 1.8f, 0.5f); // 宽1米，高1.8米，深0.5米
                
                Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, player.transform.rotation, pickableLayers);
                
                if (hitColliders.Length == 0)
                {
                    Debug.Log("No objects in range");
                    return;
                }
                
                // 找到最近的物体
                GameObject closest = GetClosestObject(hitColliders);
                if (closest == null) return;
                
                // 检查植物收获
                if (closest.TryGetComponent<PlantBase>(out var plantBase) && plantBase.GetCurrentState() == PlantState.Mature)
                {
                    plantBase.ChangeState(PlantState.Harvest);
                    Debug.Log("Harvesting plant");
                    return;
                }

                if (plantBase != null && (plantBase.GetCurrentState() == PlantState.Germination || plantBase.GetCurrentState() == PlantState.Mature))
                {
                    Debug.Log("No harvestable plant");
                    return;
                }
                
                // 检查可拾取物体
                if (closest.TryGetComponent<Liftable>(out var liftable))
                {
                    if (!liftable.IsLiftable()) return;
                    player.ToLift(closest);
                    return;
                }
            }
        }
        private void CheckPlayerDrop()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                player.ToDrop();
            }
        }
        private void CheckPlayerMove()
        {
            if (player == null) return;

            Vector3 direction = Vector3.zero;

            if (Input.GetKey(currentMoveKeyCode.forward))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(currentMoveKeyCode.back))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(currentMoveKeyCode.left))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(currentMoveKeyCode.right))
            {
                direction += Vector3.right;
            }
            if (direction != Vector3.zero) { 
                player.ToMove(direction);
            }
        }
        
        private GameObject GetClosestObject(Collider[] colliders)
        {
            GameObject closest = null;
            float closestDistance = float.MaxValue;
            Vector3 playerPos = player.transform.position;
            
            // 优先检测植物，按优先级排序
            List<GameObject> plants = new List<GameObject>();
            List<GameObject> otherObjects = new List<GameObject>();
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject == player.gameObject) continue; // 排除玩家自己
                
                if (collider.gameObject.TryGetComponent<PlantBase>(out _))
                {
                    plants.Add(collider.gameObject);
                }
                else
                {
                    otherObjects.Add(collider.gameObject);
                }
            }
            
            // 优先从植物中选择最近的
            if (plants.Count > 0)
            {
                foreach (var plant in plants)
                {
                    float distance = Vector3.Distance(playerPos, plant.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = plant;
                    }
                }
            }
            // 如果没有植物，再从其他物体中选择
            else
            {
                foreach (var obj in otherObjects)
                {
                    float distance = Vector3.Distance(playerPos, obj.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = obj;
                    }
                }
            }
            
            return closest;
        }
        
        private GameObject GetInteractionTarget(Collider[] colliders)
        {
            Vector3 playerPos = player.transform.position;
            float closestDistance = float.MaxValue;
            GameObject closest = null;
            
            // 按优先级排序：NPC > 植物 > 土地
            List<GameObject> npcs = new List<GameObject>();
            List<GameObject> plants = new List<GameObject>();
            List<GameObject> fields = new List<GameObject>();
            List<GameObject> others = new List<GameObject>();
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject == player.gameObject) continue;
                
                if (collider.gameObject.TryGetComponent<NPCBase>(out _))
                {
                    npcs.Add(collider.gameObject);
                }
                else if (collider.gameObject.TryGetComponent<PlantBase>(out _))
                {
                    plants.Add(collider.gameObject);
                }
                else if (collider.gameObject.TryGetComponent<Field>(out _))
                {
                    fields.Add(collider.gameObject);
                }
                else
                {
                    others.Add(collider.gameObject);
                }
            }
            
            // 优先选择NPC
            if (npcs.Count > 0)
            {
                foreach (var npc in npcs)
                {
                    float distance = Vector3.Distance(playerPos, npc.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = npc;
                    }
                }
                return closest;
            }
            
            // 其次选择植物
            if (plants.Count > 0)
            {
                foreach (var plant in plants)
                {
                    float distance = Vector3.Distance(playerPos, plant.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = plant;
                    }
                }
                return closest;
            }
            
            // 最后选择土地
            if (fields.Count > 0)
            {
                foreach (var field in fields)
                {
                    float distance = Vector3.Distance(playerPos, field.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = field;
                    }
                }
                return closest;
            }
            
            // 其他物体
            if (others.Count > 0)
            {
                foreach (var obj in others)
                {
                    float distance = Vector3.Distance(playerPos, obj.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = obj;
                    }
                }
            }
            
            return closest;
        }
        
        private void OnDrawGizmos()
        {
            if (!showDetectionBox || player == null) return;
            
            // 绘制检测Box
            Vector3 boxCenter = GetPlayerOffsetPosition() + GetPlayerOffsetForward() * 0.5f;
            Vector3 boxSize = new Vector3(1f, 1.8f, 0.5f);
            
            Gizmos.color = boxColor;
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, player.transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            
            // 绘制检测到的物体
            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, player.transform.rotation, pickableLayers);
            Gizmos.matrix = Matrix4x4.identity;
            
            foreach (var collider in hitColliders)
            {
                if (collider.gameObject == player.gameObject) continue;
                
                Gizmos.color = collider.gameObject.TryGetComponent<PlantBase>(out _) ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(collider.transform.position, 0.2f);
            }
        }
        
        private void CheckInteraction()
        {
            if (!Input.GetKeyDown(KeyCode.J))
            {
                return;
            }
            
            // 使用Box检测替代射线检测
            Vector3 boxCenter = GetPlayerOffsetPosition() + GetPlayerOffsetForward() * 0.5f;
            Vector3 boxSize = new Vector3(1f, 1.8f, 0.5f);
            
            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, player.transform.rotation, pickableLayers);
            
            if (hitColliders.Length == 0)
            {
                Debug.Log("No objects in interaction range");
                return;
            }
            
            // 按优先级处理交互
            GameObject target = GetInteractionTarget(hitColliders);
            if (target == null) return;
            
            // NPC交互
            if (target.TryGetComponent<NPCBase>(out var npc))
            {
                npc.OnInteract();
                Debug.Log("Interacting with NPC");
                return;
            }
            
            // 植物浇水
            if (target.TryGetComponent<PlantBase>(out var plant) && 
                (plant.GetCurrentState() == PlantState.Mature || plant.GetCurrentState() == PlantState.Germination))
            {
                plant.OnWatering();
                Debug.Log("Watering plant");
                return;
            }
            
            // 土地种植
            if (target.TryGetComponent<Field>(out var field) && !field.IsPlanted())
            {
                Debug.Log("Planting plant");
                player.ToPlant(field);
                return;
            }
        }
        private void CheckPlayerEat() { 
            if (!Input.GetKeyDown(KeyCode.K))
            {
                return;
            }
            player.ToEat();
        }
        public MoveKeyCode GetMoveKeyCode()
        {
            return currentMoveKeyCode;
        }
        public void SetMoveKeyCode(MoveKeyCode keyCode)
        {
            currentMoveKeyCode = keyCode;
        }
    }
}