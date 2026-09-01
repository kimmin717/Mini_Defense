using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    #region 인스펙터
    // 마우스 커서가 노드위에 올라가 있을때 변경할 노드 색깔
    [Header("색갈 변경")]
    [SerializeField] private Color _abideColor;

    [Header("위치 설정")]
    [SerializeField] private Vector3 _positioneOffset;
    #endregion

    #region 내부변수
    // 마우스 커서가 노드에서 벗어났을때 변경할 노드 색깔
    private Color _startColor;
    private Renderer _rend;
    private GameObject _turret;
    #endregion

    private void Awake()
    {
        _rend = GetComponent<Renderer>();

        _startColor = _rend.material.color;
    }

    // 마우스 커서가 특정 위치에 있을때 한번만 호출 되는 메세지 함수
    private void OnMouseEnter()
    {
        _rend.material.color = _abideColor;
    }

    //  마우스 커서가 특정 위치에서 벗어나면 한번만 호출되는 메세지 함수
    private void OnMouseExit()
    {
        _rend.material.color = _startColor;
    }

    private void OnMouseDown()
    {
        if (_turret != null)
        {
            CPrint.Log("Can't Build There!");
            return;
        }

        GameObject _turretToBuild = BuildManager._instance.GetTurretToBuild();
        _turret = Instantiate(_turretToBuild, transform.position + _positioneOffset, transform.rotation);
    }

}
