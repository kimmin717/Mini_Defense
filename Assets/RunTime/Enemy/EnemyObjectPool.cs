using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    #region 인터펙스 (오브젝트 풀)
    [Header("프리팹")]
    [SerializeField] private GameObject _enemyPrefab = null;

    [Header("스폰 위치")]
    [SerializeField] private Transform _spawnPoint = null;

    [Header("스폰 시간")]
    [SerializeField] private float _spawnTime = 5f;

    [Header("오브젝트 풀")]
    [SerializeField] private int _prewarmCount = 60;

    [Header("입력")]
    [SerializeField] private KeyCode _clearKey = KeyCode.Backspace;

    [Header("체력 설정(수명)")]
    [Min(0.1f)]
    [SerializeField] private float _enemyHP = 10.0f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _WaveCountdownText;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveEnemy = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;

    private float _spawnDelayTime = 2f;
    private int _waveIndex = 1;
    #endregion

    void Start()
    {
        if(_enemyPrefab == null)
        {
            CPrint.Warn("EnemyPrefad 인스펙터 확인 필요");

            enabled = false;
            return;
        }

        if(_spawnPoint == null)
        {
            CPrint.Warn("SpawnPoint 확인 필요");

            enabled = false;
            return;
        }

        CreatePoolRoot();
        Prewarm();
    }

    void Update()
    {
        if (Input.GetKeyDown(_clearKey))
        {
            ReturAll();
        }

        _spawnDelayTime -= Time.deltaTime;

        if (_spawnDelayTime <= 0)
        {
            StartCoroutine(SpawnWave());
            _spawnDelayTime = _spawnTime;
        }

        // null 예외 방지
        if (_WaveCountdownText != null)
        {
            // UI 
            _WaveCountdownText.text = Mathf.CeilToInt(_spawnDelayTime).ToString();
        }

        UpdateAliveEnemy();
  
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < _waveIndex; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }

        // 적 객체가 순차적으로 더많이 생성 됨
        _waveIndex++;
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("EnemyPool_Root");

        _poolRoot = root.transform;
    }


    private void Prewarm()
    {
        for (int i = 0; i < _prewarmCount; i++)
        {

            GameObject enemy = Instantiate(_enemyPrefab, _poolRoot);

            enemy.SetActive(false);

            _pool.Enqueue(enemy);
        }

        CPrint.Success($"Prewarm = {_prewarmCount}");

    }

    private void ReturnToPool(GameObject enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SetActive(false);

        enemy.transform.SetParent(_poolRoot);

        _pool.Enqueue(enemy);
    }

    private void ReturAll()
    {
        for (int i = _aliveEnemy.Count - 1; i >=0; i--)
        {
            GameObject enemy = _aliveEnemy[i];

            if (enemy == null)
            {  
                continue; 
            }

            ReturnToPool(enemy);
        }

        _aliveEnemy.Clear();
        _lifeMap.Clear();

        CPrint.Success($"전체 enemy 반환 / Pool = {_pool.Count}");

    }

    private void RemoveLifeIfExists(GameObject enemy)
    { 
        if(enemy == null)
        { 
            return; 
        }

        if(_lifeMap.ContainsKey(enemy))
        {
            _lifeMap.Remove(enemy);
        }
    }

    private void UpdateAliveEnemy()
    {
        for (int i = _aliveEnemy.Count - 1; i >= 0; i--)
        {
            GameObject enemy = _aliveEnemy[i];

            if (enemy == null)
            {
                _aliveEnemy.RemoveAt(i);

                continue;
            }

            if (!enemy.activeSelf)
            {
                ReturnToPool(enemy);
                _aliveEnemy.RemoveAt(i);
                RemoveLifeIfExists(enemy);

                CPrint.Once("킬존 리사이클", "비활성화된 Enemy를 다시 풀로 회수");

                continue;
            }

            if(!_lifeMap.ContainsKey(enemy))
            {
                CPrint.Warn($"라이프 정보 없음 : {enemy.name}");

                ReturnToPool(enemy);

                _aliveEnemy.RemoveAt(i);

                continue;
            }

            // 이부분 체력 감소 로직으로 추가하면
            //_lifeMap[enemy] -= Time.deltaTime;

            if (_lifeMap[enemy] < 0.0f)
            {
                ReturnToPool(enemy);
                _aliveEnemy.RemoveAt(i);
                _lifeMap.Remove(enemy);
            }

        }
    }

    private GameObject GetEnemyFromPool()
    { 
        if (_pool.Count > 0)
        {
            GameObject enemy = _pool.Dequeue();

            return enemy;
        }

        GameObject extra = Instantiate(_enemyPrefab);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    private void SpawnEnemy()
    { 
        GameObject enemy = GetEnemyFromPool();

        // 풀 안에서는 루트 하위에 정리되어 있다.
        enemy.transform.SetParent(null);
        enemy.transform.SetPositionAndRotation( _spawnPoint.position, Quaternion.identity);

        // 풀에서 꺼낸 객체를 다시 사용
        enemy.SetActive(true);
  
        if (!_aliveEnemy.Contains(enemy))
        {
            _aliveEnemy.Add(enemy);
        }

        else
        {
            CPrint.Warn($"중복 스폰 감지 : {enemy.name}");
        }

        _lifeMap[enemy] = _enemyHP;

        CPrint.Log($"스폰 : {enemy.name} / Alive = {_aliveEnemy.Count} / Pool = {_pool.Count}");

    }


}
