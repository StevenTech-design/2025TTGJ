using UnityEngine.Video;

namespace TTGJ.UI
{
    public class PVPanel : UIPanel
    {
        public double GetPVTimeLength()
        { 
            return GetComponentInChildren<VideoPlayer>().length;
        }
    }
}