using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    #region 인터펙스
    [Header("프리팹")]
    [SerializeField] private GameObject _enemyPrefad = null;

    [Header("스폰 위치")]
    [SerializeField] private Transform _spawnPoint = null;

    [Header("스폰 시간")]
    [SerializeField] private float _spawnTime = 2f;

    [Header("오브젝트 풀")]
    [SerializeField] private int _prewarmCount = 60;

    [Header("입력")]
    [SerializeField] private KeyCode _clearKey = KeyCode.Backspace;

    [Header("자료구조 / 수명")]
    [Min(0.1f)]
    [SerializeField] private float _lifeTime = 8.0f;
    #endregion

    #region 내부변수
    private readonly List<GameObject> _aliveEnemy = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();
    private Transform _poolRoot;
    private float _spawDelayTime = 0f;
    #endregion

    void Start()
    {
        if(_enemyPrefad == null)
        {
            CPrint.Warn("EnemyPrefad 확인 필요");

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
        if (_enemyPrefad == null)
        {
            enabled = false;
            return;
        }

        if (_spawnPoint == null)
        {
            enabled = false;
            return;
        }

        SpawnEnemy();

        UpdateAliveEnemy();

        if(Input.GetKeyDown(_clearKey))
        {
            ReturAll();
        }
    }

    private void CreatePoolRoot()
    {
        if (_poolRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("CubePool_Root");

        _poolRoot = root.transform;
    }


    private void Prewarm()
    {
        for (int i = 0; i < _prewarmCount; i++)
        {

            GameObject enemy = Instantiate(_enemyPrefad, _poolRoot);

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
        for (int i =0;i < _aliveEnemy.Count;i++)
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
        for (int i = 0; i < _aliveEnemy.Count; i++)
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

            _lifeMap[enemy] -= Time.deltaTime;

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

        GameObject extra = Instantiate(_enemyPrefad);

        CPrint.Once("풀 확장", "오브젝트를 추가 생성");

        return extra;
    }

    private void UpadateAutoSpawn()
    {
      
    }

    private void SpawnEnemy()
    { 
        GameObject enemy = GetEnemyFromPool();

        Vector3 basePos = (_spawnPoint != null) ? _spawnPoint.position : transform.position;



        // 풀 안에서는 루트 하위에 정리되어 있다.
        enemy.transform.SetParent(null);

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

        _lifeMap[enemy] = _lifeTime;

        CPrint.Log($"스폰 : {enemy.name} / Alive = {_aliveEnemy.Count} / Pool = {_pool.Count}");

    }


}
