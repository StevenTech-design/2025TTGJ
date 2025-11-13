using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Framework;
// using TTGJ.GamePlay;
using TTGJ.Luban;
using TTGJ.UI;
using UnityEngine;

namespace TTGJ
{ 
    public class GameRoot : MonoBehaviour
    {
        private async void Start()
        {
            // Initialize once at game start
            await LubanManager.Instance.InitializeAsync();
            // Get data anytime after initialization
        }
    }
}