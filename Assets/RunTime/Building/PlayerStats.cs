using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region 인스펙터
    [Header("시작 금액")]
    [SerializeField] private int _startMoney = 400;

    #endregion

    public static int _money;

    private void Start()
    {
        _money = _startMoney;
    }

}
