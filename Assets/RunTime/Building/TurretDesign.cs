using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretDesign : MonoBehaviour
{
    #region 인스펙터
    [Header("포탑")]
    [SerializeField] private GameObject _turretPrefab;

    [Header("가격")]
    [SerializeField] private int _cost;
    #endregion


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
