using UnityEngine;

namespace TTGJ.Entity
{
    public class PlayerEntity : MonoBehaviour, IEntity
    {
        [SerializeField] private float moveSpeed = 1f;
        private CharacterController characterController;

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
            Debug.Log("Player Move: " + direction);
            characterController.Move(new Vector3(direction.x, 0, direction.y) * moveSpeed * Time.deltaTime);
        }

        public void OnOperation() {
            Debug.Log("PLayer operation");
        }

    }
}