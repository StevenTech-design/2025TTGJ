using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private TMP_Text _currentContentText;
        [SerializeField] private Slider _progressSlider;

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
            Debug.Log("ShowItems: " + items.Count);
            for(int i = 0; i < items.Count; i++) {
                var itemObj = GetItemObject(i);
                itemObj.SetActive(true);
                int index = i;
                itemObj.GetComponent<ItemInfo>().SetItemInfo(items[i],()=>{RefreshPreviewInfo(items[index]);});
            }
            RefreshPreviewInfo(items[0]);
            for(int i = items.Count; i < _itemObjects.Count; i++) {
                Debug.Log("HideItems: " + i);
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
            var items = LubanManager.Instance.GetItemsByType(itemType);
            if (itemType == 2) { 
                _currentContentText.text = "0%";
                _progressSlider.value = 0;
            }else { 
                _currentContentText.text = "100%";
                _progressSlider.value = 1;
            }
            return items;
        }
        private void RefreshPreviewInfo(int itemId) { 
            _previewInfo.SetInfo(itemId);
        }
    }
}