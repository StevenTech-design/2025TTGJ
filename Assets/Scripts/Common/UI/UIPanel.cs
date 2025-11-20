using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TTGJ.UI
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] protected Button closeBtn;
        [SerializeField] protected CanvasGroup canvasGroup;
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
            canvasGroup.blocksRaycasts = false;
            OnPause?.Invoke();
        }
        public virtual void Resume() {
            gameObject.SetActive((true));
            canvasGroup.blocksRaycasts = true;
            OnResume?.Invoke();
        }
        public virtual void Hide() {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }

        protected virtual void OnClickClose()
        {
            UIManager.Instance.PopUp();
        }
    }
}