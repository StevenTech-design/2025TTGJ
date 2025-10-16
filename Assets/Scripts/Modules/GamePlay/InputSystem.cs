using UnityEngine;
using TTGJ.Plant;
using TTGJ.Npc;
using TTGJ.Framework;

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

        private Vector3 GetPlayerOffsetForward()
        {
            return player.transform.forward + new Vector3(0, 0.2f, 0);
        }
        private Vector3 GetPlayerOffsetPosition()
        {
            return player.transform.position + new Vector3(0, 0.2f, 0);
        }

        private void Update()
        {
            CheckPlayerLift();
            CheckPlayerDrop();
            CheckInteraction();
            //CheckPlayerMove();
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
                Ray ray = new Ray(GetPlayerOffsetPosition(), GetPlayerOffsetForward());
                if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, pickableLayers))
                {
                    return;
                }
                if (hit.collider.gameObject.TryGetComponent<PlantBase>(out var plantBase) && plantBase.GetCurrentState() == PlantState.Mature)
                {
                    plantBase.ChangeState(PlantState.Harvest);
                    return;
                }

                if (plantBase != null && (plantBase.GetCurrentState() == PlantState.Germination || plantBase.GetCurrentState() == PlantState.Mature))
                {
                    return;
                }
                if (hit.collider.gameObject.TryGetComponent<Liftable>(out var liftable))
                {
                    if (!liftable.IsLiftable()) return;
                    player.ToLift(hit.collider.gameObject);
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
            player.ToMove(direction);
        }
        private void CheckInteraction()
        {
            if (!Input.GetKeyDown(KeyCode.J))
            {
                return;
            }
            Ray forwardRay = new Ray(GetPlayerOffsetPosition(), GetPlayerOffsetForward());
            if (Physics.Raycast(forwardRay, out RaycastHit hit, rayDistance, pickableLayers))
            {
                if (hit.collider.gameObject.TryGetComponent<NPCBase>(out var npc))
                {
                    npc.OnInteract();
                    Debug.Log("Interacting with NPC");
                    return;
                }
                if (hit.collider.gameObject.TryGetComponent<PlantBase>(out var plant)
                                && plant.GetCurrentState() == PlantState.Mature
                                || plant.GetCurrentState() == PlantState.Germination)
                {
                    plant.OnWatering();
                    Debug.Log("Watering plant");
                    return;
                }


            }
            Ray downRay = new Ray(GetPlayerOffsetPosition(), -transform.up);
            if (Physics.Raycast(downRay, out RaycastHit hitDown, rayDistance, pickableLayers))
            {
                Debug.Log("Checking field");
                if (hitDown.collider.gameObject.TryGetComponent<Field>(out var field) && !field.IsPlanted())
                {
                    Debug.Log("Planting plant");
                    player.ToPlant(field);
                    return;
                }
            }

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