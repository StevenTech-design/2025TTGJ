using System;
using TMPro;
using TTGJ.Common;
using UnityEngine;
using EventType = TTGJ.Common.EventType;

namespace TTGJ.Task {
    public class SheepNPC : NPCBase
    {
        [SerializeField]
        private TMP_FontAsset specialFont;
        [SerializeField]
        private TMP_FontAsset normalFont;

        private void OnEnable() { 
            EventManager.Instance.RegisterEvent(EventType.ChangeSheepTalkState, OnChangeSheepTalkStateHandler);
        }

        private void OnChangeSheepTalkStateHandler(EventParam param)
        {
            var eventParam = param as EventParam<bool>;
            var font = eventParam.param ? specialFont : normalFont;
            plot.UpdateFont(font);
        }

        private void OnDisable() { 
            EventManager.Instance.UnregisterEvent(EventType.ChangeSheepTalkState, OnChangeSheepTalkStateHandler);
        }
    }
}