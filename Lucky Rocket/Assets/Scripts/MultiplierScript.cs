using UnityEngine;

public class MultiplierScript : MonoBehaviour
{
    public float[] multipliers = { 1f, 2f, 3f, 4f, 5f, 7f, 10f};
    public string[] plus = { "2x", "3x", "4x", "5x", "7x", "10x"};

    public float activeMultiplier;
    public string activePlus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            int randomIndex = Random.Range(0, plus.Length);
            activePlus = plus[randomIndex];
        }
        else
        {
            int randomIndex = Random.Range(0, multipliers.Length);
            activeMultiplier = multipliers[randomIndex];
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Destroy(gameObject);
        }
    }
}
