using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoScaleTest : MonoBehaviour
{
    void Start()
    {
    }


    void Update()
    {
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        PlayAnim();
    }
    
    private void PlayAnim()
    {
        transform.DOPunchScale(new Vector3(-0.2f, 0.5f, -0.2f), 0.2f);
    }
}