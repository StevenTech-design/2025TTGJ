using UnityEngine;

namespace TTGJ.Entity
{
    public class PlayerEntity : MonoBehaviour, IEntity
    {
        [SerializeField] private float moveSpeed = 1f;
        private CharacterController characterController;
        private Vector3 moveOffset = Vector3.zero;

        private uint EntityId = 0;
        uint IEntity.EntityId {
            get{ 
                if (EntityId == 0) {
                    EntityId = EntityManager.Instance.GetNextEntityId();
                }
                return EntityId;
            }

        }

        public EntityType EntityType => EntityType.Player;

        private void Awake() { 
            characterController = GetComponent<CharacterController>();
        }

        public void Move(Vector2 direction) { 
            moveOffset += new Vector3(direction.x, 0, direction.y);
        }

        public void OnOperation() {
            Debug.Log("PLayer operation");
        }

        private void FixedUpdate() {
            if (moveOffset == Vector3.zero) {
                return;
            }
            characterController.Move(moveOffset * moveSpeed * UnityEngine.Time.deltaTime);
            moveOffset = Vector3.zero;
        }

    }
}