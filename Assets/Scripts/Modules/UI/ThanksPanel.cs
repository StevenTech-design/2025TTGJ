using UnityEngine;
using UnityEngine.UI;
namespace TTGJ.UI
{
    public class ThanksPanel : UIPanel
    {
        [SerializeField] private Button _exitBtn;
    

        private void OnEnable()
        {
            _exitBtn.onClick.AddListener(OnExitBtnClick);
        
        }

        private void OnDisable()
        {
            _exitBtn.onClick.RemoveListener(OnExitBtnClick);
        
        }

        private void OnExitBtnClick()
        {
            UIManager.Instance.PopUp();
        }
    }
}