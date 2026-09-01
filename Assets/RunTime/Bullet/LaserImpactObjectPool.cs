using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserImpactObjectPool : MonoBehaviour
{
    public static LaserImpactObjectPool _instance;

    #region 인스팩터
    [Header("프리팹")]
    [SerializeField] private GameObject _laserImpactPrefab;

    [Header("오브젝트 풀")]
    [SerializeField] private int _laserImpactCount = 60;

    [Header("이펙트 유지 시간")]
    [SerializeField] private float _laserImpactLifeTime = 2f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveLaserImpact = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    #endregion

    private void Awake()
    {
        if (_instance != null)
        {
            CPrint.Warn("LaserImpactObjectPool이 씬에 하나더 존재함 확인 필요");
            return;
        }

        _instance = this;
    }

    void Start()
    {
        if (_laserImpactPrefab == null)
        {
            CPrint.Warn("LaserImpactPrefab 인스팩터 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();
    }


    void Update()
    {
        UpdateAliveLaserImpact();
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("LaserImpactPool_Root");

        _poolRoot = root.transform;
    }

    private void Prewarm()
    {
        for (int i = 0; i < _laserImpactCount; i++)
        {

            GameObject laser = Instantiate(_laserImpactPrefab, _poolRoot);

            laser.SetActive(false);

            _pool.Enqueue(laser);
        }

        CPrint.Success($"LaserImpactCount = {_laserImpactCount}");

    }

    private void ReturnToPool(GameObject laserImpact)
    {
        if (laserImpact == null)
        {
            return;
        }

        laserImpact.SetActive(false);

        laserImpact.transform.SetParent(_poolRoot);

        _pool.Enqueue(laserImpact);
    }

    private void RemoveLifeIfExists(GameObject laserImpact)
    {
        if (laserImpact == null)
        {
            return;
        }

        if (_lifeMap.ContainsKey(laserImpact))
        {
            _lifeMap.Remove(laserImpact);
        }
    }

    private void UpdateAliveLaserImpact()
    {
        for (int i = _aliveLaserImpact.Count - 1; i >= 0; i--)
        {
            GameObject laserImpact = _aliveLaserImpact[i];

            if (laserImpact == null)
            {
                _aliveLaserImpact.RemoveAt(i);

                continue;
            }

            if (!laserImpact.activeSelf)
            {
                ReturnToPool(laserImpact);
                _aliveLaserImpact.RemoveAt(i);
                RemoveLifeIfExists(laserImpact);

                CPrint.Once("킬존 리사이클", "비활성화된 LaserImpact를 다시 풀로 회수");

                continue;
            }

            if (!_lifeMap.ContainsKey(laserImpact))
            {
                CPrint.Warn($"라이프 정보 없음 : {laserImpact.name}");

                ReturnToPool(laserImpact);

                _aliveLaserImpact.RemoveAt(i);

                continue;
            }

            _lifeMap[laserImpact] -= Time.deltaTime;

            if (_lifeMap[laserImpact] < 0.0f)
            {
                ReturnToPool(laserImpact);
                _aliveLaserImpact.RemoveAt(i);
                _lifeMap.Remove(laserImpact);
            }

        }
    }

    private GameObject GetLaserImpactFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject laserImpact = _pool.Dequeue();

            return laserImpact;
        }

        GameObject extra = Instantiate(_laserImpactPrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    public GameObject SpawnImpact(Vector3 position, Quaternion rotation)
    {
        // 풀에서 이펙트 가져오기
        GameObject laserImpact = GetLaserImpactFromPool();

        // 전달받은 위치/회전값으로 설정
        laserImpact.transform.SetPositionAndRotation(position, rotation);
        laserImpact.SetActive(true);

        _aliveLaserImpact.Add(laserImpact);
        _lifeMap[laserImpact] = _laserImpactLifeTime;

        return laserImpact;
    }
}
