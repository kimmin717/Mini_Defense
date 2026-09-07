using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("돈 UI")]
    [SerializeField] private TextMeshProUGUI _moneyText;

    [Header("목숨 UI")]
    [SerializeField] private TextMeshProUGUI _lifeText;

    private void Update()
    {
        //_moneyText.text = "$" + PlayerStats._money.ToString();

        _moneyText.text = $"$ {PlayerStats._money.ToString()}";

        _lifeText.text = $"LifePoint : {PlayerStats._life.ToString()}";
    }
}
