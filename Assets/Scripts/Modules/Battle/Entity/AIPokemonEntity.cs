using System.Collections.Generic;
using cfg;
using TTGJ.Luban;
using UnityEngine;

namespace TTGJ.Battle
{
    public class AIPokemonEntity : PokemonEntity
    {
        public void TickLogic()
        {
             var skill = GetPokemonSkill();
             if(skill != null) {
                ToUseMove?.Invoke(this, skill);
             }
        }
        private move GetPokemonSkill() { 
            List<PokemonAISkill> skills = new List<PokemonAISkill>();
            if(lv < 10) {
                skills = LubanManager.Instance.GetPokemonMove(Id).LowSkill;
            } else if(lv < 20) {
                skills = LubanManager.Instance.GetPokemonMove(Id).MediumSkill;
            } else {
                skills = LubanManager.Instance.GetPokemonMove(Id).HighSkill;
            }
            int random = Random.Range(0, 100);
            foreach (var skill in skills) {
                if(random < skill.Probability) {
                    return LubanManager.Instance.GetMove(skill.SkillID);
                }
            }
            return null;
        }
    }
}
