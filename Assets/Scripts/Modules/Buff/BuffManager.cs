using TTGJ.Framework;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
namespace TTGJ.Buff
{
    public class BuffManager : Singleton<BuffManager>
    {
        private Dictionary<GameObject, List<BuffBase>> buffs = new Dictionary<GameObject, List<BuffBase>>();
        public void  AddBuff(Transform target, BuffBase buff)
        {
            if (!buffs.ContainsKey(target.gameObject))
            {
                buffs[target.gameObject] = new List<BuffBase>();
            }
            for(int i = 0; i < buffs[target.gameObject].Count; i++) { 
                if(buffs[target.gameObject][i] == null) {
                    continue;
                }
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
            if (!buffs.ContainsKey(target.gameObject))
            {
                return;
            }
            for(int i = 0; i < buffs[target.gameObject].Count; i++) { 
                if(buffs[target.gameObject][i] == null) {
                    continue;
                }
                buffs[target.gameObject][i].EndBuff();
            }
            buffs[target.gameObject].Clear();
            buffs.Remove(target.gameObject);
        }
        public async UniTask AddBuff(Transform target, List<BuffBase> buffList) { 
            if (!buffs.ContainsKey(target.gameObject))
            {
                buffs[target.gameObject] = new List<BuffBase>();
            }
            for(int i = 0; i < buffs[target.gameObject].Count; i++) { 
                buffs[target.gameObject][i].EndBuff();
            }
            buffs[target.gameObject].Clear();
            await UniTask.Delay(200);
            buffs[target.gameObject].AddRange(buffList);
            for(int i = 0; i < buffList.Count; i++) { 
                buffList[i].StartBuff();
            }
            
        }
    }
}