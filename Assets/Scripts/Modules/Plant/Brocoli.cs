using UnityEngine;

namespace TTGJ.Plant
{
    public class Brocoli : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            Debug.Log("Brocoli: OnTriggerEnter Play audio");
        }
    }
}