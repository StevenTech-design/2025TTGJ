using UnityEngine;
using TTGJ.GamePlay;
using System.Collections;

namespace TTGJ.Plant
{
    public class KiwiFruitTree : PlantBase
    {
        [SerializeField]
        private GameObject kiwiFruitPrefab;
        private KiwiFruit kiwiFruit;
        [SerializeField]
        private float interval = 0.5f;
        [SerializeField]
        private Transform fruitSpawnPoint;

        private void Start() { 
            SpawnKiwiFruit();
        }

         public override bool IsLiftable() { return false; }


        private IEnumerator SpawnKiwiFruitCoroutine() { 
            yield return new WaitForSeconds(interval);
            SpawnKiwiFruit();
        }
        public override void OnWatering() { 
            if(kiwiFruit == null) { 
                return;
            }
            kiwiFruit.OnWatering();
        }
        private void SpawnKiwiFruit() { 
            kiwiFruit = Instantiate(kiwiFruitPrefab).GetComponent<KiwiFruit>();
            kiwiFruit.transform.SetParent(fruitSpawnPoint);
            kiwiFruit.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            kiwiFruit.transform.localScale = Vector3.one;
            currentState = PlantState.Mature;
        }
        protected override void OnHarvest() {
            //TODO: Harvest kiwi fruit
            if (kiwiFruit == null) {
                return;
            }
            kiwiFruit.ChangeState(PlantState.Harvest);
            kiwiFruit.transform.SetParent(null);
            kiwiFruit = null;
            StartCoroutine(SpawnKiwiFruitCoroutine());
            currentState = PlantState.Germination;
        }   

    }
}