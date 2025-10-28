using System;
using Unity.VisualScripting;
using UnityEngine;

namespace TTGJ.UI
{
    public class UIPanel : MonoBehaviour {
        public Action OnShow;
        public Action OnHide;
        public Action OnPause;
        public Action OnResume;
        public virtual void Show() {
            gameObject.SetActive(true);
            OnShow?.Invoke();
        }
        public virtual void Pause() {
            gameObject.SetActive(false);
            OnPause?.Invoke();
        }
        public virtual void Resume() {
            gameObject.SetActive((true));
            OnResume?.Invoke();
        }
        public virtual void Hide() {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }
    }
}