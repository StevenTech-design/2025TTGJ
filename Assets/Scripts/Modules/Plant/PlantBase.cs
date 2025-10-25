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
        public PlantState currentState = PlantState.Seed;
        protected int currentGrouthCount = 0;

        private Color originalColor;
        protected Transform modelRoot;


        public virtual void OnWatering()
        {
            if (currentGrouthCount >= ConfigManager.Instance.GetGrowthCount(itemType))
            {
                return;
            }
            if (currentState == PlantState.Germination)
            {
                ChangeState(PlantState.Mature);
            }
            

            int currentOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount);
            int willOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount + 1);
            
            Debug.Log("OnWatering: " + currentOccupiedFieldSize + " " + willOccupiedFieldSize + " " + currentGrouthCount);
            if (currentOccupiedFieldSize == willOccupiedFieldSize)
            {
                ++currentGrouthCount;
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f));
                mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f).SetEase(Ease.OutElastic));
                
            }
            else if (FieldSystem.Instance.ToOccupied(GetComponent<Cell>().cellPos, currentOccupiedFieldSize, willOccupiedFieldSize))
            {
                ++currentGrouthCount;
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f));
                mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f).SetEase(Ease.OutElastic));
            }
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
            transform.localScale = Vector3.one * ConfigManager.Instance.GetHaverstSize(itemType, currentGrouthCount);;
            transform.position = new Vector3(transform.position.x, baseTeleportPos.position.y, transform.position.z);
            isLiftable = true;
            rigidbody.constraints = RigidbodyConstraints.None;
            CheckEvolution();


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
        public void InitModel()
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
            else if (currentState >= PlantState.Mature)
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
        public virtual void OnPlant() { 
           var itemNew = LubanManager.Instance.GetItemNew((int)itemType);
           if (itemNew != null)
           {
                itemType = (ItemType)itemNew.EvoId;
           }
        }
    }
}