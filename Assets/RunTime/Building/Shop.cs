using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Shop : MonoBehaviour
{
    #region 인스펙터
    [Header("UI")]
    [SerializeField] private GameObject _shopUI;
    #endregion

    #region 내부변수
    private bool _shopOnOff = true;
    #endregion

    BuildManager _buildManager;

    private void Start()
    {
        _buildManager = BuildManager._instance;

        if (_shopUI != null)
        {
            _shopUI.SetActive(_shopOnOff);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShopOnOff();
        }
    }

    private void ShopOnOff()
    {
        _shopOnOff = !_shopOnOff;

        if(_shopUI != null)
        {
            _shopUI.SetActive(_shopOnOff);
        }
    }

    public void PurchaseTurret()
    {
        if(!_shopOnOff)
        {
            return;
        }

        CPrint.Log("포탑 구입");
        _buildManager.SetTurretToBuild(_buildManager._standardTurretPrefab);
    }

    public void PurchaseMissileTurret()
    {
        if (!_shopOnOff)
        {
            return;
        }

        CPrint.Log("미사일 포탑 구입");
        _buildManager.SetTurretToBuild(_buildManager._missileTurretPrefab);
    }

    public void PurchaseLaserTurret()
    {
        if (!_shopOnOff)
        {
            return;
        }

        CPrint.Log("레이저 포탑 구입");
        _buildManager.SetTurretToBuild(_buildManager._laserTurretPrefab);
    }
}
