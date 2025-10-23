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
        protected int currentGrouthCount = 0;

        private Color originalColor;
        protected Transform modelRoot;

        private void Start() { 
            InitModel();
        }


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
                
                // 成熟动画1
                // transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f);
                // 成熟动画2
                // transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f).SetEase(Ease.OutElastic);
                
                // 生长动画3 + 成熟动画
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOScale(1.5f, 1f)); //生长阶段
                mySequence.Append(transform.DOScale(new Vector3(2, 2, 2), 0.5f).SetEase(Ease.OutElastic)); //成熟时刻
                
              
                // 落地动画
                // transform.DOPunchScale(new Vector3(-1.0f, 0.5f,-1.0f), 0.2f);
            }
            else if (FieldSystem.Instance.ToOccupied(GetComponent<Cell>().cellPos, currentOccupiedFieldSize, willOccupiedFieldSize))
            {
                ++currentGrouthCount;
                // transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f);
                
                // 生长动画3 + 成熟动画
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOScale(1.5f, 1f)); //生长阶段
                mySequence.Append(transform.DOScale(new Vector3(2, 2, 2), 0.5f).SetEase(Ease.OutElastic)); //成熟时刻

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