using System.Collections;
using System.Collections.Generic;
using TTGJ.Framework;
using TTGJ.Luban;
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