using System.Collections;
using System.Collections.Generic;
using TTGJ.Entity;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private PlayerEntity player;
    [SerializeField] private AirShipEntity airShip;

    private void Awake()
    {
        EntityManager.Instance.AddEntity(player);
        EntityManager.Instance.AddEntity(airShip);
    }
}
