using UnityEngine;
using System.Threading.Tasks;

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

    public async Task OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket") || other.CompareTag("Bullet"))
        {
            BoxCollider box = other.GetComponent<BoxCollider>();

            box.isTrigger = false;

            Vector3 collectPosition = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);

            while (Vector3.Distance(transform.position, collectPosition) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, collectPosition, 0.1f);

                await Task.Yield();
            }

            Destroy(gameObject);
        }
    }
}
