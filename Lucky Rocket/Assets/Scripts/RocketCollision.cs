using System;
using System.Collections;
using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    public Rocket rocket;
    public CashSystem cashSystem;

    public bool isFuelTank;

    public AudioSource CollectPlus;
    public AudioSource CollectDivider;
    public AudioSource CollectSpeedBoost;
    public AudioSource CollectStar;
    public AudioSource WinCash;
    public AudioSource Crash;
    public AudioSource CollectShield;

    private void OnTriggerEnter(Collider other)
    {
        // Losgekoppelde fuel tank doet helemaal niets meer.
        if (isFuelTank && rocket.loseFuel)
            return;

        if (other.CompareTag("Obstacle"))
        {
            // Tijdens SpeedBoost kan de rocket niet sterven
            if (rocket.speedBoostInvincible)
                return;

            Crash.Play();

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
            Debug.Log(
                "cash + " +
                rocket.value +
                " x " +
                rocket.starMultiplier +
                " = " +
                rocket.value * rocket.starMultiplier
            );

            rocket.cashSystem.cash +=
                rocket.value * rocket.starMultiplier;

            WinCash.Play();

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

                CollectPlus.Play();
            }
        }

        if (other.CompareTag("Divider"))
        {
            rocket.value /= 2;
            CollectDivider.Play();
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

            CollectStar.Play();
        }

        if (other.CompareTag("Shield"))
        {
            CollectShield.Play();

            rocket.health += 1;
        }

        if (other.CompareTag("SpeedBoost"))
        {
            rocket.SpeedBoost();
            CollectSpeedBoost.Play();
        }
    }

    public IEnumerator OnTriggerExit(Collider other)
    {
        // Losgekoppelde fuel tank doet ook bij TriggerExit niets.
        if (isFuelTank && rocket.loseFuel)
            yield break;

        if (rocket.currentBlackHole != null)
        {
            if (
                other.CompareTag("BlackHole") &&
                rocket.transform.localScale.x ==
                rocket.blackHoleMiniGameScale
            )
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