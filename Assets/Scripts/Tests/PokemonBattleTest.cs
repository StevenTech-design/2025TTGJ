// Assets/Scripts/Tests/PokemonBattleTest.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTGJ.Battle;
using TTGJ.Luban;
using Cysharp.Threading.Tasks;

namespace TTGJ.Tests
{
    /// <summary>
    /// Pokemon 1v1 对战测试类
    /// 用于测试现有的Battle模块功能
    /// </summary>
    public class PokemonBattleTest : MonoBehaviour
    {
        [Header("测试配置")]
        [Tooltip("玩家Pokemon的ID")]
        public int playerPokemonId = 1;
        
        [Tooltip("玩家Pokemon的技能ID列表（最多4个）")]
        public List<int> playerMoveIds = new List<int> { 1, 2, 3, 4 };
        
        [Tooltip("敌方Pokemon的ID")]
        public int enemyPokemonId = 2;
        
        [Tooltip("敌方Pokemon的技能ID列表（最多4个）")]
        public List<int> enemyMoveIds = new List<int> { 1, 2, 3 };
        
        [Tooltip("敌方Pokemon的AI等级")]
        public int enemyLevel = 5;
        
        [Tooltip("是否自动战斗（AI vs AI）")]
        public bool isAutoBattle = false;
        
        [Tooltip("自动战斗时的回合间隔（秒）")]
        public float autoBattleTurnDelay = 1.5f;

        [Header("运行时信息")]
        [SerializeField] private string currentTurn = "";
        [SerializeField] private string playerStatus = "";
        [SerializeField] private string enemyStatus = "";
        [SerializeField] private bool battleEnded = false;

        // 战斗管理
        private PokemonEntity playerPokemon;
        private AIPokemonEntity enemyPokemon;
        private PokemonBattleManager battleManager;
        private bool isInitialized = false;

        private async void Start()
        {
            await InitializeTest();
        }

        /// <summary>
        /// 初始化测试
        /// </summary>
        private async UniTask InitializeTest()
        {
            Debug.Log("=== Pokemon 对战测试开始 ===");
            
            // 确保 LubanManager 已初始化
            if (!LubanManager.Instance.IsInitialized)
            {
                Debug.Log("正在初始化 LubanManager...");
                await LubanManager.Instance.InitializeAsync();
            }
            
            // 创建玩家Pokemon
            Debug.Log($"创建玩家 Pokemon (ID: {playerPokemonId})");
            playerPokemon = PokemonEntity.CreateFromConfig(playerPokemonId);
            playerPokemon.SetMovesByIds(playerMoveIds);
            
            // 创建敌方AI Pokemon
            Debug.Log($"创建敌方 AI Pokemon (ID: {enemyPokemonId})");
            enemyPokemon = PokemonEntity.CreateFromConfig<AIPokemonEntity>(enemyPokemonId);
            enemyPokemon.SetMovesByIds(enemyMoveIds);
            enemyPokemon.lv = enemyLevel;
            
            // 注册AI回调
            enemyPokemon.ToUseMove = OnEnemyUseMove;
            
            // 创建战斗管理器
            List<PokemonEntity> enemyList = new List<PokemonEntity> { enemyPokemon };
            battleManager = new PokemonBattleManager(playerPokemon, enemyList);
            
            // 开始战斗
            battleManager.StartBattle();
            isInitialized = true;
            
            Debug.Log($"=== 战斗开始: {playerPokemon.Name} VS {enemyPokemon.Name} ===");
            PrintPokemonInfo(playerPokemon, "玩家");
            PrintPokemonInfo(enemyPokemon, "敌方");
            
            UpdateStatusDisplay();
            
            // 如果是自动战斗模式，启动自动战斗协程
            if (isAutoBattle)
            {
                StartCoroutine(AutoBattleCoroutine());
            }
            else
            {
                // 否则，如果是敌方回合，让敌方行动
                if (!battleManager.IsSelfTurn)
                {
                    StartCoroutine(EnemyTurnCoroutine());
                }
                else
                {
                    PrintPlayerTurnHint();
                }
            }
        }

