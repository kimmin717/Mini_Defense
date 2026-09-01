using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileObjectPool : MonoBehaviour
{
    public static MissileObjectPool _instance;

    #region 인터펙스
    [Header("프리팹")]
    [SerializeField] private GameObject _missilePrefab = null;

    [Header("오브젝트 풀")]
    [SerializeField] private int _prewarmCount = 60;

    [Header("수명 시간")]
    [Min(0.1f)]
    [SerializeField] private float _lifeTime = 10f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveMissile = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    #endregion

    private void Awake()
    {
        if (_instance != null)
        {
            CPrint.Warn("MissileObjectPool이 씬에 하나더 존재함 확인 필요");
            return;
        }

        _instance = this;
    }

    void Start()
    {
        if (_missilePrefab == null)
        {
            CPrint.Warn("MissilePrefab 인스펙터 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();
    }


    void Update()
    {
        UpdateAliveMissile();
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("MissilePool_Root");

        _poolRoot = root.transform;
    }

    private void Prewarm()
    {
        for (int i = 0; i < _prewarmCount; i++)
        {

            GameObject bullet = Instantiate(_missilePrefab, _poolRoot);

            bullet.SetActive(false);

            _pool.Enqueue(bullet);
        }

        CPrint.Success($"Prewarm = {_prewarmCount}");

    }

    private void ReturnToPool(GameObject missile)
    {
        if (missile == null)
        {
            return;
        }

        missile.SetActive(false);

        missile.transform.SetParent(_poolRoot);

        _pool.Enqueue(missile);
    }

    private void RemoveLifeIfExists(GameObject missile)
    {
        if (missile == null)
        {
            return;
        }

        if (_lifeMap.ContainsKey(missile))
        {
            _lifeMap.Remove(missile);
        }
    }

    private void UpdateAliveMissile()
    {
        for (int i = _aliveMissile.Count - 1; i >= 0; i--)
        {
            GameObject bullet = _aliveMissile[i];

            if (bullet == null)
            {
                _aliveMissile.RemoveAt(i);

                continue;
            }

            if (!bullet.activeSelf)
            {
                ReturnToPool(bullet);
                _aliveMissile.RemoveAt(i);
                RemoveLifeIfExists(bullet);

                CPrint.Once("킬존 리사이클", "비활성화된 Bullet를 다시 풀로 회수");

                continue;
            }

            if (!_lifeMap.ContainsKey(bullet))
            {
                CPrint.Warn($"라이프 정보 없음 : {bullet.name}");

                ReturnToPool(bullet);

                _aliveMissile.RemoveAt(i);

                continue;
            }

            _lifeMap[bullet] -= Time.deltaTime;

            if (_lifeMap[bullet] < 0.0f)
            {
                ReturnToPool(bullet);
                _aliveMissile.RemoveAt(i);
                _lifeMap.Remove(bullet);
            }

        }
    }

    private GameObject GetMissileFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject bullet = _pool.Dequeue();

            return bullet;
        }

        GameObject extra = Instantiate(_missilePrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    public GameObject SpawnMissile(Vector3 position, Quaternion rotation)
    {
        GameObject missile = GetMissileFromPool();

        // 풀 안에서는 루트 하위에 정리되어 있다.
        missile.transform.SetParent(null);
        missile.transform.SetPositionAndRotation(position, rotation);

        // 풀에서 꺼낸 객체를 다시 사용
        missile.SetActive(true);

        if (!_aliveMissile.Contains(missile))
        {
            _aliveMissile.Add(missile);
        }

        else
        {
            CPrint.Warn($"중복 스폰 감지 : {missile.name}");
        }

        _lifeMap[missile] = _lifeTime;

        CPrint.Log($"스폰 : {missile.name} / Alive = {_aliveMissile.Count} / Pool = {_pool.Count}");

        return missile;
    }
}
