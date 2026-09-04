using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretDesign 
{

    [Header("포탑 가격 설정")]
    [SerializeField] public GameObject _turretPrefab;
    [SerializeField] public int _cost;

}
