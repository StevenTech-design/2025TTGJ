using UnityEngine;

namespace TTGJ.Entity
{
    public class AirShipEntity : MonoBehaviour, IEntity
    {
        [SerializeField] private float moveSpeed = 10f;
        private Rigidbody rigidbody;

        private uint EntityId = 0;
        uint IEntity.EntityId {
            get{ 
                if (EntityId == 0) {
                    EntityId = EntityManager.Instance.GetNextEntityId();
                }
                return EntityId;
            }

        }

        public EntityType EntityType => EntityType.AirShip;

        private void Awake() { 
            rigidbody = GetComponent<Rigidbody>();
        }

        public void Move(Vector3 direction) { 
            rigidbody.MovePosition(rigidbody.position + direction * moveSpeed * Time.deltaTime);
        }

    }
}