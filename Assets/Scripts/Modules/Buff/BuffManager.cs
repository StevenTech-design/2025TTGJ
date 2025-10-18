using TTGJ.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace TTGJ.Buff
{
    public class BuffManager : Singleton<BuffManager>
    {
        private Dictionary<GameObject, List<BuffBase>> buffs = new Dictionary<GameObject, List<BuffBase>>();
        public void AddBuff(Transform target, BuffBase buff)
        {
            if (!buffs.ContainsKey(target.gameObject))
            {
                buffs[target.gameObject] = new List<BuffBase>();
            }
            for(int i = 0; i < buffs[target.gameObject].Count; i++) { 
                buffs[target.gameObject][i].EndBuff();
            }
            buffs[target.gameObject].Clear();
            buffs[target.gameObject].Add(buff);
            buff.StartBuff();
        }
        public void RemoveBuff(Transform target, BuffBase buff)
        {
            if (buffs.ContainsKey(target.gameObject))
            {
                buffs[target.gameObject].Remove(buff);
            }
        }
        public void RemoveAllBuff(Transform target)
        {
            if (buffs.ContainsKey(target.gameObject))
            {
                buffs[target.gameObject].Clear();
            }
        }
        public void AddBuff(Transform target, List<BuffBase> buffList) { 
            if (!buffs.ContainsKey(target.gameObject))
            {
                buffs[target.gameObject] = new List<BuffBase>();
            }
            for(int i = 0; i < buffs[target.gameObject].Count; i++) { 
                buffs[target.gameObject][i].EndBuff();
            }
            buffs[target.gameObject].Clear();
            buffs[target.gameObject].AddRange(buffList);
            for(int i = 0; i < buffList.Count; i++) { 
                buffList[i].StartBuff();
            }
            
        }
    }
}