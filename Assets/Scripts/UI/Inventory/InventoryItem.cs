using cfg.Config;
using Steven.Framework;
using TMPro;
using TTGJ.Luban;
using UnityEngine;
using UnityEngine.UI;


namespace TTGJ.UI
{
    public class InventoryItem:MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text count;

        public void RefreshInfo(Inventory.InventoryItem inventoryItem)
        {
            ItemDetail itemDetail = LubanManager.Instance.GetItemInfo(inventoryItem.ItemId);
            icon.sprite = StResources.Instance.LoadByResources<Sprite>(itemDetail.Icon);
            name.text = itemDetail.Name;
            count.text = inventoryItem.Count.ToString();
        }
    }
}