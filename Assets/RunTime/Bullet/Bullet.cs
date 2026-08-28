using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("속도")]
    [SerializeField] private float _speed = 70f;

    [Header("데미지")]
    [SerializeField] private float _damage = 10f;

    [Header("임팩트")]
    [SerializeField] private BulletImpactObjectPool _impactPool;
    #endregion

    private void OnEnable()
    {
        _target = null;
    }

    public void Seek(Transform target, BulletImpactObjectPool impactPool)
    {
        _target = target;
        _impactPool = impactPool;
    }

    void Update()
    {
        if (_target == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        Vector3 dir = _target.position - transform.position;
        float distanceThisFrame = _speed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

    }

    private void HitTarget()
    {
        if (_impactPool != null)
        {
            _impactPool.SpawnImpact(transform.position, transform.rotation);
        }

        // 데미지 매커니즘 
        if (_target != null)
        {
            Enemy enemy = _target.GetComponent<Enemy>();

            if(enemy != null)
            {
                enemy.TakeDamage(_damage);
            }

        }

        gameObject.SetActive(false);
    }
}
