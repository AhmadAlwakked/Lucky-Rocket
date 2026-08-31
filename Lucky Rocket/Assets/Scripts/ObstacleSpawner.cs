using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstacles;
    public GameObject multiplier;
    public GameObject divider;
    public GameObject earth;

    public Transform parentTransform;

    [Space]


    [Range(0, 100)]
    public int maxObstacles;

    [Range(0, 100)]
    public int maxMultipliers;

    [Range(0, 100)]
    public int maxDividers;

    [Range(0, 100)]
    public int maxEarth = 1;

    [Space]

    public float squareWidth = 20f;
    public float squareHeight = 20f;
    public float minSpawnHeight = 0f;

    [Space]

    [Range(0, 5)]
    public int squaresLeftRight = 1;

    [Range(1, 5)]
    public int spawnAheadSquares = 2;

    [Range(0, 5)]
    public int squaresBehind = 1;

    private Rocket rocket;

    private HashSet<Vector2Int> spawnedSquares = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, List<GameObject>> squareObjects =
        new Dictionary<Vector2Int, List<GameObject>>();

    void Start()
    {
        rocket = FindFirstObjectByType<Rocket>();
    }

    void Update()
    {
        if (rocket == null || !rocket.isLaunching)
            return;

        SpawnAroundRocket();
        RemoveSquaresBehindRocket();
        CheckPrefabCollisions();
        CheckPrefabCollisions();
    }

    public void SpawnObjects()
    {
        spawnedSquares.Clear();
        squareObjects.Clear();

        SpawnAroundRocket();
    }

    void SpawnAroundRocket()
    {
        int rocketXSquare = Mathf.FloorToInt(
            rocket.transform.position.x / squareWidth
        );

        int rocketYSquare = Mathf.FloorToInt(
            (rocket.transform.position.y - minSpawnHeight) / squareHeight
        );

        for (int y = 0; y <= spawnAheadSquares; y++)
        {
            int squareY = rocketYSquare + y;

            if (squareY < 0)
                continue;

            for (int x = -squaresLeftRight; x <= squaresLeftRight; x++)
            {
                int squareX = rocketXSquare + x;

                SpawnSquareIfNeeded(squareX, squareY);
            }
        }
    }

    void SpawnSquareIfNeeded(int squareX, int squareY)
    {
        Vector2Int square = new Vector2Int(squareX, squareY);

        if (spawnedSquares.Contains(square))
            return;

        spawnedSquares.Add(square);
        squareObjects.Add(square, new List<GameObject>());

        float minX = squareX * squareWidth - squareWidth / 2f;
        float maxX = squareX * squareWidth + squareWidth / 2f;

        float minY = minSpawnHeight + squareY * squareHeight;
        float maxY = minY + squareHeight;

        SpawnSquare(square, minX, maxX, minY, maxY);
    }

    void SpawnSquare(
        Vector2Int square,
        float minX,
        float maxX,
        float minY,
        float maxY
    )
    {
        for (int i = 0; i < maxObstacles; i++)
            Spawn(obstacles, square, minX, maxX, minY, maxY);

        for (int i = 0; i < maxMultipliers; i++)
            Spawn(multiplier, square, minX, maxX, minY, maxY);

        for (int i = 0; i < maxDividers; i++)
            Spawn(divider, square, minX, maxX, minY, maxY);

        if (Random.Range(0, 4) == 0)
        {
            for (int i = 0; i < maxEarth; i++)
            {
                Spawn(earth, square, minX, maxX, minY, maxY);
            }
        }
    }

    public void Spawn(
        GameObject prefab,
        Vector2Int square,
        float minX,
        float maxX,
        float minY,
        float maxY
    )
    {
        Vector3 position = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            transform.position.z
        );

        GameObject spawnedObject = Instantiate(
            prefab,
            position,
            transform.rotation,
            parentTransform
        );

        squareObjects[square].Add(spawnedObject);
    }

    public void ReplaceObject(GameObject objectToReplace)
    {
        foreach (KeyValuePair<Vector2Int, List<GameObject>> pair in squareObjects)
        {
            if (pair.Value.Contains(objectToReplace))
            {
                Vector2Int square = pair.Key;

                float minX = square.x * squareWidth - squareWidth / 2f;
                float maxX = square.x * squareWidth + squareWidth / 2f;

                float minY = minSpawnHeight + square.y * squareHeight;
                float maxY = minY + squareHeight;

                GameObject prefab = null;

                if (objectToReplace.CompareTag("Obstacle"))
                    prefab = obstacles;
                else if (objectToReplace.CompareTag("Multiplier"))
                    prefab = multiplier;
                else if (objectToReplace.CompareTag("Divider"))
                    prefab = divider;
                else if (objectToReplace.CompareTag("Earth"))
                    prefab = earth;

                if (prefab == null)
                    return;

                pair.Value.Remove(objectToReplace);

                Destroy(objectToReplace);

                Spawn(
                    prefab,
                    square,
                    minX,
                    maxX,
                    minY,
                    maxY
                );

                return;
            }
        }
    }

    void CheckPrefabCollisions()
    {
        foreach (KeyValuePair<Vector2Int, List<GameObject>> pair in squareObjects)
        {
            List<GameObject> objects = pair.Value;

            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = objects[i];

                if (obj == null)
                    continue;

                Collider collider = obj.GetComponent<Collider>();

                if (collider == null)
                    continue;

                Collider[] hits = Physics.OverlapBox(
                    collider.bounds.center,
                    collider.bounds.extents,
                    obj.transform.rotation
                );

                foreach (Collider hit in hits)
                {
                    if (hit.gameObject == obj)
                        continue;

                    if (hit.CompareTag("Obstacle") ||
                        hit.CompareTag("Multiplier") ||
                        hit.CompareTag("Divider") ||
                        hit.CompareTag("Earth"))
                    {
                        ReplaceObject(hit.gameObject);
                        return;
                    }
                }
            }
        }
    }

    void RemoveSquaresBehindRocket()
    {
        int rocketYSquare = Mathf.FloorToInt(
            (rocket.transform.position.y - minSpawnHeight) / squareHeight
        );

        int removeBefore = rocketYSquare - squaresBehind;

        List<Vector2Int> squaresToRemove = new List<Vector2Int>();

        foreach (Vector2Int square in spawnedSquares)
        {
            if (square.y < removeBefore)
            {
                squaresToRemove.Add(square);
            }
        }

        foreach (Vector2Int square in squaresToRemove)
        {
            foreach (GameObject obj in squareObjects[square])
            {
                if (obj != null)
                    Destroy(obj);
            }

            squareObjects.Remove(square);
            spawnedSquares.Remove(square);
        }
    }

    public void ResetSpawnObstacles()
    {
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
            Destroy(parentTransform.GetChild(i).gameObject);

        spawnedSquares.Clear();
        squareObjects.Clear();
    }
}