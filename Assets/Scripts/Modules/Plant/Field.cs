using UnityEngine;
namespace TTGJ.Plant
{
    public class Field : MonoBehaviour
    {
        private bool isPlanted = false;

        public void ToToPlanting(PlantBase plant)
        {
            isPlanted = true;
            plant.transform.SetParent(transform);
            plant.transform.localRotation = Quaternion.identity;
            plant.transform.localPosition = Vector3.zero;
            plant.ChangeState(PlantState.Germination);
        }
        public bool IsPlanted()
        {
            return isPlanted;
        }
        public void OnHarvest()
        {
            isPlanted = false;
        }
    }
}