using UnityEngine;

public class Star : MonoBehaviour
{
    public Rocket rocket;

    void Start()
    {
        rocket = FindFirstObjectByType<Rocket> ();
    }

    public void Update()
    {
        float scale = 2f - (rocket.totalStarsCollected / 10f);

        transform.localScale = new Vector3(
            scale,
            scale,
            scale
        );

        if (rocket.totalStarsCollected == 10 || rocket.totalStarsCollected >= 10)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket") || other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
