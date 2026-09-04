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

    private TurretDesign _turretToBuild;

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

    public bool CanBuild {get { return _turretToBuild != null; } }
    public bool HoldingMoney { get { return PlayerStats._money >= _turretToBuild._cost; } }

    public void BuildTurretOn(Node node)
    {
        if(PlayerStats._money < _turretToBuild._cost)
        {
            CPrint.Log("포탑을 건설할 돈이 부족합니다.");
            return;
        }

        PlayerStats._money -= _turretToBuild._cost;

        GameObject turret = Instantiate(_turretToBuild._turretPrefab, node.GetBuildPosition(), Quaternion.identity);
        node._turret = turret;

        CPrint.Log($"포탑 건설! 남은 돈 : {PlayerStats._money}");
    }

    public void SetTurretToBuild(TurretDesign turret)
    {
        _turretToBuild = turret;
    }
}
