using UnityEngine;

namespace TTGJ.Item
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private void OnCollisionEnter(Collision collision)
        {
            Ray upRay = new Ray(collision.gameObject.transform.position, Vector3.up);
            if (Physics.Raycast(upRay, out RaycastHit hit, 100, LayerMask.GetMask("Building"))
            && collision.gameObject.TryGetComponent<ITeleport>(out var teleport))
            {
                teleport.OnTeleport();
                return;
            }
            collision.gameObject.transform.position = target.position;
        }
    }
}