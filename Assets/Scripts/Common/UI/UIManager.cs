using System;
using System.Collections.Generic;
using TTGJ.Framework;
namespace TTGJ.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private static Dictionary<Type, string> UIPanelPathDic = new Dictionary<Type, string>()
        {
            { typeof(UIPanel), "UI/MainPanel" },
        };
        
        
    }
}