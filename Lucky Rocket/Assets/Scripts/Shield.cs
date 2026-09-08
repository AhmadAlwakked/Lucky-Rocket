using UnityEngine;

public class Shield : MonoBehaviour
{
    public void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            Destroy(gameObject);
        }
    }
}
