using Cysharp.Threading.Tasks;
using TTGJ.Interactable;
using UnityEngine;
using DG.Tweening;
using TTGJ.Config;
using TTGJ.Luban;
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
        [SerializeField]
        protected PlantState startState = PlantState.Seed;
        protected int currentGrouthCount = 0;

        private Color originalColor;
        protected Transform modelRoot;


        public void OnWatering()
        {

            if (currentGrouthCount >= ConfigManager.Instance.GetGrowthCount(itemType))
            {
                return;
            }
            if (currentState == PlantState.Germination)
            {
                ChangeState(PlantState.Mature);
                ++currentGrouthCount;
                return;
            }

            int currentOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount);
            int willOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount + 1);

            if (currentGrouthCount == willOccupiedFieldSize)
            {
                ++currentGrouthCount;
                transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f);
            }
            else if (FieldSystem.Instance.ToOccupied(GetComponent<Cell>().cellPos, currentOccupiedFieldSize, willOccupiedFieldSize))
            {
                ++currentGrouthCount;
                transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f);
            }

            CheckEvolution();
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
            if(currentState == state) { 
                return;
            }
            currentState = state;
            switch (state)
            {
                case PlantState.Seed:
                    InitModel();
                    break;
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
            transform.localScale = ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount);
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
        private void InitModel()
        {
            Debug.Log("InitModel: " + currentState);
            if (modelRoot == null)
            {
                modelRoot = transform.GetChild(0);
            }
            DestoryPreModel();
            GameObject go = null;
            if (currentState == PlantState.Seed)
            {
                go = GameObject.Instantiate(sackGo);
            }
            else if (currentState == PlantState.Germination)
            {
                go = GameObject.Instantiate(grothGo);
            }
            else if (currentState == PlantState.Mature)
            {
                go = GameObject.Instantiate(realGo);
            }
            go.transform.SetParent(modelRoot);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            go.transform.localScale = Vector3.one;
        }
        private void DestoryPreModel()
        {
            if (modelRoot.childCount > 0)
            {
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
        private void CheckEvolution()
        {
            if (currentGrouthCount >= ConfigManager.Instance.GetGrowthCount(itemType))
            {
                itemType = (ItemType)LubanManager.Instance.GetItemNew((int)itemType).EvoId;
            }
        }
        public override void OnLift()
        {
            base.OnLift();
            
        }
    }
}