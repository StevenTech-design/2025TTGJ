using UnityEngine;

namespace TTGJ.Interactable
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private void OnCollisionEnter(Collision collision)
        {
            TeleportObject(collision.gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            TeleportObject(other.gameObject);
        }
        private void TeleportObject(GameObject obj) { 
            Ray upRay = new Ray(obj.transform.position, Vector3.up);
            if (Physics.Raycast(upRay, out RaycastHit hit, 1000)
            && obj.TryGetComponent<ITeleport>(out var teleport))
            {
                teleport.OnTeleport(target.transform);
                return;
            }
            obj.transform.position = target.position;
        }
    }
}