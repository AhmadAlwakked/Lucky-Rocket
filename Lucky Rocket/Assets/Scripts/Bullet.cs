using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rocket rocket;

    public bool isBlackHoleBullet;

    private void Start()
    {
        rocket = FindFirstObjectByType<Rocket>();

        // Black Hole bullet wordt kleiner
        if (isBlackHoleBullet)
        {
            transform.localScale = Vector3.one * 0.05f;
        }
    }

    private void Update()
    {
        float bulletSpeed;

        if (isBlackHoleBullet)
        {
            bulletSpeed = rocket.blackHoleRocketSpeed * 3f;
        }
        else
        {
            bulletSpeed = rocket.speed * 2f;
        }

        transform.Translate(
            Vector3.up * bulletSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") ||
            other.CompareTag("Multiplier") ||
            other.CompareTag("Divider") ||
            other.CompareTag("Earth") ||
            other.CompareTag("Star"))
        {
            Destroy(gameObject);
        }
    }
}