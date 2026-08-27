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

    [Header("임팩트")]
    [SerializeField] private GameObject _BulletImpact;
    #endregion

    public void Seek(Transform target)
    {
        _target = target;
    }

    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
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
        // 오브젝트 풀 만들어서 해결해야지

        GameObject effectIns = Instantiate(_BulletImpact, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        Destroy(_target.gameObject);
        Destroy(gameObject);
    }
}
