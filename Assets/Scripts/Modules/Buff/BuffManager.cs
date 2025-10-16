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
            buffs[target.gameObject].Add(buff);
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
    }
}