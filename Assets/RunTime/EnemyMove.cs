using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _wavePointIndex = 0;

    // 여기에 체력 괄련해서 무언가를 만들고

    #endregion

    void Start()
    {
        if (_target == null)
        {
            CPrint.Log("Target 확인 필요 (인스펙터를 확인 하시오)");
            return;
        }

        _target = WayPoints._points[0];
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
            CPrint.Log("베이스 캠프 도착 이동 종료");
            // 임시 오브젝츠 풀과 킬존을 만들면 수정 필요
            Destroy(gameObject);
            return;
        }

        // 배열의 인덱스를 높여 다음 장소로 이동하개 하는 것
        _wavePointIndex++;
        _target = WayPoints._points[_wavePointIndex];
    }

}
