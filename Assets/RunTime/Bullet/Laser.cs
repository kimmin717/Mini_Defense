using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("속도")]
    [SerializeField] private float _speed = 80f;

    [Header("데미지")]
    [SerializeField] private float _damage = 15f;

    #endregion
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
