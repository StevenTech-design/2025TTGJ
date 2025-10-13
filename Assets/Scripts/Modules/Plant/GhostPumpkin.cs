using Unity.VisualScripting;
using UnityEngine;

namespace TTGJ.Plant
{
    public class GhostPumpkin : PlantBase
    {
        [SerializeField]
        private float intervalTimer = 0.5f;
        private float currentTime;
        [SerializeField]
        private float bounceForce = 10;
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
            { 
                return;
            }
            if (currentState != PlantState.Mature)
            {
                return;
            }
            currentTime = Time.time;
        }
        private void OnTriggerStay(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Building") || other.CompareTag("Field"))
            { 
                return;
            }
            if (currentState != PlantState.Mature)
            {
                return;
            }
            if (Time.time - currentTime >= intervalTimer) {
                Bounce(other.gameObject);
                Debug.Log("Bouncing pumpkin"+other.gameObject.name);
                currentTime = Time.time;
            }
        }
        private void Bounce(GameObject gameObject) { 
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * bounceForce, ForceMode.Impulse);
        }
    }
}