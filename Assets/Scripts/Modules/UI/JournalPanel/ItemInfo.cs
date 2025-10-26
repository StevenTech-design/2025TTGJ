using System.Collections;
using System.Collections.Generic;
using TMPro;
using TTGJ.Framework;
using TTGJ.Luban;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TTGJ.UI
{
    public class ItemInfo : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private Button _itemBtn;
        private Action _onClick;

        private void OnEnable()
        {
            _itemBtn.onClick.AddListener(OnItemBtnClick);
        }
        private void OnDisable()
        {
            _itemBtn.onClick.RemoveListener(OnItemBtnClick);
        }
        

        public void SetItemInfo(int itemId, Action onClick)
        {
            var item = LubanManager.Instance.GetItemNew(itemId);
            var sprite = StResources.Instance.LoadByResources<Sprite>(item.Icon);
            if (sprite != null)
            {
                _itemIcon.sprite = sprite;
            }
            _itemName.text = item.Name;
            _onClick = onClick;
        }
        private void OnItemBtnClick()
        {
            _onClick?.Invoke();
        }
    }
}
