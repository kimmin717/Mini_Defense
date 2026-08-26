using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSearch : MonoBehaviour
{
    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("범위")]
    [SerializeField] private float _range = 15f;
    #endregion

    #region 내부변수

    #endregion

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        // 원형으로 범위를 보여줌
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
