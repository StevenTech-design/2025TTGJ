using Cysharp.Threading.Tasks;
using TTGJ.Interactable;
using UnityEngine;
using DG.Tweening;
namespace TTGJ.Plant
{
    public class PlantBase : Liftable, ITeleport, IDyeingable, IEatable
    {
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
        protected Transform modelRoot;

        protected void Start()
        {
            
        }

        public void OnWatering()
        {
            if(currentGrouthCount >= growthScale.Length) {
                return;
            }
            Debug.Log("Watering");
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
            transform.DOScale(growthScale[currentGrouthCount - 1],0.5f);
        }
        protected virtual void OnHarvest()
        {
            collider.isTrigger = true;
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Building");
            collider.excludeLayers += 1 << LayerMask.NameToLayer("Default");
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            isLiftable = false;
            transform.SetParent(null);
            FieldSystem.Instance.RemovePlant(this);
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