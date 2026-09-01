using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    public void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Multiplier") || other.CompareTag("Divider") || other.CompareTag("Earth"))
        {
            Destroy(gameObject);
        }
    }
}
