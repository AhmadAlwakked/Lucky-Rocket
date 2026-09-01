using UnityEngine;

public class Earth : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
