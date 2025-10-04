using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class InventoryItemData
{
    public int id;
    public string name;
    public string description;
    public int maxStack = 99;      // 最大堆叠数
    public bool canUse = true;      // 是否可使用
    public bool canSell = true;     // 是否可出售
    public int sellPrice = 10;      // 出售价格
    public string iconPath;         // 图标路径（后续用于加载 Sprite）
}