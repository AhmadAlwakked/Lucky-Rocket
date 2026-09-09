using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    public Rocket rocket;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (rocket.health > 0)
            {
                rocket.health -= 1;
            }
            else
            {
                rocket.Die();
            }
        }

        if (other.CompareTag("Earth"))
        {
            rocket.cashSystem.cash += rocket.value * rocket.starMultiplier;

            rocket.Die();
        }

        if (other.CompareTag("Multiplier"))
        {
            MultiplierScript multiplierObject =
                other.GetComponent<MultiplierScript>();

            if (multiplierObject != null)
            {
                if (multiplierObject.activeMultiplier > 0)
                {
                    rocket.value +=
                        rocket.baseValue *
                        multiplierObject.activeMultiplier;
                }

                if (multiplierObject.activePlus != "")
                {
                    float multiplyAmount =
                        float.Parse(
                            multiplierObject.activePlus
                                .Replace("x", "")
                        );

                    rocket.value *= multiplyAmount;
                }
            }
        }

        if (other.CompareTag("Divider"))
        {
            rocket.value /= 2;
        }

        if (other.CompareTag("BlackHole"))
        {
            BlackHole blackHole =
                other.GetComponent<BlackHole>();

            if (blackHole != null && !rocket.inBlackHole)
            {
                rocket.EnterBlackHole(blackHole);
            }
        }

        if (other.CompareTag("Star"))
        {
            if (other.CompareTag("Star"))
            {
                if (rocket.totalStarsCollected == 0)
                {
                    rocket.starMultiplier = 1.5f;
                }
                else if (rocket.totalStarsCollected == 1)
                {
                    rocket.starMultiplier = 2f;
                }
                else if (rocket.totalStarsCollected == 2)
                {
                    rocket.starMultiplier = 3f;
                }
                else if (rocket.totalStarsCollected == 3)
                {
                    rocket.starMultiplier = 5f;
                }
                else if (rocket.totalStarsCollected == 4)
                {
                    rocket.starMultiplier = 7.5f;
                }
                else if (rocket.totalStarsCollected == 5)
                {
                    rocket.starMultiplier = 10f;
                }
                else if (rocket.totalStarsCollected == 6)
                {
                    rocket.starMultiplier = 20f;
                }
                else if (rocket.totalStarsCollected == 7)
                {
                    rocket.starMultiplier = 50f;
                }
                else if (rocket.totalStarsCollected == 8)
                {
                    rocket.starMultiplier = 100f;
                }
                else
                {
                    rocket.MaxWin();
                }

                rocket.totalStarsCollected += 1;
            }
        }

        if (other.CompareTag("Shield"))
        {
            rocket.health = 2;
        }
    }
}