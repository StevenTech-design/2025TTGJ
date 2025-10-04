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
            var pokemon = LubanManager.Instance.GetPokemon(1);
            var move = LubanManager.Instance.GetMove(1);
            var item = LubanManager.Instance.GetItem(1);
            Debug.Log(pokemon);
            Debug.Log(move);
            Debug.Log(item);
        }
    }
}