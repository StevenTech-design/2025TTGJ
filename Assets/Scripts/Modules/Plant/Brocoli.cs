using UnityEngine;

namespace TTGJ.Plant
{
    public class Brocoli : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Brocoli: OnTriggerEnter Play audio");
        }
    }
}