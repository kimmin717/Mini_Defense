using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObjectPool : MonoBehaviour
{
    public static LaserObjectPool _instance;

    #region 인터펙스
    [Header("프리팹")]
    [SerializeField] private GameObject _laserPrefab = null;

    [Header("오브젝트 풀")]
    [SerializeField] private int _prewarmCount = 60;

    [Header("수명 시간")]
    [Min(0.1f)]
    [SerializeField] private float _lifeTime = 10f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveLaser = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    #endregion

    private void Awake()
    {
        if (_instance != null)
        {
            CPrint.Warn("LaserObjectPool이 씬에 하나더 존재함 확인 필요");
            return;
        }

        _instance = this;
    }

    void Start()
    {
        if (_laserPrefab == null)
        {
            CPrint.Warn("LaserPrefab 인스펙터 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();
    }


    void Update()
    {
        UpdateAliveLaser();
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("LaserPool_Root");

        _poolRoot = root.transform;
    }

    private void Prewarm()
    {
        for (int i = 0; i < _prewarmCount; i++)
        {

            GameObject laser = Instantiate(_laserPrefab, _poolRoot);

            laser.SetActive(false);

            _pool.Enqueue(laser);
        }

        CPrint.Success($"Prewarm = {_prewarmCount}");

    }

    private void ReturnToPool(GameObject laser)
    {
        if (laser == null)
        {
            return;
        }

        laser.SetActive(false);

        laser.transform.SetParent(_poolRoot);

        _pool.Enqueue(laser);
    }

    private void RemoveLifeIfExists(GameObject laser)
    {
        if (laser == null)
        {
            return;
        }

        if (_lifeMap.ContainsKey(laser))
        {
            _lifeMap.Remove(laser);
        }
    }

    private void UpdateAliveLaser()
    {
        for (int i = _aliveLaser.Count - 1; i >= 0; i--)
        {
            GameObject laser = _aliveLaser[i];

            if (laser == null)
            {
                _aliveLaser.RemoveAt(i);

                continue;
            }

            if (!laser.activeSelf)
            {
                ReturnToPool(laser);
                _aliveLaser.RemoveAt(i);
                RemoveLifeIfExists(laser);

                CPrint.Once("킬존 리사이클", "비활성화된 Laser를 다시 풀로 회수");

                continue;
            }

            if (!_lifeMap.ContainsKey(laser))
            {
                CPrint.Warn($"라이프 정보 없음 : {laser.name}");

                ReturnToPool(laser);

                _aliveLaser.RemoveAt(i);

                continue;
            }

            _lifeMap[laser] -= Time.deltaTime;

            if (_lifeMap[laser] < 0.0f)
            {
                ReturnToPool(laser);
                _aliveLaser.RemoveAt(i);
                _lifeMap.Remove(laser);
            }

        }
    }

    private GameObject GetLaserFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject laser = _pool.Dequeue();

            return laser;
        }

        GameObject extra = Instantiate(_laserPrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    public GameObject SpawnLaser(Vector3 position, Quaternion rotation)
    {
        GameObject laser = GetLaserFromPool();

        // 풀 안에서는 루트 하위에 정리되어 있다.
        laser.transform.SetParent(null);
        laser.transform.SetPositionAndRotation(position, rotation);

        // 풀에서 꺼낸 객체를 다시 사용
        laser.SetActive(true);

        if (!_aliveLaser.Contains(laser))
        {
            _aliveLaser.Add(laser);
        }

        else
        {
            CPrint.Warn($"중복 스폰 감지 : {laser.name}");
        }

        _lifeMap[laser] = _lifeTime;

        CPrint.Log($"스폰 : {laser.name} / Alive = {_aliveLaser.Count} / Pool = {_pool.Count}");

        return laser;
    }
}
