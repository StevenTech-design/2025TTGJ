using System;
using System.Collections.Generic;
using UnityEngine;
using TTGJ.Luban;

namespace TTGJ.Battle
{
	// 招式目标标记（与表里 move_Target 对应）
	public enum MoveTarget
	{
		Enemy = 1,
		Self = 2,
	}

	// 战斗端的宝可梦实体（非 MonoBehaviour，可作为纯数据在流程中使用）
	[Serializable]
	public class PokemonEntity
	{
		// -------- 基础配置/静态信息 --------
		public int Id { get; private set; }
		public string Name { get; private set; }

		// 基础参数（来自 cfg.pokemon）
		public int BaseAttack { get; private set; }
		public int BaseDefense { get; private set; }
		public int BaseSpAttack { get; private set; }
		public int BaseSpDefense { get; private set; }
		public int BaseSpeed { get; private set; }
		public int BaseHP { get; private set; }

        // -------- 战斗中的即时状态 --------
        public int lv;
        public int MaxHP { get; private set; }
		public int HP { get; private set; }
		public int Shield { get; private set; }
		public int ActionPoint { get; private set; }

		public bool IsAlive => HP > 0;

		// 学会的招式
		public readonly List<cfg.move> Moves = new List<cfg.move>(4);

		// 随机源（频次、多段伤害等用）
		private System.Random _rng;

        public Action<PokemonEntity,cfg.move> ToUseMove;

		public PokemonEntity(int seed = 0)
		{
			_rng = seed == 0 ? new System.Random() : new System.Random(seed);
		}

		// 由表初始化一只宝可梦
		public static PokemonEntity CreateFromConfig(int pokemonId, int seed = 0, int initActionPoint = 1)
		{
			var cfgPokemon = LubanManager.Instance.GetPokemon(pokemonId);
			if (cfgPokemon == null) throw new Exception($"PokemonEntity: 未找到 pokemonId={pokemonId} 的配置。");

			var entity = new PokemonEntity(seed)
			{
				Id = cfgPokemon.PokemonId,
				Name = cfgPokemon.Name,

				BaseAttack = cfgPokemon.Attack,
				BaseDefense = cfgPokemon.Defense,
				BaseSpAttack = cfgPokemon.SpAttack,
				BaseSpDefense = cfgPokemon.SpDefense,
				BaseSpeed = cfgPokemon.Speed,
				BaseHP = cfgPokemon.Hp,
			};

			// 这里直接用基础 HP 作为 MaxHP，可根据等级/成长再做换算
			entity.MaxHP = Mathf.Max(1, entity.BaseHP);
			entity.HP = entity.MaxHP;
			entity.Shield = 0;
			entity.ActionPoint = Mathf.Max(0, initActionPoint);

			return entity;
		}

        // 新增：泛型工厂，支持直接创建 AIPokemonEntity 等子类
        public static T CreateFromConfig<T>(int pokemonId, int seed = 0, int initActionPoint = 1) where T : PokemonEntity, new()
        {
            var cfgPokemon = LubanManager.Instance.GetPokemon(pokemonId);
            if (cfgPokemon == null) throw new Exception($"PokemonEntity: 未找到 pokemonId={pokemonId} 的配置。");

            var entity = new T();
            if (seed != 0)
            {
                entity._rng = new System.Random(seed);
            }

            entity.Id = cfgPokemon.PokemonId;
            entity.Name = cfgPokemon.Name;

            entity.BaseAttack = cfgPokemon.Attack;
            entity.BaseDefense = cfgPokemon.Defense;
            entity.BaseSpAttack = cfgPokemon.SpAttack;
            entity.BaseSpDefense = cfgPokemon.SpDefense;
            entity.BaseSpeed = cfgPokemon.Speed;
            entity.BaseHP = cfgPokemon.Hp;

            entity.MaxHP = Mathf.Max(1, entity.BaseHP);
            entity.HP = entity.MaxHP;
            entity.Shield = 0;
            entity.ActionPoint = Mathf.Max(0, initActionPoint);

            return entity;
        }

		// 用一组 moveId 配置招式（从 Luban 取）
		public void SetMovesByIds(IEnumerable<int> moveIds, int maxCount = 4)
		{
			Moves.Clear();
			if (moveIds == null) return;

			foreach (var id in moveIds)
			{
				if (Moves.Count >= maxCount) break;
				var mv = LubanManager.Instance.GetMove(id);
				if (mv != null) Moves.Add(mv);
			}
		}

