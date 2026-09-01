using UnityEngine;
using UnityEngine.UI;

public class Divider : MonoBehaviour
{
    public int health;
    public Slider healthSlider;

    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();

        healthSlider.maxValue = health;
        healthSlider.value = health;

        healthSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        healthSlider.value = health;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Bullet"))
        {
            if (health > 0)
            {
                health -= 1;
            }

            healthSlider.gameObject.SetActive(true);
        }
    }
}