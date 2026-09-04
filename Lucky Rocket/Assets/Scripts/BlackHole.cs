using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float size;
    public float minSize;
    public float maxSize;

    [Space]

    public GameObject divider;
    public GameObject multiplier;

    private Rigidbody rb;

    private GameObject spawnedMultiplier;
    private GameObject spawnedDivider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        size = Random.Range(minSize, maxSize);

        rb.mass = size * size;

        transform.localScale = new Vector3(size, size, size);

        if (Random.Range(0, 4) == 0)
        {
            spawnedMultiplier = Spawn(multiplier);
        }
        else
        {
            spawnedDivider = Spawn(divider);
        }
    }

    public GameObject Spawn(GameObject prefab)
    {
        Vector3 position = transform.position;

        GameObject spawnedObject = Instantiate(
            prefab,
            position,
            transform.rotation,
            transform
        );

        spawnedObject.transform.localScale = Vector3.one / size;

        return spawnedObject;
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
        // Als de multiplier bestaat, trekt de black hole.
        if (spawnedMultiplier != null)
        {
            return true;
        }

        // Als de divider bestaat, trekt de black hole.
        if (spawnedDivider != null)
        {
            return true;
        }

        // Geen multiplier/divider meer = geen aantrekkingskracht.
        return false;
    }
}