        /// <summary>
        /// 打印Pokemon信息
        /// </summary>
        private void PrintPokemonInfo(PokemonEntity pokemon, string side)
        {
            Debug.Log($"【{side}】{pokemon.Name}");
            Debug.Log($"  HP: {pokemon.HP}/{pokemon.MaxHP}");
            Debug.Log($"  攻击: {pokemon.BaseAttack} | 防御: {pokemon.BaseDefense} | 速度: {pokemon.BaseSpeed}");
            Debug.Log($"  技能列表:");
            foreach (var move in pokemon.Moves)
            {
                Debug.Log($"    - {move.Name} (威力:{move.Power}, 消耗:{move.Cost}AP)");
            }
        }

        /// <summary>
        /// 更新状态显示（用于Inspector查看）
        /// </summary>
        private void UpdateStatusDisplay()
        {
            if (!isInitialized) return;
            
            currentTurn = battleManager.IsSelfTurn ? "玩家回合" : "敌方回合";
            playerStatus = $"{playerPokemon.Name} | HP:{playerPokemon.HP}/{playerPokemon.MaxHP} | AP:{playerPokemon.ActionPoint} | 护盾:{playerPokemon.Shield}";
            enemyStatus = $"{enemyPokemon.Name} | HP:{enemyPokemon.HP}/{enemyPokemon.MaxHP} | AP:{enemyPokemon.ActionPoint} | 护盾:{enemyPokemon.Shield}";
            
            if (!playerPokemon.IsAlive || !enemyPokemon.IsAlive)
            {
                battleEnded = true;
            }
        }

        /// <summary>
        /// 敌方使用技能回调
        /// </summary>
        private void OnEnemyUseMove(PokemonEntity aiPokemon, cfg.move move)
        {
            battleManager.UsePokemonSkill(aiPokemon, move);
            UpdateStatusDisplay();
        }

        /// <summary>
        /// 敌方回合协程
        /// </summary>
        private IEnumerator EnemyTurnCoroutine()
        {
            Debug.Log("--- 敌方回合开始 ---");
            
            // 等待一小段时间
            yield return new WaitForSeconds(0.5f);
            
            // AI自动选择并使用技能，直到行动点用完或回合结束
            while (!battleManager.IsSelfTurn && !battleEnded)
            {
                enemyPokemon.TickLogic();
                UpdateStatusDisplay();
                
                // 检查战斗是否结束
                if (CheckBattleEnd()) yield break;
                
                yield return new WaitForSeconds(0.8f);
            }
            
            if (!battleEnded)
            {
                Debug.Log("--- 敌方回合结束 ---");
                Debug.Log("--- 玩家回合开始 ---");
                UpdateStatusDisplay();
                
                if (!isAutoBattle)
                {
                    PrintPlayerTurnHint();
                }
            }
        }

        /// <summary>
        /// 自动战斗协程（AI vs AI）
        /// </summary>
        private IEnumerator AutoBattleCoroutine()
        {
            Debug.Log("【自动战斗模式】");
            
            while (!battleEnded)
            {
                if (battleManager.IsSelfTurn)
                {
                    // 玩家回合 - 使用第一个可用的技能
                    Debug.Log("--- 玩家回合（自动） ---");
                    yield return new WaitForSeconds(autoBattleTurnDelay);

                    // 找到第一个能使用的技能
                    foreach (var move in playerPokemon.Moves)
                    {
                        if (playerPokemon.CanPay(move.Cost))
                        {
                            UsePlayerSkill(move);
                            yield return new WaitForSeconds(0.8f);
                            break;
                        }
                    }

                    // 如果没有技能可用，跳过回合
                    if (playerPokemon.ActionPoint > 0)
                    {
                        battleManager.CheckActionEnd();
                    }
                }
                else
                {
                    // 敌方回合
                    Debug.Log("--- 敌方回合（自动） ---");
                    yield return new WaitForSeconds(autoBattleTurnDelay);

                    enemyPokemon.TickLogic();
                    UpdateStatusDisplay();

                    if (CheckBattleEnd()) yield break;

                    yield return new WaitForSeconds(0.8f);
                }
                
                UpdateStatusDisplay();
            }
        }

