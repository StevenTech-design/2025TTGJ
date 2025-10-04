using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItemInstance
{
    public string uid;              // 唯一ID（用于区分同类型物品）
    public int itemId;              // 对应 InventoryItemData.id
    public int count = 1;           // 数量（堆叠）
    public bool isNew = true;       // 是否为新获得
}