using UnityEngine;
using TMPro;

public class CashSystem : MonoBehaviour
{
    public float cash;
    public Rocket rocket;

    public TMP_Text balance;
    public TMP_Text totalWin;
    public TMP_Text die;

    public void Start()
    {
        totalWin.gameObject.SetActive(false);
        die.gameObject.SetActive(false);
    }

    public void Update()
    {
        balance.text = "Balance " + cash.ToString("F2");
        totalWin.text = "Total Win: " + (rocket.value * rocket.starMultiplier).ToString("F2");

        if (Input.GetKey(KeyCode.Alpha1))
        {
            cash = 2000;
        }
    }

    public void TotalWin()
    {
        totalWin.gameObject.SetActive(true);
    }

    public void SetDieText(string message)
    {
        die.text = message;
        die.gameObject.SetActive(true);
    }
}
