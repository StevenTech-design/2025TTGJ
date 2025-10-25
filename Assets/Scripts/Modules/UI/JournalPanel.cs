using UnityEngine;
using UnityEngine.UI;

namespace TTGJ.UI
{
    public class JournalPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _allPlantsBtn;
        [SerializeField] private Button _allNpcBtn;
        [SerializeField] private Button _collectionBtn;
        [SerializeField] private Button _receipBtn;


        private void OnEnable()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClick);
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(OnCloseBtnClick);
        }

        private void OnCloseBtnClick()
        {
            UIManager.Instance.HidePanel(this);
        }
    }
}