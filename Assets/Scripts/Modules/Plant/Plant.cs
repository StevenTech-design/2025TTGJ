using System.Collections;

using TTGJ.Teleport;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Plant : MonoBehaviour,ITeleport
    {
        public PlantState currentState = PlantState.Seed;
        [SerializeField]
        protected int growthTime = 10;
        [SerializeField]
        protected Collider plantCollider;
        protected int currentGrowthTime = 0;
        protected Coroutine growthCoroutine;


        private void Start() { 
            growthCoroutine = StartCoroutine(Growth());
        }


        public virtual void OnWatering() {
            ++currentGrowthTime;
            if (growthCoroutine != null && CheckGrowthFinish()) { 
                StopCoroutine(growthCoroutine);
                growthCoroutine = null;
                ChangeState(PlantState.Mature);
                return;
            }
        }
        protected virtual bool CheckGrowthFinish(){
            return currentGrowthTime >= growthTime;
        }
        private IEnumerator Growth() { 
            while (!CheckGrowthFinish()) { 
                currentGrowthTime++;
                yield return new WaitForSeconds(1);
            }
            ChangeState(PlantState.Mature);
            growthCoroutine = null;
            Debug.Log("Plant is mature");
        }
        public virtual void OnHarvest()
        {
            
        }
        public virtual void ChangeState(PlantState state) { 
            currentState = state;
            switch (state) { 
                case PlantState.Mature:
                    OnMature();
                    break;
                case PlantState.Harvest:
                    OnHarvest();
                    break;
            }
            Debug.Log("Plant state is " + state);
        }

        public virtual void OnTeleport()
        {
            
        }

        public virtual void OnMature()
        {
            
        }

    }
}