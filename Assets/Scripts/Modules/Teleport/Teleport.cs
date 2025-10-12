using UnityEngine;

namespace TTGJ.Teleport
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private void OnCollisionEnter(Collision collision) { 
            collision.gameObject.transform.position = target.position;
        }
    }
}