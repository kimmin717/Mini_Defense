using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    // 싱글턴
    public static BuildManager _instance;

    [Header("포탑")]
    [SerializeField] private GameObject _standardTurretPrefab;

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
        _turretToBuild = _standardTurretPrefab;
    }

    public GameObject GetTurretToBuild()
    {
        return _turretToBuild;
    }
}
