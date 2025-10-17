using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TTGJ.GamePlay;
using TTGJ.Item;
using UnityEngine;

namespace TTGJ.Plant
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlantBase : Liftable, ITeleport, IDyeingable
    {
        protected PlantState currentState = PlantState.Seed;
        [Tooltip("unit: millisecond")]
        [SerializeField]
        protected int growthTime = 1000; 
        [SerializeField]
        protected int [] growthScale = { 1, 3,3, 5, 5 };
        protected int currentGrouthCount = 0;
        public async virtual void OnWatering()
        {
            if(currentGrouthCount >= growthScale.Length) {
                return;
            }
            if (!FieldSystem.Instance.ToOccupied(GetComponent<Cell>().cellPos,
                                                growthScale[currentGrouthCount - 1],
                                                growthScale[currentGrouthCount - 2]))
            {
                return;
            }
            Debug.Log("OnWatering");
            await UniTask.Delay(growthTime);
            Debug.Log("Watering: Finished");
            ++currentGrouthCount;
            transform.localScale *= growthScale[currentGrouthCount - 1];
            if (currentState == PlantState.Germination)
            { 
                ChangeState(PlantState.Mature);
            }
        }
        public virtual void OnHarvest()
        {
            collider.isTrigger = false;
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
                    OnGermination();
                    break;
                case PlantState.Mature:
                    OnMature();
                    break;
                case PlantState.Harvest:
                    OnHarvest();
                    break;
            }
        }

        public virtual void OnTeleport()
        {
            collider.isTrigger = false;
            rigidbody.isKinematic = false;
            collider.excludeLayers -= 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers -= 1 << LayerMask.NameToLayer("Default");
            transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.up * 5;
            transform.localScale = Vector3.one;
        }

        public virtual void OnGermination()
        {
            collider.enabled = true;
            collider.isTrigger = false;
            rigidbody.isKinematic = true;
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

        public virtual void SpecialAction(PlantSpacialParam param)
        { 
            
        }

        public void Dyeing(Color color)
        {
            Debug.Log("Dyeing: " + color);
        }
    }
}