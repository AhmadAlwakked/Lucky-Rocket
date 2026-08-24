using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Internal;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject Obstacles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObstacles();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnObstacles()
    {
        Instantiate(Obstacles, transform.position, transform.rotation);
    }
}
