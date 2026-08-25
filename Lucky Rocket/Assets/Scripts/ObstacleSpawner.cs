using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject Obstacles;
    public GameObject Multiplier;

    [Range(0, 100)]
    public int maxObstacles;

    [Range(0, 100)]
    public int maxMultipliers;

    [Space(20)]

    public float minX;
    public float maxX;

    public float minY;
    public float maxY;

    public void SpawnObjects()
    {
        for (float i = 1; i <= maxObstacles; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            Vector3 randomPosition = new Vector3(randomX, randomY, transform.position.z);

            Instantiate(Obstacles, randomPosition, transform.rotation);
        }

        for (float i = 1; i <= maxMultipliers; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            Vector3 randomPosition = new Vector3(randomX, randomY, transform.position.z);

            Instantiate(Multiplier, randomPosition, transform.rotation);
        }
    }

    public void ResetSpawnObstacles()
    {
        Debug.Log("Reset Obstacles");
    }
}
