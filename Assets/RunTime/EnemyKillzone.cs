using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyKillzone : MonoBehaviour
{
    #region 인스펙터
    [Header("트리거 토글")]
    [SerializeField] private KeyCode _togglekey = KeyCode.K;
    [SerializeField] private bool _startEnabled = true;

    [Header("필터")]
    [SerializeField] private bool _useTagFilter = true;
    [SerializeField] private string _targetTag = "PoolEnemy";

    [Header("로그")]
    [SerializeField] private bool _printLog = true;
    #endregion

    #region 내부변수
    private BoxCollider _targetCollider;
    #endregion

    private void Awake()
    {
        _targetCollider = GetComponent<BoxCollider>();

        if( _targetCollider == null )
        {
            CPrint.Warn("BoxCollider 컴포넌트를 찾을 수 없습니다. 확인 필요");

            enabled = false;
            return;
        }

        _targetCollider.isTrigger = true;
        _targetCollider.enabled = _startEnabled;
    }

    void Start()
    {
        CPrint.Title("킬존 트리거");

        CPrint.KV("트리거", _targetCollider.enabled);
        CPrint.Line();
    }

    
    void Update()
    {
        if(Input.GetKeyDown(_togglekey))
        {
            ToggleKillzone();
        }
    }

    private void ToggleKillzone()
    {
        if(_targetCollider == null)
        {
            CPrint.Warn("BoxCollider 컴포넌트를 찾을 수 없습니다. 확인 필요");

            return;
        }

        _targetCollider.enabled = !_targetCollider.enabled;

        if(_printLog)
        {
            if(_targetCollider.enabled)
            {
                CPrint.Success("킬존 ON");
            }

            else
            {
                CPrint.Warn("킬존 OFF");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == null)
        {
            return;
        }

        if (_useTagFilter)
        {
            if (string.IsNullOrEmpty(_targetTag))
            {
                CPrint.Once("태그 없음", "대상 태그가 비어 있음");

                return;
            }

            if (!other.CompareTag(_targetTag))
            {
                CPrint.Once("태그 다름", "대상 태그가 다름");

                return;
            }
            
        }

        if(_printLog)
        {
            CPrint.Group("킬존 진입", () =>
            {
                CPrint.KV("대상", other.name);
                CPrint.KV("태그", other.tag);
                CPrint.KV("위치", other.transform.position);
            
            });
        }

        other.gameObject.SetActive(false);
    }
}
