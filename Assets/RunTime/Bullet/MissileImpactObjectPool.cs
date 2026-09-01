using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileImpactObjectPool : MonoBehaviour
{
    public static MissileImpactObjectPool _instance;

    #region 인스펙터
    [Header("프리팹")]
    [SerializeField] private GameObject _missileImpactPrefab;

    [Header("오브젝트 풀")]
    [SerializeField] private int _missileImpactCount = 60;

    [Header("이펙트 유지 시간")]
    [SerializeField] private float _missileImpactLifeTime = 2f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveMissileImpact = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    #endregion

    private void Awake()
    {
        if(_instance != null)
        {
            CPrint.Warn("MissileImpactObjectPool이 씬에 하나더 존재함 확인 필요");
            return;
        }

        _instance = this;
    }


    void Start()
    {
        if(_missileImpactPrefab == null)
        {
            CPrint.Warn("MissileImpactPrefab 인스팩터 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();

    }

    void Update()
    {
        UpdateAliveMissileImpact();
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("MissileImpactPool_Root");

        _poolRoot = root.transform;
    }

    private void Prewarm()
    {
        for (int i = 0; i < _missileImpactCount; i++)
        {

            GameObject missile = Instantiate(_missileImpactPrefab, _poolRoot);

            missile.SetActive(false);

            _pool.Enqueue(missile);
        }

        CPrint.Success($"MissileImpactCount = {_missileImpactCount}");

    }

    private void ReturnToPool(GameObject missileImpact)
    {
        if (missileImpact == null)
        {
            return;
        }

        missileImpact.SetActive(false);

        missileImpact.transform.SetParent(_poolRoot);

        _pool.Enqueue(missileImpact);
    }

    private void RemoveLifeIfExists(GameObject missileImpact)
    {
        if (missileImpact == null)
        {
            return;
        }

        if (_lifeMap.ContainsKey(missileImpact))
        {
            _lifeMap.Remove(missileImpact);
        }
    }

    private void UpdateAliveMissileImpact()
    {
        for (int i = _aliveMissileImpact.Count - 1; i >= 0; i--)
        {
            GameObject missileImpact = _aliveMissileImpact[i];

            if (missileImpact == null)
            {
                _aliveMissileImpact.RemoveAt(i);

                continue;
            }

            if (!missileImpact.activeSelf)
            {
                ReturnToPool(missileImpact);
                _aliveMissileImpact.RemoveAt(i);
                RemoveLifeIfExists(missileImpact);

                CPrint.Once("킬존 리사이클", "비활성화된 MissileImpact를 다시 풀로 회수");

                continue;
            }

            if (!_lifeMap.ContainsKey(missileImpact))
            {
                CPrint.Warn($"라이프 정보 없음 : {missileImpact.name}");

                ReturnToPool(missileImpact);

                _aliveMissileImpact.RemoveAt(i);

                continue;
            }

            _lifeMap[missileImpact] -= Time.deltaTime;

            if (_lifeMap[missileImpact] < 0.0f)
            {
                ReturnToPool(missileImpact);
                _aliveMissileImpact.RemoveAt(i);
                _lifeMap.Remove(missileImpact);
            }

        }
    }

    private GameObject GetMissileImpactFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject missileImpact = _pool.Dequeue();

            return missileImpact;
        }

        GameObject extra = Instantiate(_missileImpactPrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    public GameObject SpawnImpact(Vector3 position, Quaternion rotation)
    {
        // 풀에서 이펙트 가져오기
        GameObject missileImpact = GetMissileImpactFromPool();

        // 전달받은 위치/회전값으로 설정
        missileImpact.transform.SetPositionAndRotation(position, rotation);
        missileImpact.SetActive(true);

        _aliveMissileImpact.Add(missileImpact);
        _lifeMap[missileImpact] = _missileImpactLifeTime;

        return missileImpact;
    }

}
