using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Framework;
using UnityEngine;
using TTGJ.Generate;
namespace TTGJ.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private static Dictionary<Type, string> UIPanelPathDic = new Dictionary<Type, string>()
        {
            //{ typeof(UIPanel), "UI/MainPanel" },
            { typeof(ExitPanel), ResPathConfig.UI_ExitPannel},
            { typeof(JournalPanel), ResPathConfig.UI_JournalPannel},
            { typeof(SettingPanel), ResPathConfig.UI_SettingPannel},
            { typeof(ThanksPanel), ResPathConfig.UI_ThanksPannel},
        };

        public async UniTask<T> ShowPanel<T>() where T : UIPanel{
            if(!UIPanelPathDic.TryGetValue(typeof(T), out string path)){
                return null;
            }
            GameObject obj = await ObjectPoolManager.Instance.GetGameObject(path);
            return obj.GetComponent<T>();
        }
        public void HidePanel<T>(T panel) where T : UIPanel{
            panel.Hide();
            ObjectPoolManager.Instance.ReturnGameObjectToPool(panel.gameObject);
        }
    }
}