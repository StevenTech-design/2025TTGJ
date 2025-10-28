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
           BuffManager.Instance.RemoveAllBuff(PlayerController.Instance.transform);
           foreach (var item in config.BuffType)
            {
                BuffManager.Instance.AddBuff((BuffType)item, PlayerController.Instance.transform, false);
            }
        }
        public virtual bool CanEat() {
            return true;
        }
    }
}