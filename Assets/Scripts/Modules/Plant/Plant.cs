using System.Collections;
using TTGJ.GamePlay;
using TTGJ.Teleport;
using UnityEngine;

namespace TTGJ.Plant
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Plant :Liftable, ITeleport
    {
        public PlantState currentState = PlantState.Seed;
        [SerializeField]
        protected int growthTime = 10;
        protected int currentGrowthTime = 0;
        protected Coroutine growthCoroutine;
        public virtual void OnWatering()
        {
            ++currentGrowthTime;
            if (growthCoroutine != null && CheckGrowthFinish())
            {
                StopCoroutine(growthCoroutine);
                growthCoroutine = null;
                ChangeState(PlantState.Mature);
                return;
            }
        }
        protected virtual bool CheckGrowthFinish()
        {
            return currentGrowthTime >= growthTime;
        }
        private IEnumerator Growth()
        {
            while (!CheckGrowthFinish())
            {
                currentGrowthTime++;
                yield return new WaitForSeconds(1);
            }
            ChangeState(PlantState.Mature);
            growthCoroutine = null;
        }
        public virtual void OnHarvest()
        {

        }
        public virtual void ChangeState(PlantState state)
        {
            currentState = state;
            switch (state)
            {
                case PlantState.Germination:
                    growthCoroutine = StartCoroutine(Growth());
                    break;
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
            collider.isTrigger = true;
            rigidbody.isKinematic = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeState(PlantState.Mature);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeState(PlantState.Harvest);
            }
        }
    }
}