        /// <summary>
        /// 打印玩家回合提示
        /// </summary>
        private void PrintPlayerTurnHint()
        {
            Debug.Log("━━━━━━━━━━━━━━━━━━━━");
            Debug.Log("【你的回合】可用技能:");
            for (int i = 0; i < playerPokemon.Moves.Count; i++)
            {
                var move = playerPokemon.Moves[i];
                bool canUse = playerPokemon.CanPay(move.Cost);
                string status = canUse ? "✓" : "✗";
                Debug.Log($"  [{i}] {status} {move.Name} (威力:{move.Power}, 消耗:{move.Cost}AP)");
            }
            Debug.Log($"当前行动点: {playerPokemon.ActionPoint}");
            Debug.Log("使用技能: 在Inspector中调用 UsePlayerSkillByIndex(技能索引)");
            Debug.Log("或直接调用: TestUseSkill0/1/2/3()");
            Debug.Log("━━━━━━━━━━━━━━━━━━━━");
        }

        /// <summary>
        /// 使用玩家技能
        /// </summary>
        public void UsePlayerSkillByIndex(int skillIndex)
        {
            if (battleEnded)
            {
                Debug.LogWarning("战斗已结束！");
                return;
            }
            
            if (!battleManager.IsSelfTurn)
            {
                Debug.LogWarning("❌ 不是你的回合！");
                return;
            }
            
            if (skillIndex < 0 || skillIndex >= playerPokemon.Moves.Count)
            {
                Debug.LogWarning($"❌ 无效的技能索引: {skillIndex}");
                return;
            }
            
            var move = playerPokemon.Moves[skillIndex];
            UsePlayerSkill(move);
        }

        /// <summary>
        /// 使用玩家技能（内部方法）
        /// </summary>
        private void UsePlayerSkill(cfg.move move)
        {
            if (!playerPokemon.CanPay(move.Cost))
            {
                Debug.LogWarning($"❌ 行动点不足！需要 {move.Cost} AP");
                return;
            }
            
            battleManager.UsePokemonSkill(playerPokemon, move);
            UpdateStatusDisplay();
            
            // 检查战斗是否结束
            if (CheckBattleEnd()) return;
            
            // 如果是敌方回合，自动执行敌方行动
            if (!battleManager.IsSelfTurn && !isAutoBattle)
            {
                StartCoroutine(EnemyTurnCoroutine());
            }
            else if (battleManager.IsSelfTurn)
            {
                PrintPlayerTurnHint();
            }
        }

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        private bool CheckBattleEnd()
        {
            if (battleEnded) return true;
            
            if (!playerPokemon.IsAlive)
            {
                Debug.Log("━━━━━━━━━━━━━━━━━━━━");
                Debug.Log("💀 战斗结束：你输了！");
                Debug.Log($"{enemyPokemon.Name} 获胜！");
                Debug.Log("━━━━━━━━━━━━━━━━━━━━");
                battleEnded = true;
                return true;
            }
            
            if (!enemyPokemon.IsAlive)
            {
                Debug.Log("━━━━━━━━━━━━━━━━━━━━");
                Debug.Log("🎉 战斗结束：你赢了！");
                Debug.Log($"{playerPokemon.Name} 获胜！");
                Debug.Log("━━━━━━━━━━━━━━━━━━━━");
                battleEnded = true;
                return true;
            }
            
            return false;
        }

        // ============ 快捷测试方法（可在Inspector的Context Menu中调用）============
        
        [ContextMenu("测试 - 使用技能0")]
        public void TestUseSkill0() => UsePlayerSkillByIndex(0);
        
        [ContextMenu("测试 - 使用技能1")]
        public void TestUseSkill1() => UsePlayerSkillByIndex(1);
        
        [ContextMenu("测试 - 使用技能2")]
        public void TestUseSkill2() => UsePlayerSkillByIndex(2);
        
        [ContextMenu("测试 - 使用技能3")]
        public void TestUseSkill3() => UsePlayerSkillByIndex(3);
        
        [ContextMenu("测试 - 查看当前状态")]
        public void PrintCurrentStatus()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("战斗尚未初始化");
                return;
            }
            
            Debug.Log("━━━━━━━━━━━━━━━━━━━━");
            Debug.Log($"【当前回合】{currentTurn}");
            Debug.Log($"【玩家】{playerStatus}");
            Debug.Log($"【敌方】{enemyStatus}");
            Debug.Log("━━━━━━━━━━━━━━━━━━━━");
        }
        
        [ContextMenu("测试 - 重新开始战斗")]
        public async void RestartBattle()
        {
            battleEnded = false;
            StopAllCoroutines();
            await InitializeTest();
        }
    }
}