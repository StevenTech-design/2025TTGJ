using TTGJ.GamePlay;
using UnityEngine;

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
            ghostPumpkin.transform.position = transform.forward *2 + transform.position;
        }
    }
}