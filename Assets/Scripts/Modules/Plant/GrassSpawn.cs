using TTGJ.Framework.Timer;
using TTGJ.GamePlay;
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
            Debug.Log("CreateGrass: " + index);
            GameObject grass = GameObject.Instantiate(grassPrefab);
            grass.transform.SetParent(spawnPoint[index]);
            grass.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            grass.transform.localScale = Vector3.one;
            grass.GetComponent<Grass>().currentState = PlantState.Mature;
            grass.GetComponent<Grass>().index = index;
            grass.GetComponent<Grass>().OnGrassHarvested += OnGrassHarvested;
        }
        private void OnGrassHarvested(GameObject grass,int index) { 
            TimerManager.Instance.StartTimer(spawnInterval, () => { CreateGrass(index); });
            PlayerController.Instance.ToLift(grass);
        }
    }
}