		// 回到战斗初始状态（保留已学会的招式）
		public void ResetForBattle(int initActionPoint = 1)
		{
			MaxHP = Mathf.Max(1, BaseHP);
			HP = MaxHP;
			Shield = 0;
			ActionPoint = Mathf.Max(0, initActionPoint);
		}

		// -------- 数值操作 --------

		public bool CanPay(int cost) => cost <= ActionPoint;

		public void Pay(int cost)
		{
			ActionPoint = Mathf.Max(0, ActionPoint - Mathf.Max(0, cost));
		}

		public void AddActionPoint(int ap)
		{
			if (ap <= 0) return;
			ActionPoint += ap;
		}

        public void ResetActionPoint(int initActionPoint = 1)
        {
            ActionPoint = Mathf.Max(0, initActionPoint);
        }

		// 承受伤害，优先扣护盾；返回实际对 HP 造成的伤害
		public int ReceiveDamage(int amount, bool pierceShield = false)
		{
			amount = Mathf.Max(0, amount);
			int hpDmg = 0;

			if (!pierceShield && Shield > 0)
			{
				int fromShield = Mathf.Min(Shield, amount);
				Shield -= fromShield;
				amount -= fromShield;
			}

			if (amount > 0)
			{
				int before = HP;
				HP = Mathf.Max(0, HP - amount);
				hpDmg = before - HP;
			}

			return hpDmg;
		}

		public void Heal(int amount)
		{
			if (amount <= 0 || !IsAlive) return;
			HP = Mathf.Min(MaxHP, HP + amount);
		}

		public void AddShield(int amount)
		{
			if (amount <= 0) return;
			Shield += amount;
		}

		// 简单伤害计算：物理 power + 攻击系数（可替换为正式公式）
		public int CalcDamage(int power)
		{
			int dmg = power + Mathf.RoundToInt(BaseAttack * 0.2f);
			return Mathf.Max(1, dmg);
		}

		// -------- 使用招式（通用执行器，占位逻辑） --------
		// 返回是否成功释放（支付了费用并生效）
		public bool UseMove(cfg.move move, PokemonEntity target, out string log)
		{
			log = string.Empty;
			if (move == null) { log = "招式为空"; return false; }
			if (!IsAlive) { log = $"{Name} 已倒下"; return false; }
			if (!CanPay(move.Cost)) { log = "行动点不足"; return false; }

			// 支付行动点
			Pay(move.Cost);

			// 目标选择
			bool targetIsSelf = (MoveTarget)move.MoveTarget == MoveTarget.Self;
			var realTarget = targetIsSelf ? this : target;

			// 频次（如 [2,5] 表示 2~5 次）
			int times = 1;
			if (move.Frequency != null && move.Frequency.Count >= 2)
			{
				int min = Mathf.Max(1, move.Frequency[0]);
				int max = Mathf.Max(min, move.Frequency[1]);
				times = _rng.Next(min, max + 1);
			}

			int totalHPDamage = 0;

			// 伤害
			if (move.Power > 0 && realTarget != null)
			{
				for (int i = 0; i < times; i++)
				{
					int dmg = CalcDamage(move.Power);
					totalHPDamage += realTarget.ReceiveDamage(dmg);
					if (!realTarget.IsAlive) break;
				}
			}

			if (move.Shield > 0)
			{
				realTarget?.AddShield(move.Shield);
			}

			// 行动点奖励
			if (move.Points > 0) AddActionPoint(move.Points);

			// 文字日志
			if (totalHPDamage > 0 && realTarget != null)
			{
				log = $"{Name} 使用 {move.Name} 对 {(realTarget == this ? "自己" : realTarget.Name)} 造成 {totalHPDamage} 伤害";
			}
			else if (move.Shield > 0 && realTarget != null)
			{
				log = $"{Name} 使用 {move.Name} 使 {(realTarget == this ? "自己" : realTarget.Name)} 获得 {move.Shield} 护盾";
			}
			else
			{
				log = $"{Name} 使用了 {move.Name}";
			}

			return true;
		}
	}
}