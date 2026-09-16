using System.Collections;
using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    public Rocket rocket;
    public CashSystem cashSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (rocket.health > 0)
            {
                rocket.health -= 1;

                if (rocket.health <= 0)
                {
                    rocket.cashSystem.SetDieText("You Crashed");

                    StartCoroutine(rocket.Die());
                }
            }
        }

        if (other.CompareTag("Earth"))
        {
            Debug.Log("cash + " + rocket.value + " x " + rocket.starMultiplier + " = " + rocket.value * rocket.starMultiplier);

            rocket.cashSystem.cash += rocket.value * rocket.starMultiplier;

            cashSystem.TotalWin();

            StartCoroutine(rocket.WinDie());
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

        if (other.CompareTag("Shield"))
        {
            rocket.health = 2;
        }
    }
    
    public IEnumerator OnTriggerExit(Collider other)
    {
        if (rocket.currentBlackHole != null)
        {
            if (other.CompareTag("BlackHole") && rocket.transform.localScale.x == rocket.blackHoleMiniGameScale)
            {
                yield return new WaitForSecondsRealtime(0.25f);

                if (rocket.inBlackHole)
                {
                    rocket.ExitBlackHole();
                }
            }
        }
    }
}