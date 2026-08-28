using UnityEngine;

public class Earth : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;

    public void Start()
    {
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Obstacle") || other.CompareTag("Multiplier") || other.CompareTag("Divider") || other.CompareTag("Earth"))
        {
            Destroy(gameObject);
        }
    }
}
