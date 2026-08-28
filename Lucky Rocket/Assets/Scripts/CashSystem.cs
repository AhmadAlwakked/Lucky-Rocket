using UnityEngine;
using TMPro;

public class CashSystem : MonoBehaviour
{
    public float cash;

    public TMP_Text Balance;

    public void Update()
    {
        Balance.text = "Balance " + cash.ToString("F2");
    }
}
