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
        protected int [] growthScale = { 1, 3,3, 5, 5 };
        protected int currentGrouthCount = 0;

        private Transform modelRoot;
        private GameObject currentModel;

        protected void Start() {
            InitModel();
        }

        public async virtual UniTask OnWatering()
        {
            if(currentGrouthCount >= growthScale.Length) {
                return;
            }
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
        public virtual void OnHarvest()
        {
            collider.isTrigger = false;
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Default");
            rigidbody.isKinematic = false;
            transform.SetParent(null);
        }
        public virtual void ChangeState(PlantState state)
        {
            currentState = state;
            switch (state)
            {
                case PlantState.Germination:
                    OnGermination();
                    InitModel();
                    break;
                case PlantState.Mature:
                    OnMature();
                    InitModel();
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
        }

        public virtual void OnGermination()
        {
            Initialize();
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
        private void InitModel() {
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
            currentModel = go;
            (collider as MeshCollider).sharedMesh = go.GetComponent<MeshFilter>().sharedMesh;
            Initialize();
        }
        private void DestoryPreModel() {
            if(modelRoot.childCount > 0) {
                Destroy(modelRoot.GetChild(0).gameObject);
            }
            currentModel = null;
        }

        public virtual void OnEat()
        {
           
        }
    }
}