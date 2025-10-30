using System.Collections;
using System.Collections.Generic;
using TTGJ.Audio;
using TTGJ.Interactable;
using TTGJ.Plant;
using Unity.VisualScripting;
using UnityEngine;

namespace TTGJ.Other
{
    public class HiddenObstacle : MonoBehaviour
    {
        [SerializeField]
        private ItemType keyType;
        private void OnCollisionEnter(Collision other)
        {
             if (!other.gameObject.TryGetComponent<Liftable>(out var liftable)) {
            return;
        }
        if (liftable.itemType == keyType) {
            AudioManager.Instance.PlaySFX(TTGJ.Audio.AudioType.Key_Door_Opening_Closing_Sound);
            Destroy(this.gameObject);
            return;
        }

        }
    }
}