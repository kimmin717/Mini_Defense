using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public enum EMovement
    {
        // 앞
        Move_FrontZ,
        // 뒤
        Move_BackZ,
        // 왼쪽
        Move_LeftX,
        // 오른쪽
        Move_RightX,
        // 정지
        Move_Wait

    }


    #region 인스펙터
    [Header("타겟")]
    [SerializeField] private Transform _target;

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] private float _moveDistanceZ = 0.0f;
    [SerializeField] private float _moveDistanceX = 0.0f;

    [Header("시작 시간")]
    [SerializeField] private float _startMoveTime = 3f;

    [Header("예제")]
    [SerializeField] private EMovement _moveing = EMovement.Move_FrontZ;
    #endregion

    #region 내부변수
    // 시작 좌표 저장
    private Vector3 _startPos;
    // 넘어갈 상태 저장
    private EMovement _nextMove;
    #endregion

    void Start()
    {
        if(_target == null)
        {
            _target = transform;
        }
    }

    
    void Update()
    {
        
    }
}
