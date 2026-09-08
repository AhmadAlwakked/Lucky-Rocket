using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstacles;
    public GameObject multiplier;
    public GameObject divider;
    public GameObject earth;
    public GameObject blackHole;
    public GameObject stars;
    public GameObject shield;

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

    [Range(0, 100)]
    public int maxBlackHoles;

    [Range(0, 100)]
    public int maxStars;

    [Range(0, 100)]
    public int maxShields;

    [Space]

    [Header("Black Hole Size")]
    public float minBlackHoleSize = 2f;
    public float maxBlackHoleSize = 5f;

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

        if (Random.Range(0, 5) == 0)
        {
            for (int i = 0; i < maxEarth; i++)
            {
                Spawn(earth, square, minX, maxX, minY, maxY);
            }
        }

        if (Random.Range(0, 4) == 0)
        {
            for (int i = 0; i < maxBlackHoles; i++)
            {
                SpawnBlackHole(square, minX, maxX, minY, maxY);
            }
        }

        if (Random.Range(0, 10) == 0 && rocket.totalStarsCollected < 10)
        {
            for (int i = 0; i < maxStars; i++)
            {
                Spawn(stars, square, minX, maxX, minY, maxY);
            }
        }

        if (Random.Range(0, 10) == 0)
        {
            for (int i = 0; i < maxShields; i++)
            {
                Spawn(shield, square, minX, maxX, minY, maxY);
            }
        }
    }

    void SpawnBlackHole(
        Vector2Int square,
        float minX,
        float maxX,
        float minY,
        float maxY
    )
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 position = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                transform.position.z
            );

            GameObject spawnedObject = Instantiate(
                blackHole,
                position,
                transform.rotation,
                parentTransform
            );

            // De grootte van de BlackHole wordt hier door de Spawner bepaald.
            float blackHoleSize = Random.Range(
                minBlackHoleSize,
                maxBlackHoleSize
            );

            BlackHole blackHoleScript =
                spawnedObject.GetComponent<BlackHole>();

            if (blackHoleScript != null)
            {
                blackHoleScript.InitializeSize(blackHoleSize);
            }

            // --------------------------------
            // BLACK HOLE SCHAAL DIRECT INSTELLEN
            // --------------------------------

            spawnedObject.transform.localScale =
                new Vector3(
                    blackHoleSize,
                    blackHoleSize,
                    blackHoleSize
                );

            // Unity direct de nieuwe collider-grootte laten bijwerken
            Physics.SyncTransforms();

            Collider newCollider =
                spawnedObject.GetComponent<Collider>();

            if (newCollider == null)
            {
                squareObjects[square].Add(spawnedObject);
                return;
            }

            Collider[] hits = Physics.OverlapBox(
                newCollider.bounds.center,
                newCollider.bounds.extents,
                Quaternion.identity,
                Physics.AllLayers,
                QueryTriggerInteraction.Collide
            );

            bool touchingObject = false;

            foreach (Collider hit in hits)
            {
                if (hit.gameObject != spawnedObject &&
                    (hit.CompareTag("Obstacle") ||
                     hit.CompareTag("Multiplier") ||
                     hit.CompareTag("Divider") ||
                     hit.CompareTag("Earth") ||
                     hit.CompareTag("BlackHole") ||
                     hit.CompareTag("Star") ||
                     hit.CompareTag("Shield")))
                {
                    touchingObject = true;
                    break;
                }
            }

            // --------------------------------
            // GEEN OBJECT RAAKT BLACK HOLE
            // --------------------------------

            if (!touchingObject)
            {
                squareObjects[square].Add(spawnedObject);
                return;
            }

            // --------------------------------
            // OBJECT RAAKT BLACK HOLE
            // DUS VERWIJDEREN EN OPNIEUW PROBEREN
            // --------------------------------

            Destroy(spawnedObject);
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
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 position = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                transform.position.z
            );

            GameObject spawnedObject = Instantiate(
                prefab,
                position,
                prefab.transform.rotation,
                parentTransform
            );

            Collider newCollider = spawnedObject.GetComponent<Collider>();

            if (newCollider == null)
            {
                squareObjects[square].Add(spawnedObject);
                return;
            }

            Collider[] hits = Physics.OverlapBox(
                newCollider.bounds.center,
                newCollider.bounds.extents
            );

            bool touchingObject = false;

            foreach (Collider hit in hits)
            {
                if (hit.gameObject != spawnedObject &&
                    (hit.CompareTag("Obstacle") ||
                     hit.CompareTag("Multiplier") ||
                     hit.CompareTag("Divider") ||
                     hit.CompareTag("Earth") ||
                     hit.CompareTag("BlackHole") ||
                     hit.CompareTag("Star") ||
                     hit.CompareTag("SHields")))
                {
                    touchingObject = true;
                    break;
                }
            }

            if (!touchingObject)
            {
                squareObjects[square].Add(spawnedObject);
                return;
            }

            Destroy(spawnedObject);
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