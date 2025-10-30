using TTGJ.Framework.Timer;
using TTGJ.GamePlay;
using TTGJ.Scene;
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
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }
            ShowPanel();
        }
        private void ShowPanel()
        {
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
                    SetPlayerPos(panelType);
                    TimerManager.Instance.DelayedCall(0.01f, () =>
                    {
                        SceneLoaderManager.Instance.LoadScene(1);
                    });
                    break;
                case PanelType.StartGame:
                    SetPlayerPos(panelType);
                    TimerManager.Instance.DelayedCall(0.01f, () =>
                    {
                        SceneLoaderManager.Instance.LoadScene(0);
                    });
                    break;
                default:
                    break;
            }
        }
        private void SetPlayerPos(PanelType panelType)
        { 
            switch (panelType)
            {
                case PanelType.EnterGame:
                    PlayerController.GetActivePlayer().transform.position = new Vector3(0.075f, 0.254f, -2.1f);
                    break;
                case PanelType.StartGame:
                    PlayerController.GetActivePlayer().transform.position = new Vector3(158.73f, 2.84f, 180);
                    break;
            }
        }
    }
}