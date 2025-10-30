using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Audio;
using TTGJ.Framework;
using TTGJ.Generate;
using TTGJ.Luban;
using TTGJ.UI;
using UnityEngine;

namespace TTGJ
{ 
    public enum GameState
    {
        Gaming,
        Pause,
    }
    public class GameRoot : MonoSingleton<GameRoot>
    {
        private async void Start()
        {
            // Initialize once at game start
            await LubanManager.Instance.InitializeAsync();
            // Get data anytime after initialization
            PVPanel pvPanel = UIManager.Instance.Push<PVPanel>();
            double pvTimeLength = pvPanel.GetPVTimeLength();
            Debug.Log("PVTimeLength: " + pvTimeLength);
            await UniTask.Delay(TimeSpan.FromSeconds(pvTimeLength));
            UIManager.Instance.PopUp();
            AudioManager.Instance.PlayBGM(ResPathConfig.BGM_BGM_island);
            
        }
    }
}