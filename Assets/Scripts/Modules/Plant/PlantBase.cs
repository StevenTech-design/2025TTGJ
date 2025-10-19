using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Plant
{
    public class PlantBase : Liftable, ITeleport, IDyeingable, IEatable
    {
        public PlantType plantType;
        [SerializeField]
        protected GameObject sackGo;
        [SerializeField]
        protected GameObject grothGo;
        [SerializeField]
        protected GameObject realGo;
        protected PlantState currentState = PlantState.Seed;
        protected int growthTime = 1; 
        [SerializeField]
        protected int [] growthScale = { 1, 2, 3};
        protected int currentGrouthCount = 0;

        private Color originalColor;
        private Transform modelRoot;

        protected void Start() {
            //originalColor = GetComponentInChildren<Renderer>().material.color;
        }

        public async virtual UniTask OnWatering()
        {
            if(currentGrouthCount >= growthScale.Length) {
                return;
            }
            //GetComponentInChildren<Renderer>().material.color = originalColor;
            Debug.Log("OnWatering");
            await UniTask.Delay(growthTime);
            Debug.Log("Watering: Finished");
            if (currentState == PlantState.Germination)
            { 
                ChangeState(PlantState.Mature);
                ++currentGrouthCount;
                return;
            }
            if (!FieldSystem.Instance.ToOccupied(GetComponent<Cell>().cellPos,
                                                growthScale[currentGrouthCount - 1],
                                                growthScale[currentGrouthCount]))
            {
                return;
            }
             ++currentGrouthCount;
            transform.localScale *= growthScale[currentGrouthCount - 1];
        }
        protected virtual void OnHarvest()
        {
            collider.isTrigger = false;
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Default");
            rigidbody.isKinematic = false;
            isLiftable = false;
            transform.SetParent(null);
        }
        public virtual void ChangeState(PlantState state)
        {
            currentState = state;
            switch (state)
            {
                case PlantState.Germination:
                    InitModel();
                    OnGermination();
                    break;
                case PlantState.Mature:
                    InitModel();
                    OnMature();
                    break;
                case PlantState.Harvest:
                    OnHarvest();
                    break;
            }
        }

        public virtual void OnTeleport(Transform baseTeleportPos)
        {
            collider.isTrigger = false;
            rigidbody.isKinematic = false;
            collider.excludeLayers -= 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers -= 1 << LayerMask.NameToLayer("Default");
            transform.localScale = Vector3.one;
            transform.position = new Vector3(transform.position.x, baseTeleportPos.position.y, transform.position.z);
            isLiftable = true;
            rigidbody.constraints = RigidbodyConstraints.None;
        }

        public virtual void OnGermination()
        {
            Initialize();
            collider.enabled = true;
            collider.isTrigger = true;
            rigidbody.isKinematic = true;
            Debug.Log("OnGermination: " + collider.isTrigger);
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
            //GetComponentInChildren<Renderer>().material.color = color;
        }
        private void InitModel() {
            Debug.Log("InitModel: " + currentState);
            if(modelRoot == null) {
                modelRoot = transform.GetChild(0);
            }
            DestoryPreModel();
            GameObject go = null;
            if (currentState == PlantState.Seed) {
                go = GameObject.Instantiate(sackGo);
            }else if (currentState == PlantState.Germination) { 
                go = GameObject.Instantiate(grothGo);
            }else if (currentState == PlantState.Mature) { 
                go = GameObject.Instantiate(realGo);
            }
            go.transform.SetParent(modelRoot);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            go.transform.localScale = Vector3.one;
        }
        private void DestoryPreModel() {
            if(modelRoot.childCount > 0) {
                Destroy(modelRoot.GetChild(0).gameObject);
            }
        }

        public virtual void OnEat()
        {
           
        }
        public override bool CheckCanLift()
        {
            return (currentState == PlantState.Harvest || currentState == PlantState.Seed) && isLiftable;
        }

        public bool CanEat()
        {
            return currentState == PlantState.Harvest;
        }
    }
}