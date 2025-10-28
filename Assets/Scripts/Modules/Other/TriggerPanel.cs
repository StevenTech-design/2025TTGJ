namespace TTGJ.Other
{
    public enum PanelType
    {
        Journal,
        Setting,
        Thanks,
        Exit,
    }
    public class TriggerPanel : MonoBehaviour
    {
        [SerializeField]
        private PanelType panelType;
        public void OnTriggerEnter(Collider other)
        {
            if(other.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }
            ShowPanel();
        }
        private void ShowPanel() {
            switch (panelType)
            {
                case PanelType.Journal:
                    UIManager.Instance.Push<JournalPanel>();
                    break;
                case PanelType.Setting:
                    UIManager.Instance.Push<SettingPanel>();
                    break;
                case PanelType.Thanks:
                    UIManager.Instance.Push<ThanksPanel>();
                    break;
                case PanelType.Exit:
                    UIManager.Instance.Push<ExitPanel>();
                    break;
                default:
                    break;
            }
        }
    }
}