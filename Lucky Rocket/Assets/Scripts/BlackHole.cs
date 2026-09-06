using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("Black Hole")]
    public float size;

    [Space]

    public GameObject divider;
    public GameObject multiplier;

    [Header("Spawn")]
    public int minObjects = 2;
    public int maxObjects = 5;

    [Header("Orbit")]
    public float rotationSpeed = 100f;

    [Range(0.1f, 0.9f)]
    public float minOrbitRadius = 0.25f;

    [Range(0.1f, 0.9f)]
    public float maxOrbitRadius = 0.85f;

    [Header("Object Size")]
    public float minObjectScale = 0.5f;
    public float maxObjectScale = 1.2f;

    [Range(0f, 0.3f)]
    public float sizeRandomness = 0.1f;

    private Rigidbody rb;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private List<float> orbitRadii = new List<float>();
    private List<float> orbitAngles = new List<float>();
    private List<float> orbitSpeeds = new List<float>();

    private bool sizeInitialized = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (!sizeInitialized)
        {
            Debug.LogError("BlackHole size is not initialized by ObstacleSpawner!");
            return;
        }

        rb.mass = size * size;

        transform.localScale = new Vector3(size, size, size);

        SpawnObjects();
    }


    public void InitializeSize(float newSize)
    {
        size = newSize;
        sizeInitialized = true;
    }


    void Update()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            // Object is verwijderd
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                orbitRadii.RemoveAt(i);
                orbitAngles.RemoveAt(i);
                orbitSpeeds.RemoveAt(i);

                continue;
            }

            RotateObject(i);
        }
    }


    void SpawnObjects()
    {
        // Hoe groter de BlackHole, hoe meer objecten.
        // Hoe kleiner de BlackHole, hoe minder objecten.
        int amount = Mathf.RoundToInt(
            Mathf.Lerp(
                minObjects,
                maxObjects,
                Mathf.InverseLerp(
                    0f,
                    10f,
                    size
                )
            )
        );

        for (int i = 0; i < amount; i++)
        {
            SpawnObject();
        }
    }


    void SpawnObject()
    {
        // Kies willekeurig Multiplier of Divider
        GameObject prefab;

        if (Random.Range(0, 2) == 0)
        {
            prefab = multiplier;
        }
        else
        {
            prefab = divider;
        }

        if (prefab == null)
        {
            return;
        }


        // Willekeurige afstand vanaf het midden
        float blackHoleRadius = size / 2f;

        float radius = Random.Range(
            blackHoleRadius * minOrbitRadius,
            blackHoleRadius * maxOrbitRadius
        );


        // Willekeurige beginhoek
        float angle = Random.Range(0f, 360f);


        // Object maken
        GameObject spawnedObject = Instantiate(
            prefab,
            transform.position,
            Quaternion.identity,
            transform
        );


        // Box Collider uitschakelen
        BoxCollider boxCollider =
            spawnedObject.GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }


        // Positie berekenen
        float radians = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius,
            0f
        );

        spawnedObject.transform.position =
            transform.position + offset;


        // -------------------------
        // GROOTTE BEREKENEN
        // -------------------------

        // 0 = midden
        // 1 = buitenkant

        float normalizedDistance =
            Mathf.InverseLerp(
                blackHoleRadius * minOrbitRadius,
                blackHoleRadius * maxOrbitRadius,
                radius
            );


        // --------------------------------
        // OBJECT SIZE AFHANKELIJK VAN BH SIZE
        // --------------------------------

        // Kleine BlackHole = kleinere objecten
        // Grote BlackHole = grotere objecten

        float sizeMultiplier =
            Mathf.InverseLerp(
                2f,
                5f,
                size
            );

        float currentMinObjectScale =
            Mathf.Lerp(
                minObjectScale * 0.5f,
                minObjectScale * 1.5f,
                sizeMultiplier
            );

        float currentMaxObjectScale =
            Mathf.Lerp(
                maxObjectScale * 0.5f,
                maxObjectScale * 1.5f,
                sizeMultiplier
            );


        // Dichter bij midden = kleiner
        float objectScale = Mathf.Lerp(
            currentMinObjectScale,
            currentMaxObjectScale,
            normalizedDistance
        );


        // Kleine willekeurige afwijking
        float randomScale = Random.Range(
            1f - sizeRandomness,
            1f + sizeRandomness
        );

        objectScale *= randomScale;


        // Schaal corrigeren voor grootte van BlackHole
        spawnedObject.transform.localScale =
            Vector3.one / size * objectScale;


        // -------------------------
        // ORBIT DATA OPSLAAN
        // -------------------------

        spawnedObjects.Add(spawnedObject);

        orbitRadii.Add(radius);
        orbitAngles.Add(angle);

        // Kleine verschillen in snelheid
        orbitSpeeds.Add(
            rotationSpeed * Random.Range(0.8f, 1.2f)
        );
    }


    void RotateObject(int index)
    {
        GameObject objectToRotate = spawnedObjects[index];

        // Hoek aanpassen
        orbitAngles[index] +=
            orbitSpeeds[index] * Time.deltaTime;


        float radians =
            orbitAngles[index] * Mathf.Deg2Rad;


        float radius =
            orbitRadii[index];


        // Nieuwe positie
        Vector3 offset = new Vector3(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius,
            0f
        );


        objectToRotate.transform.position =
            transform.position + offset;


        // Object zelf draait NIET
        objectToRotate.transform.rotation =
            Quaternion.identity;
    }


    public float GetMass()
    {
        if (rb != null)
        {
            return rb.mass;
        }

        return size * size;
    }


    public bool IsAttracting()
    {
        // Zolang er minimaal één multiplier/divider bestaat,
        // blijft de black hole aantrekken.

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
            {
                return true;
            }
        }

        return false;
    }
}