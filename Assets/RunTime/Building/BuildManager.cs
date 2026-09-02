using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    // 싱글턴
    public static BuildManager _instance;

    [Header("기본 포탑")]
    [SerializeField] public GameObject _standardTurretPrefab;

    [Header("미사일 포탑")]
    [SerializeField] public GameObject _missileTurretPrefab;

    [Header("레이저 포탑")]
    [SerializeField] public GameObject _laserTurretPrefab;

    private GameObject _turretToBuild;

    private void Awake()
    {
        if (_instance != null)
        {
            CPrint.Warn("BuildManager가 씬에 하나더 존재함 확인 필요");
        }

        _instance = this;
    }

    private void Start()
    {
        if(_standardTurretPrefab == null || _missileTurretPrefab == null || _laserTurretPrefab == null)
        {
            CPrint.Warn("포탑 프리팹 열결 확인 필요");
            return;
        }


    }

    public GameObject GetTurretToBuild()
    {
        return _turretToBuild;
    }

    public void SetTurretToBuild(GameObject turret)
    {
        _turretToBuild = turret;
    }
}
