using UnityEngine;
using TTGJ.Env;

namespace TTGJ.Entity
{
    public class AirShipEntity : MonoBehaviour, IEntity
    {
        [SerializeField] private float moveSpeed = 10f;
        private Rigidbody rigidbody;
        private bool isMoor = false;
        private bool isDriving = false;
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

        public EntityType EntityType => EntityType.AirShip;

        private void Awake() { 
            rigidbody = GetComponent<Rigidbody>();
        }

        public void Move(Vector3 direction) {
            moveOffset += direction * moveSpeed;
            Debug.Log("Move: " + moveOffset);
        }

        public void OnOperation() {
            if(CheckInDrivingRoom() && !isDriving)
            {
                isDriving = true;
                return;
            }
            if (CheckInPort() && isDriving) {
                isMoor = !isMoor;
            }

        }

        public bool CheckInPort() {
            return true;
        }
        public bool CheckInDrivingRoom() {
            return true;
        }
        public void CancelDriving() { 
            isDriving = false;
        }

        private void FixedUpdate() {
            if (isMoor) {
                return;
            }
            moveOffset += Weather.Instance.GetWindAtPosition(transform.position);
            rigidbody.MovePosition(rigidbody.position + moveOffset * UnityEngine.Time.deltaTime);
            moveOffset = Vector3.zero;
        }

    }
}