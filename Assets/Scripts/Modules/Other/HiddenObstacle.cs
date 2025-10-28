using System.Collections;
using System.Collections.Generic;
using TTGJ.Interactable;
using TTGJ.Plant;
using Unity.VisualScripting;
using UnityEngine;

public class HiddenObstacle : MonoBehaviour
{
    [SerializeField]
    private ItemType keyType;
    private void OnCollisionEnter(Collision other) {
        if (!other.gameObject.TryGetComponent<Liftable>(out var liftable)) {
            return;
        }
        if (liftable.itemType == keyType) {
            Destroy(this.gameObject);
            return;
        }
    }
}
