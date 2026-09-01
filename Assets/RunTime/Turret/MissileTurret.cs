using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : MonoBehaviour
{
    #region 인스펙터 (설정)
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("태그")]
    [SerializeField] private string _targetTag = "Enemy";

    [Header("포탑 회전, 회전 속도")]
    [SerializeField] private Transform _turretRotation;
    [SerializeField] private float _turretSpeed = 10f;

    [Header("발사 위치")]
    [SerializeField] private Transform _firePoint;
    #endregion

    #region 인스펙터 (포탑 설정)
    [Header("범위")]
    [SerializeField] private float _range = 30f;

    [Header("발사 속도")]
    [SerializeField] private float _fireRate = 0.25f;

    [Header("발사 간격")]
    [SerializeField] private float _fireCountdown = 2f;
    #endregion

    void Start()
    {
        if (_turretRotation == null)
        {
            CPrint.Warn("회전 설정 확인 필요");
        }

        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }

    private void UpdateTarget()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(_targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemt = null;

        for (int i = 0; i < enemys.Length; i++)
        {
            GameObject enemy = enemys[i];
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
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

        // 타겟 록 온
        Vector3 dir = _target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(_turretRotation.rotation, lookRotation, Time.deltaTime * _turretSpeed).eulerAngles;
        _turretRotation.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // 발사
        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / _fireRate;
        }

        _fireCountdown -= Time.deltaTime;
    }

    private void Shoot()
    {
        GameObject missileGo = MissileObjectPool._instance.SpawnMissile(_firePoint.position, _firePoint.rotation);

        if (missileGo == null)
        {
            return;
        }

        Missile missile = missileGo.GetComponent<Missile>();

        if (missile != null)
        {
            missile.Seek(_target, MissileImpactObjectPool._instance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 원형으로 범위를 보여줌
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
