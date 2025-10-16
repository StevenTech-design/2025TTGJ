namespace TTGJ.Buff
{
    public class BuffBase : MonoBehaviour
    {
        public float duration;

        protected virtual void OnEnable()
        {
            StartBuff();
        }

        protected virtual void StartBuff()
        {

        }
        protected virtual void EndBuff()
        {
            Destroy(this);
        }
        protected virtual void OnBuffUpdate(float remainingTime)
        { 
            
        }
    }
}