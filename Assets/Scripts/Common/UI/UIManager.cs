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
            { typeof(TeamPanel), ResPathConfig.UI_TeamPannel},
            { typeof(OperationPanel), ResPathConfig.UI_OperationPannel},
            
        };
        private Stack<UIPanel> _panels = new Stack<UIPanel>();
        private Dictionary<Type, UIPanel> panelDic = new Dictionary<Type, UIPanel>();
        private UIPanel _currentSecondPanel;

        public T Push<T>() where T : UIPanel
        {
            if (!UIPanelPathDic.TryGetValue(typeof(T), out string path))
            {
                return null;
            }
            if (panelDic.ContainsKey(typeof(T)))
            {
                return null;
            }
            if (_panels != null && _panels.Count > 0) {
                _panels.Peek().Pause();
            }
            GameObject obj = ObjectPoolManager.Instance.GetGameObject(path);
            var panel = InitPanel<T>(obj);
            _panels?.Push(panel);
            panelDic.Add(typeof(T), panel);
            panel.Show();
            SetCursorState();
            return panel;
        }
        public T PushSecondTip<T>()where T: UIPanel{
            if (!UIPanelPathDic.TryGetValue(typeof(T), out string path))
            {
                return null;
            }
            if (_currentSecondPanel != null) {
                _currentSecondPanel.Hide();
                ObjectPoolManager.Instance.ReturnGameObjectToPool(_currentSecondPanel.gameObject);
            }
            GameObject obj = ObjectPoolManager.Instance.GetGameObject(path);
            var panel = InitPanel<T>(obj);
            _currentSecondPanel = panel;
            _currentSecondPanel.Show();
            return panel;
        }
        public void PopUp(){
            if (_panels == null || _panels.Count == 0) {
                return;
            }
            if (_currentSecondPanel != null ) {
                _currentSecondPanel.Hide();
                ObjectPoolManager.Instance.ReturnGameObjectToPool(_currentSecondPanel.gameObject);
                _currentSecondPanel = null;
                return;
            }
            UIPanel currentPanel = _panels.Pop();
            panelDic.Remove(currentPanel.GetType());
            currentPanel.Hide();
            ObjectPoolManager.Instance.ReturnGameObjectToPool(currentPanel.gameObject);
            if (_panels.TryPeek(out UIPanel peekedPanel)) {
                peekedPanel.Resume();
            }
        }

        private void SetCursorState() { 
            if(_panels.Count > 0) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        public int GetUICount() {
            return _panels.Count;
        }
        private T InitPanel<T>(GameObject panel) where T : UIPanel{
            panel.transform.SetParent(transform);
            panel.transform.localScale = Vector3.one;
            panel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            panel.SetActive(true);
            return panel.GetComponent<T>();
        }
    }
}