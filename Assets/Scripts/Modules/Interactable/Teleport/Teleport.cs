using UnityEngine;

namespace TTGJ.Interactable
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private void OnCollisionEnter(Collision collision)
        {
            Ray upRay = new Ray(collision.gameObject.transform.position, Vector3.up);
            if (Physics.Raycast(upRay, out RaycastHit hit, 1000)
            && collision.gameObject.TryGetComponent<ITeleport>(out var teleport))
            {
                teleport.OnTeleport(target.transform);
                return;
            }
            collision.gameObject.transform.position = target.position;
        }
    }
}