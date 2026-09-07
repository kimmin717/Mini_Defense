using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region 인스펙터
    [Header("시작 금액")]
    [SerializeField] private int _startMoney = 400;

    [Header("시작 목숨")]
    [SerializeField] private int _startLife = 20;
    #endregion

    #region 내부변수
    public static int _money;
    public static int _life;
    #endregion
    

    private void Start()
    {
        _money = _startMoney;

        _life = _startLife;
    }

}
