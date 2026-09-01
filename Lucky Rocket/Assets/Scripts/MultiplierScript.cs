using UnityEngine;
using TMPro;

public class MultiplierScript : MonoBehaviour
{
    public float[] multipliers = { 1f, 2f, 3f, 4f, 5f, 7f, 10f };
    public float[] multiplierChances = { 50f, 20f, 12f, 8f, 5f, 3f, 2f };

    public string[] plus = { "2x", "3x", "4x", "5x", "7x", "10x" };
    public float[] plusChances = { 40f, 25f, 15f, 10f, 7f, 3f };

    public float activeMultiplier;
    public string activePlus;
    public TMP_Text multiplierText;

    void Start()
    {
        RandomMultiplier();
    }

    public void RandomMultiplier()
    {
        activeMultiplier = 0;
        activePlus = "";

        if (Random.Range(0, 4) == 0)
        {
            int randomIndex = GetWeightedIndex(plusChances);
            activePlus = plus[randomIndex];

            if (activePlus == "1x")
            {
                activePlus = "";
                activeMultiplier = 1f;
            }
        }
        else
        {
            int randomIndex = GetWeightedIndex(multiplierChances);
            activeMultiplier = multipliers[randomIndex];
        }

        if (activePlus != "")
        {
            multiplierText.text = activePlus;
        }
        else
        {
            multiplierText.text = activeMultiplier.ToString("0");
        }
    }

    int GetWeightedIndex(float[] chances)
    {
        float total = 0f;

        foreach (float chance in chances)
        {
            total += chance;
        }

        float randomValue = Random.Range(0f, total);

        for (int i = 0; i < chances.Length; i++)
        {
            randomValue -= chances[i];

            if (randomValue <= 0)
            {
                return i;
            }
        }

        return chances.Length - 1;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Bullet"))
            Destroy(gameObject);
    }
}