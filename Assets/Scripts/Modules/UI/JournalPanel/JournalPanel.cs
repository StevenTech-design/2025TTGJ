using System.Collections.Generic;
using TTGJ.Luban;
using TTGJ.Plant;
using UnityEngine;
using UnityEngine.UI;

namespace TTGJ.UI
{
    public class JournalPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _allPlantsBtn;
        [SerializeField] private Button _allNpcBtn;
        [SerializeField] private PreviewInfo _previewInfo;
        [SerializeField] private Transform itemRoot;
        [SerializeField] private GameObject itemPrefab;
        private List<GameObject> _itemObjects = new List<GameObject>();

        private void OnEnable()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClick);
            _allPlantsBtn.onClick.AddListener(() => SwitchContent(2));
            _allNpcBtn.onClick.AddListener(() => SwitchContent(7));
            SwitchContent(2);
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            _allPlantsBtn.onClick.RemoveListener(() => SwitchContent(2));
            _allNpcBtn.onClick.RemoveListener(() => SwitchContent(7));
        }

        private void OnCloseBtnClick()
        {
            UIManager.Instance.HidePanel(this);
        }

        private void SwitchContent(int itemType) { 
             ShowItems(GetItemsBytype(itemType));
        }

        private void ShowItems(List<int> items) {
            foreach (var item in items) { 
                var itemObj = GetItemObject(item);
                itemObj.SetActive(true);
                itemObj.GetComponent<ItemInfo>().SetItemInfo(item);
            }
            for(int i = items.Count; i < _itemObjects.Count; i++) {
                _itemObjects[i].SetActive(false);
            }
        }
        private GameObject GetItemObject(int index) { 
            if(index >= _itemObjects.Count) {
                var itemObj = Instantiate(itemPrefab, itemRoot);
                _itemObjects.Add(itemObj);
                return itemObj;
            }
            return _itemObjects[index];
        }

        private List<int> GetItemsBytype(int itemType) { 
            return LubanManager.Instance.GetItemsByType(itemType);
        }
    }
}