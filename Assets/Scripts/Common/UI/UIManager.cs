using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Framework;
using UnityEngine;
using TTGJ.Generate;
namespace TTGJ.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private static Dictionary<Type, string> UIPanelPathDic = new Dictionary<Type, string>()
        {
            //{ typeof(UIPanel), "UI/MainPanel" },
            { typeof(ExitPanel), ResPathConfig.UI_ExitPannel},
            { typeof(JournalPanel), ResPathConfig.UI_JournalPannel},
            { typeof(SettingPanel), ResPathConfig.UI_SettingPannel},
            { typeof(ThanksPanel), ResPathConfig.UI_ThanksPannel},
        };
        private List<UIPanel> panels = new List<UIPanel>();

        public T ShowPanel<T>() where T : UIPanel{
            if(!UIPanelPathDic.TryGetValue(typeof(T), out string path)){
                return null;
            }
            if(panels.Find(p => p is T) != null) {
                return panels.Find(p => p is T) as T;
            }
            GameObject obj = ObjectPoolManager.Instance.GetGameObject(path);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            obj.SetActive(true);
            var panel = obj.GetComponent<T>();
            panels.Add(panel);
            panel.Show();
            SetCursorState();
            Debug.Log("ShowPanel: " + panel.name);
            return panel;
        }
        public void HidePanel<T>(T panel) where T : UIPanel{
            panel.Hide();
            panels.Remove(panel);
            ObjectPoolManager.Instance.ReturnGameObjectToPool(panel.gameObject);
            Debug.Log("HidePanel: " + panel.name);
            SetCursorState();
        }

        private void SetCursorState() { 
            if(panels.Count > 0) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        public int GetUICount() {
            return panels.Count;
        }
    }
}