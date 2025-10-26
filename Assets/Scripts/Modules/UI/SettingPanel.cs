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
        private Slider _musicVolume;
        [SerializeField]
        private Slider _fieldOfViewSlider;
        [SerializeField]
        [Range(45, 90)]
        private int fieldOfView = 60;
        private void OnEnable()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClick);
            _toggleShowUIBtn.onClick.AddListener(OnToggleShowUIBtnClick);
            _musicVolume.onValueChanged.AddListener(OnSliderMusicVolumeValueChanged);
            _confirmBtn.onClick.AddListener(OnCloseBtnClick);
            _fieldOfViewSlider.onValueChanged.AddListener(OnSliderFieldOfViewValueChanged);
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            _toggleShowUIBtn.onClick.RemoveListener(OnToggleShowUIBtnClick);
            _musicVolume.onValueChanged.RemoveListener(OnSliderMusicVolumeValueChanged);
            _confirmBtn.onClick.RemoveListener(OnCloseBtnClick);
            _fieldOfViewSlider.onValueChanged.RemoveListener(OnSliderFieldOfViewValueChanged);
        }

        private void OnCloseBtnClick()
        {
            UIManager.Instance.HidePanel(this);
        }

        private void Init() { 
            this._valueImage.fillAmount = DataManager.Instance.GetShowTipUIState() ? 1 : 0;
            this._musicVolume.value = DataManager.Instance.GetMusicVolume();
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

        private void OnSliderMusicVolumeValueChanged(float value)
        {
            DataManager.Instance.SetMusicVolume(value);
        }

        private void OnSliderFieldOfViewValueChanged(float value)
        {
            int fov = (int)Mathf.Lerp(60, 90, value);
            Camera.main.fieldOfView = fov;
            Debug.Log("FieldOfView: " + fov);
            DataManager.Instance.SetFieldOfView(fov);
        }
    }
}