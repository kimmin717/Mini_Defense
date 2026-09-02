using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    BuildManager _buildManager;

    private void Start()
    {
        _buildManager = BuildManager._instance;
    }

    public void PurchaseTurret()
    {
        CPrint.Log("포탑 구입");
        _buildManager.SetTurretToBuild(_buildManager._standardTurretPrefab);
    }

    public void PurchaseMissileTurret()
    {
        CPrint.Log("미사일 포탑 구입");
        _buildManager.SetTurretToBuild(_buildManager._missileTurretPrefab);
    }

    public void PurchaseLaserTurret()
    {
        CPrint.Log("레이저 포탑 구입");
        _buildManager.SetTurretToBuild(_buildManager._laserTurretPrefab);
    }
}
