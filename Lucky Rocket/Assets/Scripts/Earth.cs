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
    }
}
