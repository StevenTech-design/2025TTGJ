using TTGJ.Framework.Timer;
using UnityEngine;

namespace TTGJ.Plant {
    public class GrassSpawn : MonoBehaviour {
        [SerializeField] 
        private GameObject grassPrefab;
        [SerializeField]
        private float spawnInterval = 1f;

        [SerializeField]
        private Transform []spawnPoint;

        private void Start() { 
            Init();
        }

        private void Init() { 
            for(int i = 0; i < spawnPoint.Length; i++) { 
                CreateGrass(i);
            }
        }

        private void CreateGrass(int index) { 
            GameObject grass = GameObject.Instantiate(grassPrefab);
            grass.transform.SetParent(spawnPoint[index]);
            grass.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            grass.transform.localScale = Vector3.one;
            grass.GetComponent<Grass>().ChangeState(PlantState.Harvest);
            grass.GetComponent<Grass>().index = index;
            grass.GetComponent<Grass>().OnGrassHarvested += OnGrassHarvested;
        }
        private void OnGrassHarvested(int index) { 
            TimerManager.Instance.StartTimer(spawnInterval, () => { CreateGrass(index); });
        }
    }
}