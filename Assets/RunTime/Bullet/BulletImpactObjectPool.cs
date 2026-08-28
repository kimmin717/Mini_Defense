using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpactObjectPool : MonoBehaviour
{
    #region 인스팩터
    [Header("프리팹")]
    [SerializeField] private GameObject _bulletImpactPrefab;

    [Header("오브젝트 풀")]
    [SerializeField] private int _bulletImpactCount = 60;

    [Header("이펙트 유지 시간")]
    [SerializeField] private float _bulletImpactLifeTime = 2f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveBulletImpact = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    #endregion

    void Start()
    {
        if(_bulletImpactPrefab == null)
        {
            CPrint.Warn("BulletImpactPrefab 인스팩터 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();
    }

    
    void Update()
    {
        UpdateAliveBulletImpact();
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("BulletImpactPool_Root");

        _poolRoot = root.transform;
    }

    private void Prewarm()
    {
        for (int i = 0; i < _bulletImpactCount; i++)
        {

            GameObject bullet = Instantiate(_bulletImpactPrefab, _poolRoot);

            bullet.SetActive(false);

            _pool.Enqueue(bullet);
        }

        CPrint.Success($"BulletImpactCount = {_bulletImpactCount}");

    }

    private void ReturnToPool(GameObject bulletImpact)
    {
        if (bulletImpact == null)
        {
            return;
        }

        bulletImpact.SetActive(false);

        bulletImpact.transform.SetParent(_poolRoot);

        _pool.Enqueue(bulletImpact);
    }

    private void RemoveLifeIfExists(GameObject bulletImpact)
    {
        if (bulletImpact == null)
        {
            return;
        }

        if (_lifeMap.ContainsKey(bulletImpact))
        {
            _lifeMap.Remove(bulletImpact);
        }
    }

    private void UpdateAliveBulletImpact()
    {
        for (int i = _aliveBulletImpact.Count - 1; i >= 0; i--)
        {
            GameObject bulletImpact = _aliveBulletImpact[i];

            if (bulletImpact == null)
            {
                _aliveBulletImpact.RemoveAt(i);

                continue;
            }

            if (!bulletImpact.activeSelf)
            {
                ReturnToPool(bulletImpact);
                _aliveBulletImpact.RemoveAt(i);
                RemoveLifeIfExists(bulletImpact);

                CPrint.Once("킬존 리사이클", "비활성화된 BulletImpact를 다시 풀로 회수");

                continue;
            }

            if (!_lifeMap.ContainsKey(bulletImpact))
            {
                CPrint.Warn($"라이프 정보 없음 : {bulletImpact.name}");

                ReturnToPool(bulletImpact);

                _aliveBulletImpact.RemoveAt(i);

                continue;
            }

            _lifeMap[bulletImpact] -= Time.deltaTime;

            if (_lifeMap[bulletImpact] < 0.0f)
            {
                ReturnToPool(bulletImpact);
                _aliveBulletImpact.RemoveAt(i);
                _lifeMap.Remove(bulletImpact);
            }

        }
    }

    private GameObject GetBulletImpactFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject bulletImpact = _pool.Dequeue();

            return bulletImpact;
        }

        GameObject extra = Instantiate(_bulletImpactPrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    public GameObject SpawnImpact(Vector3 position, Quaternion rotation)
    {
        // 풀에서 이펙트 가져오기
        GameObject bulletImpact = GetBulletImpactFromPool();

        // 전달받은 위치/회전값으로 설정
        bulletImpact.transform.SetPositionAndRotation(position, rotation);
        bulletImpact.SetActive(true);

        _aliveBulletImpact.Add(bulletImpact);
        _lifeMap[bulletImpact] = _bulletImpactLifeTime;

        return bulletImpact;
    }

}
