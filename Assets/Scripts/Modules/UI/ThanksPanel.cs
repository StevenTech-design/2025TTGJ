using UnityEngine;
using UnityEngine.UI;
namespace TTGJ.UI
{
    public class ThanksPanel : UIPanel
    {
        [SerializeField] private Button _exitBtn;
        [SerializeField] private Button _confirmBtn;

        private void OnEnable()
        {
            _exitBtn.onClick.AddListener(OnExitBtnClick);
            _confirmBtn.onClick.AddListener(OnExitBtnClick);
        }

        private void OnDisable()
        {
            _exitBtn.onClick.RemoveListener(OnExitBtnClick);
            _confirmBtn.onClick.RemoveListener(OnExitBtnClick);
        }

        private void OnExitBtnClick()
        {
            UIManager.Instance.PopUp();
        }
    }
}