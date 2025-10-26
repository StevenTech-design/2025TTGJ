using UnityEngine;
using UnityEngine.UI;
using TTGJ.Data;
using DG.Tweening;

namespace TTGJ.UI
{
    public class SettingPanel : UIPanel
    {
        [SerializeField]
        private Button _closeBtn;
        [SerializeField]
        private Button _confirmBtn;
        [SerializeField]
        private Button _toggleShowUIBtn;
        [SerializeField]
        private Image _valueImage;
        [SerializeField]
        private Slider _slider;
        private void OnEnable()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClick);
            _toggleShowUIBtn.onClick.AddListener(OnToggleShowUIBtnClick);
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            _confirmBtn.onClick.AddListener(OnCloseBtnClick);
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            _toggleShowUIBtn.onClick.RemoveListener(OnToggleShowUIBtnClick);
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            _confirmBtn.onClick.RemoveListener(OnCloseBtnClick);
        }

        private void OnCloseBtnClick()
        {
            UIManager.Instance.HidePanel(this);
        }

        private void Init() { 
            this._valueImage.fillAmount = DataManager.Instance.GetShowTipUIState() ? 1 : 0;
            this._slider.value = DataManager.Instance.GetMusicVolume();
        }

        public override void Show()
        {
            base.Show();
            Debug.Log("SettingPanel Show");
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void OnToggleShowUIBtnClick()
        {
            DOTween.Kill(_valueImage.fillAmount);
            bool isShow = DataManager.Instance.GetShowTipUIState();
            DataManager.Instance.SetShowTipUIState(!isShow);
            float targetValue = isShow ? 0 : 1;
            _valueImage.DOFillAmount(targetValue, 0.5f);
        }

        private void OnSliderValueChanged(float value)
        {
            DataManager.Instance.SetMusicVolume(value);
        }


    }
}