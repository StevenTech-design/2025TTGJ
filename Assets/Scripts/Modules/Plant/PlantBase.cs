using System.Collections;
using TTGJ.GamePlay;
using TTGJ.Teleport;
using UnityEngine;

namespace TTGJ.Plant
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlantBase :Liftable, ITeleport
    {
        protected PlantState currentState = PlantState.Seed;
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
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Default");
            rigidbody.isKinematic = false;
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
            collider.isTrigger = false;
            rigidbody.isKinematic = false;
            collider.excludeLayers -= 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers -= 1 << LayerMask.NameToLayer("Default");
        }

        public virtual void OnMature()
        {
            collider.enabled = true;
            collider.isTrigger = true;
            rigidbody.isKinematic = true;
        }
        public PlantState GetCurrentState()
        {
            return currentState;
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