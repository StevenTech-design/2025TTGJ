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
    private int maxCount;

    public void SetItem(int itemId, int count) { 
        maxCount = count;
        var item = LubanManager.Instance.GetItemNew(itemId);
        var sprite = StResources.Instance.LoadByResources<Sprite>(item.Icon);
        if(sprite != null) {
            itemIcon.sprite = sprite;
        }
        itemCount.text = count.ToString();
        Debug.Log($"itemId:{itemId} count:{count}");
    }

    public void UpdateCount(int currentCount)
    {
        itemCount.text = (maxCount - currentCount).ToString();
        Debug.Log($"itemId:{currentCount} count:{currentCount} maxCount:{maxCount}");
    }
}
