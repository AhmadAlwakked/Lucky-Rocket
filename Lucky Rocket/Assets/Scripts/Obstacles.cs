using UnityEngine;
using System.Collections.Generic;

public class Obstacles : MonoBehaviour
{
    public int health;
    public UnityEngine.UI.Slider healthSlider;

    [Header("Rotation")]
    public float minRotationSpeed = 10f;
    public float maxRotationSpeed = 50f;

    private float rotationSpeed;

    void Start()
    {
        // Random beginrotatie op de Y-as
        transform.rotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f
        );

        // Elke obstacle krijgt een eigen random snelheid
        rotationSpeed = Random.Range(
            minRotationSpeed,
            maxRotationSpeed
        );

        healthSlider = GetComponentInChildren<UnityEngine.UI.Slider>();

        healthSlider.maxValue = health;
        healthSlider.value = health;

        healthSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        healthSlider.value = health;

        // Alleen draaien om de Y-as
        transform.Rotate(
            0f,
            rotationSpeed * Time.deltaTime,
            0f
        );

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