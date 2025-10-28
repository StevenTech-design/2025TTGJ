using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTGJ.UI
{
    public class ExitPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _confirmBtn;

        private void OnEnable()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClick);
            _confirmBtn.onClick.AddListener(OnConfirmBtnClick);
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            _confirmBtn.onClick.RemoveListener(OnConfirmBtnClick);
        }

        private void OnCloseBtnClick()
        {
            UIManager.Instance.PopUp();
        }

        private void OnConfirmBtnClick()
        {
            UIManager.Instance.PopUp();
            Application.Quit();
        }
    }
}