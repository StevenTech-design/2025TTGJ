using System;

namespace TTGJ.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        /// <summary>
        /// 物品配置ID（对应Luban配置表中的ItemDetail.Id）
        /// </summary>
        public int ItemId;
        
        /// <summary>
        /// 物品数量
        /// </summary>
        public int Count;

        public int Index;


        public InventoryItem()
        {
            ItemId = 0;
            Count = 0;
            Index = 0;
        }

        public InventoryItem(int itemId, int count, int index)
        {
            ItemId = itemId;
            Count = count;
            Index = index;
        }

        /// <summary>
        /// 创建物品的深拷贝
        /// </summary>
        public InventoryItem Clone()
        {
            return new InventoryItem(ItemId, Count, Index);
        }

        public override string ToString()
        {
            return $"[InventoryItem] ItemId:{ItemId}, Count:{Count}, index:{Index}";
        }
    }
}

