using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private Transform _enemyPrefab;

    [Header("스폰")]
    [SerializeField] private Transform _spawnPoint;

    [Header("스폰 시간")]
    [SerializeField] private float _timeWaves = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _WaveCountdownText;

    // 내부 변수
    private float _countdown = 2f;
    private int _waveInedx = 1;
   
    void Update()
    {
        _countdown -= Time.deltaTime;

        if (_countdown <= 0)
        {
           StartCoroutine(SpawnWave());
            _countdown = _timeWaves;
        }

        // UI
        _WaveCountdownText.text = Mathf.CeilToInt(_countdown).ToString();
    }

    // 코루틴을 사용해 적 객체가 겹처서 스폰되는 것을 딜레이 타입을 주어 해결 
    // 나중가서 다시 겹쳐서 스폰 되니 수정 해주기
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < _waveInedx; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }

        // 적 객체가 순차적으로 더많이 생성 됨
        _waveInedx++;
    }

    private void SpawnEnemy()
    {
        Instantiate(_enemyPrefab, _spawnPoint.position, Quaternion.identity);
    }

}
