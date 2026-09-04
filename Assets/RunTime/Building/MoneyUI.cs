using System.Collections;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [Header("돈 UI")]
    [SerializeField] private TextMeshProUGUI _moneyText;

    private void Update()
    {
        _moneyText.text = "$" + PlayerStats._money.ToString();
    }
}
