using System.Collections;
using UnityEngine;
namespace TTGJ.Plant
{
    public class Field : MonoBehaviour
    {
        [SerializeField]
        private float wetInterval = 0.5f;
        private float currentWetTime = 0f;
        private Coroutine wetCoroutine;
        [SerializeField]
        private Material wetMaterial;
        private Material originalMaterial;
        private bool isPlanted = false;

        private PlantBase occupiedPlant;

        private void Start()
        {
            originalMaterial = GetComponent<Renderer>().material;
        }   
        public void ToPlanting(PlantBase plant)
        {
            isPlanted = true;
            plant.transform.SetParent(transform);
            plant.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
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
        public void ToWet()
        {
            currentWetTime = 0f;
            GetComponent<Renderer>().material = wetMaterial;
            if(wetCoroutine != null) { 
                StopCoroutine(wetCoroutine);
            }
            wetCoroutine = StartCoroutine(ToWetCoroutine());
        }
        private IEnumerator ToWetCoroutine()
        {
            while (currentWetTime < wetInterval)
            {
                currentWetTime += Time.deltaTime;
                yield return null;
            }
            GetComponent<Renderer>().material = originalMaterial;
            wetCoroutine = null;
        }
    }
}