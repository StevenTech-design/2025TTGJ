using UnityEngine;
namespace TTGJ.Plant
{
    public class Field : MonoBehaviour
    {
        private bool isPlanted = false;
        private PlantBase occupiedPlant;
        public void ToPlanting(PlantBase plant)
        {
            isPlanted = true;
            plant.transform.SetParent(transform);
            plant.transform.localRotation = Quaternion.identity;
            plant.transform.localPosition = Vector3.zero;
            plant.ChangeState(PlantState.Germination);
            occupiedPlant = plant;
            FieldSystem.Instance.AddPlant(plant, GetComponent<Cell>().cellPos);
            
        }
        public bool IsPlanted()
        {
            return isPlanted;
        }
        public void ToOccupied()
        {
            isPlanted = true;
        }
        public void Release()
        {
            isPlanted = false;
            occupiedPlant = null;
        }
        public PlantBase GetOccupiedPlant()
        {
            return occupiedPlant;
        }
    }
}