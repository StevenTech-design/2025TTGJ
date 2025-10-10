using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Battle
{
    public class PokemonBattleManager
    {
        private PokemonEntity _selfPokemon;
        private List<PokemonEntity> _allPokemonList;
        private int defaultActionPoint = 1;
        private PokemonEntity currentPokemon;
        private int currentPokemonIndex = -1;

        public PokemonBattleManager(PokemonEntity selfPokemon, List<PokemonEntity> enemyPokemonList)
        {
            _selfPokemon = selfPokemon;
            _allPokemonList = enemyPokemonList;
            _allPokemonList.Add(selfPokemon);
            _allPokemonList.Sort((a, b) => a.BaseSpeed.CompareTo(b.BaseSpeed));
        }

        // 新增：开局，确定第一位行动者
        public void StartBattle()
        {
            if (_allPokemonList == null || _allPokemonList.Count == 0)
            {
                Debug.LogError("PokemonBattleManager: 战斗列表为空，无法开始");
                return;
            }
            currentPokemonIndex = -1;
            FindNextPokemon();
        }

        public bool IsSelfTurn => currentPokemon == _selfPokemon;

        // 新增：获取当前行动的宝可梦
        public PokemonEntity GetCurrentPokemon()
        {
            return currentPokemon;
        }
        private bool CheckBattleEnd()
        {
            if (!_selfPokemon.IsAlive)
            {
                Debug.Log("战斗结束，我方失败");
                return true;
            }
            foreach (var pokemon in _allPokemonList)
            {
                if (pokemon != _selfPokemon && pokemon.IsAlive)
                {
                    return false;
                }
            }
            Debug.Log("战斗结束，我方胜利");
            return true;
        }
        private void FindNextPokemon()
        {
            currentPokemonIndex = (currentPokemonIndex + 1) % _allPokemonList.Count;
            currentPokemon = _allPokemonList[currentPokemonIndex];
            currentPokemon.ResetActionPoint(defaultActionPoint);
            if (currentPokemon == _selfPokemon)
            {
                Debug.Log("Player's turn");
            }
            else
            {
                Debug.Log("Enemy's turn");
            }
        }
        public PokemonEntity GetSelfPokemon()
        {
            return _selfPokemon;
        }
        public PokemonEntity GetEnemyPokemon()
        {
            foreach (var pokemon in _allPokemonList)
            {
                if (pokemon != _selfPokemon && pokemon.IsAlive)
                {
                    return pokemon;
                }
            }
            return null;
        }
        public void UsePokemonSkill(PokemonEntity pokemon, cfg.move skill)
        {
            if (pokemon != currentPokemon)
            {
                Debug.LogError("No you turn");
                return;
            }
            PokemonEntity enemyPokemon = pokemon == _selfPokemon ? GetEnemyPokemon() : _selfPokemon;
            bool result = pokemon.UseMove(skill, enemyPokemon, out string log);
            if (!result)
            {
                Debug.LogError(log);
                return;
            }
            // 新增：成功释放技能时输出战斗日志
            Debug.Log(log);
            CheckActionEnd();
        }
        public void CheckActionEnd()
        {
            if (CheckBattleEnd())
            {
                return;
            }
            if (currentPokemon.ActionPoint <= 0 || currentPokemon != _selfPokemon)
            {
                FindNextPokemon();
            }
        }
        
        private void Update() {
            //if(Input.GetKeyDown(KeyCode)) {
        }
    }
}
