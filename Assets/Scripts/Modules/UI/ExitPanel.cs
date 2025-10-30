using System.Collections;
using System.Collections.Generic;
using TTGJ.Audio;
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
            AudioManager.Instance.PlaySFX(Audio.AudioType.Button_Click);
            UIManager.Instance.PopUp();
        }

        private void OnConfirmBtnClick()
        {
            AudioManager.Instance.PlaySFX(Audio.AudioType.Button_Click);
            UIManager.Instance.PopUp();
            Application.Quit();
        }
    }
}