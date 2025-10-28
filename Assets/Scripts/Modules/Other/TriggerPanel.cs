using TTGJ.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTGJ.Other
{
    public enum PanelType
    {
        EnterGame,
        Journal,
        Setting,
        Thanks,
        Exit,
        StartGame,
        OperationPanel,
        TeamPanel,

    }
    public class TriggerPanel : MonoBehaviour
    {
        [SerializeField]
        private PanelType panelType;
        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer != LayerMask.NameToLayer("Player"))
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
                case PanelType.OperationPanel:
                    UIManager.Instance.Push<OperationPanel>();
                    break;
                case PanelType.TeamPanel:
                    UIManager.Instance.Push<TeamPanel>();
                    break;
                case PanelType.EnterGame:
                    SceneManager.LoadScene(1);
                    break;
                case PanelType.StartGame:
                    SceneManager.LoadScene(0);
                    break;
                default:
                    break;
            }
        }
    }
}