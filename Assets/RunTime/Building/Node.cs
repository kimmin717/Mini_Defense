using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    #region 인스펙터
    // 마우스 커서가 노드위에 올라가 있을때 변경할 노드 색깔
    [Header("색갈 변경")]
    [SerializeField] private Color _abideColor;

    [Header("색깔 변경(돈 부족)")]
    [SerializeField] private Color _notMoneyColor;

    [Header("위치 설정")]
    [SerializeField] private Vector3 _positioneOffset;

    [Header("옵션")]
    [SerializeField]public GameObject _turret;
    #endregion

    #region 내부변수
    // 마우스 커서가 노드에서 벗어났을때 변경할 노드 색깔
    private Color _startColor;
    private Renderer _rend;
    #endregion

    BuildManager _buildManager;

    private void Start()
    {
        _rend = GetComponent<Renderer>();

        _startColor = _rend.material.color;

        _buildManager = BuildManager._instance;
    }

    // 마우스 커서가 특정 위치에 있을때 한번만 호출 되는 메세지 함수
    private void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!_buildManager.CanBuild)
        {
            return;
        }

        if (_buildManager.HoldingMoney)
        {
            _rend.material.color = _abideColor;
        }

        else
        {
            _rend.material.color = _notMoneyColor;
        }

    }

    //  마우스 커서가 특정 위치에서 벗어나면 한번만 호출되는 메세지 함수
    private void OnMouseExit()
    {
        _rend.material.color = _startColor;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!_buildManager.CanBuild)
        {
            return;
        }

        if (_turret != null)
        {
            CPrint.Log("Can't Build There!");
            return;
        }

        _buildManager.BuildTurretOn(this);
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + _positioneOffset;
    }

}
