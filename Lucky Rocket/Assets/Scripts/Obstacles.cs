using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;

    public void Start()
    {
        obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Multiplier") || other.CompareTag("Divider") || other.CompareTag("Earth"))
        {
            Destroy(gameObject);
        }
    }
}
