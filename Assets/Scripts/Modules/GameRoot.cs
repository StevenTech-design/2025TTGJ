using System;
using System.Collections;
using System.Collections.Generic;
using TTGJ.Luban;
using TTGJ.UI;
using UnityEngine;

namespace TTGJ
{ 
    public class GameRoot : MonoBehaviour
    {
        private async void Start()
        {
            await LubanManager.Instance.InitializeAsync();
        }
    }
}