using UnityEngine;

namespace TTGJ.Interactable
{
    public interface IFallCollisionable
    {
        void OnFallCollision(Collider other);
    }
}