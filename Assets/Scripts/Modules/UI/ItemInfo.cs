using System.Collections;
using System.Collections.Generic;
using TMPro;
using TTGJ.Framework;
using TTGJ.Luban;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _itemName;

    public void SetItemInfo(int itemId) { 
        var item = LubanManager.Instance.GetItemNew(itemId);
        var sprite = StResources.Instance.LoadByResources<Sprite>(item.Icon);
        if(sprite != null) {
            _itemIcon.sprite = sprite;
        }
        _itemName.text = item.Name;
    }
}
