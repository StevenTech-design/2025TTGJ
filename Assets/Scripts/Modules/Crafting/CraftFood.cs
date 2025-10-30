using TTGJ.Audio;
using TTGJ.Buff;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using TTGJ.Luban;

namespace TTGJ.Crafting
{
    public class CraftFood : Liftable,IEatable
    {
        public virtual void OnEat() {
           var config = LubanManager.Instance.GetItemNew((int)itemType);
           BuffManager.Instance.RemoveAllBuff(PlayerController.GetActivePlayer().transform);
           foreach (var item in config.BuffType)
            {
                BuffManager.Instance.AddBuff((BuffType)item, PlayerController.GetActivePlayer().transform, false);
            }
            AudioManager.Instance.PlaySFX(Audio.AudioType.Eat_Popcorn);
        }
        public virtual bool CanEat() {
            return true;
        }
    }
}