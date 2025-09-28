using UnityEngine;

namespace TTGJ.UI
{
    public class UIPanel : MonoBehaviour
    {
        public virtual void Show() { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void Hide() { }
    }
}