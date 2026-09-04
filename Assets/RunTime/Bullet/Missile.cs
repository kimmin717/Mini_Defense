using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("속도")]
    [SerializeField] private float _speed = 80f;

    [Header("데미지")]
    [SerializeField] private float _damage = 20f;

    [Header("폭발 밤위")]
    [SerializeField] private float _explosionRange = 10f;

    [Header("임팩트")]
    [SerializeField] private MissileImpactObjectPool _impactPool;
    #endregion

    private void OnEnable()
    {
        _target = null;
    }

    public void Seek(Transform target, MissileImpactObjectPool impactPool)
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

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        if(dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

    }

    private void HitTarget()
    {
        if (_impactPool != null)
        {
            _impactPool.SpawnImpact(transform.position, transform.rotation);
        }

        if(_explosionRange > 0f)
        {
            Explode();
        }

        else
        {
            Damage(_target);
        }

        gameObject.SetActive(false);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange);

       foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Damage(collider.transform);
            }
        }
    }

    private void Damage (Transform enemyTr)
    {
        if(enemyTr == null)
        { 
            return; 
        }

        Enemy enemy = enemyTr.GetComponent<Enemy>();

        if(enemy != null )
        {
            enemy.TakeDamage(_damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _explosionRange);
    }
}
