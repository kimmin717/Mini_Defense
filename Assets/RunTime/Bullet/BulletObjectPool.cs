using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{
    #region 인터펙스
    [Header("프리팹")]
    [SerializeField] private GameObject _bulletPrefab = null;

    [Header("오브젝트 풀")]
    [SerializeField] private int _prewarmCount = 60;

    [Header("수명 시간")]
    [Min(0.1f)]
    [SerializeField] private float _lifeTime = 10f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveBullet = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    #endregion

    void Start()
    {
        if (_bulletPrefab == null)
        {
            CPrint.Warn("BulletPrefab 인스펙터 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();
    }

    
    void Update()
    {
        UpdateAliveBullet();
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("BulletPool_Root");

        _poolRoot = root.transform;
    }

    private void Prewarm()
    {
        for (int i = 0; i < _prewarmCount; i++)
        {

            GameObject bullet = Instantiate(_bulletPrefab, _poolRoot);

            bullet.SetActive(false);

            _pool.Enqueue(bullet);
        }

        CPrint.Success($"Prewarm = {_prewarmCount}");

    }

    private void ReturnToPool(GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        bullet.SetActive(false);

        bullet.transform.SetParent(_poolRoot);

        _pool.Enqueue(bullet);
    }

    private void RemoveLifeIfExists(GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        if (_lifeMap.ContainsKey(bullet))
        {
            _lifeMap.Remove(bullet);
        }
    }

    private void UpdateAliveBullet()
    {
        for (int i = _aliveBullet.Count - 1; i >= 0; i--)
        {
            GameObject bullet = _aliveBullet[i];

            if (bullet == null)
            {
                _aliveBullet.RemoveAt(i);

                continue;
            }

            if (!bullet.activeSelf)
            {
                ReturnToPool(bullet);
                _aliveBullet.RemoveAt(i);
                RemoveLifeIfExists(bullet);

                CPrint.Once("킬존 리사이클", "비활성화된 Bullet를 다시 풀로 회수");

                continue;
            }

            if (!_lifeMap.ContainsKey(bullet))
            {
                CPrint.Warn($"라이프 정보 없음 : {bullet.name}");

                ReturnToPool(bullet);

                _aliveBullet.RemoveAt(i);

                continue;
            }

            _lifeMap[bullet] -= Time.deltaTime;

            if (_lifeMap[bullet] < 0.0f)
            {
                ReturnToPool(bullet);
                _aliveBullet.RemoveAt(i);
                _lifeMap.Remove(bullet);
            }

        }
    }

    private GameObject GetBulletFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject bullet = _pool.Dequeue();

            return bullet;
        }

        GameObject extra = Instantiate(_bulletPrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    public GameObject SpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = GetBulletFromPool();

        // 풀 안에서는 루트 하위에 정리되어 있다.
        bullet.transform.SetParent(null);
        bullet.transform.SetPositionAndRotation(position, rotation);

        // 풀에서 꺼낸 객체를 다시 사용
        bullet.SetActive(true);

        if (!_aliveBullet.Contains(bullet))
        {
            _aliveBullet.Add(bullet);
        }

        else
        {
            CPrint.Warn($"중복 스폰 감지 : {bullet.name}");
        }

        _lifeMap[bullet] = _lifeTime;

        CPrint.Log($"스폰 : {bullet.name} / Alive = {_aliveBullet.Count} / Pool = {_pool.Count}");

        return bullet;
    }
}
