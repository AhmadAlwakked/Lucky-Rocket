using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rocket rocket;

    public void Start()
    {
        rocket = FindFirstObjectByType<Rocket>();
    }
    public void Update()
    {
        transform.Translate(Vector3.up * rocket.speed * 2 * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Multiplier") || other.CompareTag("Divider") || other.CompareTag("Earth") || other.CompareTag("Star"))
        {
            Destroy(gameObject);
        }
    }
}
