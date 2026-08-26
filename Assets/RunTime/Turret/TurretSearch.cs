using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSearch : MonoBehaviour
{
    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("태그")]
    [SerializeField] private string _targetTag = "Enemy";

    [Header("범위")]
    [SerializeField] private float _range = 15f;
    #endregion

    void Start()
    {
        if(_target == null)
        {
            CPrint.Warn("Target 없음 확인 필요");

            return;
        }

        // nameof : 변수, 클래스, 메서드 등의 이름(식별자)을 문자열(String)로 변환해 주는 C#의 연산자
        InvokeRepeating(nameof(UpdateTarget),0f,0.5f);
    }

    private void UpdateTarget()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(_targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemt = null;

        // 프렌치 말고 다른거 쓸수 없나?
        foreach (GameObject enemy in enemys)
        { 
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemt = enemy;
            }
        }

        if (nearestEnemt != null && shortestDistance <= _range)
        {
            _target = nearestEnemt.transform;
        }

        else
        {
            _target = null;
        }
    }

    
    void Update()
    {
        if (_target == null)
        {
            return;
        }



    }

    private void OnDrawGizmosSelected()
    {
        // 원형으로 범위를 보여줌
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
