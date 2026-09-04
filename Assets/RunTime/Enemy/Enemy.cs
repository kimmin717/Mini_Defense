using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _wavePointIndex = 0;

    [Header("체력 설정")]
    [Min(0.1f)]
    [SerializeField] private float _enemyHP = 10f;
    #endregion

    #region 내부변수
    private float _temporaryHP;
    #endregion

    private void OnEnable()
    {
        // 체력 초기화
        _temporaryHP = _enemyHP;

        // 위치 초기화
        _wavePointIndex = 0;

        if (WayPoints._points != null && WayPoints._points.Length > 0)
        {
            _target = WayPoints._points[0];
        }
    }

    void Start()
    {
        if (_target == null)
        {
            CPrint.Log("Target 확인 필요 (인스펙터를 확인 하시오)");
            return;
        }

    }


    void Update()
    {
        if (_target == null)
        {
            CPrint.Log("Target 확인 필요 (인스펙터를 확인 하시오)");
            return;
        }

        Vector3 dir = _target.position - transform.position;
        transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);

        // 다음 웨이포인트로 가기 위한 if문
        if (Vector3.Distance(transform.position, _target.position) <= 0.4f)
        {
            GetNextMovePoint();
        }

    }

    private void GetNextMovePoint()
    {
        if( _wavePointIndex >= WayPoints._points.Length - 1)
        {
            Die();
            return;
        }

        // 배열의 인덱스를 높여 다음 장소로 이동하개 하는 것
        _wavePointIndex++;
        _target = WayPoints._points[_wavePointIndex];


    }

    public void TakeDamage(float amount)
    {
        _temporaryHP -= amount;

        CPrint.Log($"{gameObject.name} 피격! 남은 HP : {_temporaryHP}");

        if (_temporaryHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

}
