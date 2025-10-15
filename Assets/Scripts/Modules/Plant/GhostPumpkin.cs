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
                SpecialAction(new PlantSpacialParam<GameObject> { param = other.gameObject });
                currentTime = Time.time;
            }
        }
        public override void SpecialAction(PlantSpacialParam param) { 
            if (!(param is PlantSpacialParam<GameObject> plantSpacialParam))
            {
                return;
            }
            plantSpacialParam.param.GetComponent<Rigidbody>().AddForce(Vector3.left * bounceForce, ForceMode.Impulse);
        }
    }
}