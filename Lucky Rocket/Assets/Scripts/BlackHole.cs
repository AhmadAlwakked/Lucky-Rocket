using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float size;
    public float minSize;
    public float maxSize;

    void Start()
    {
        size = Random.Range(minSize, maxSize);

        transform.localScale = new Vector3(size, size, size);
    }
}
