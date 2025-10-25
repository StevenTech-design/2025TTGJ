using System.Collections;
using System.Collections.Generic;
using TMPro;
using TTGJ.Framework;
using TTGJ.Luban;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private TMP_Text itemCount;

    public void SetItem(int itemId, int count) { 
        var item = LubanManager.Instance.GetItemNew(itemId);
        var sprite = StResources.Instance.LoadByResources<Sprite>(item.Icon);
        if(sprite != null) {
            itemIcon.sprite = sprite;
        }
        itemCount.text = count.ToString();
    } 
}
