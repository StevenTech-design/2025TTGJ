using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Framework;
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
        public GameState _gameState = GameState.Gaming;

        private async void Start()
        {
            // Initialize once at game start
            await LubanManager.Instance.InitializeAsync();
            _gameState = GameState.Pause;
            // Get data anytime after initialization
            PVPanel pvPanel = UIManager.Instance.Push<PVPanel>();
            double pvTimeLength = pvPanel.GetPVTimeLength();
            Debug.Log("PVTimeLength: " + pvTimeLength);
            await UniTask.Delay(TimeSpan.FromSeconds(pvTimeLength));
            UIManager.Instance.PopUp();
            _gameState = GameState.Gaming;
        }
    }
}