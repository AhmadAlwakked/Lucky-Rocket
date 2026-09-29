using UnityEngine;
using System.Threading.Tasks;

public class SpeedBoost : MonoBehaviour
{
    public async Task OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket") || other.CompareTag("Bullet"))
        {
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
