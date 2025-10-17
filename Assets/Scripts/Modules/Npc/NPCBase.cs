using UnityEngine;
using TTGJ.Item;

namespace TTGJ.Npc
{
    public class NPCBase : Liftable
    {
        [SerializeField]
        private GameObject ghostPumpkinPrefab;
        public virtual void OnInteract()
        {
            GameObject ghostPumpkin = Instantiate(ghostPumpkinPrefab);
            ghostPumpkin.SetActive(true);
            ghostPumpkin.transform.position = transform.right * 2 + transform.position;
        }
    }
}