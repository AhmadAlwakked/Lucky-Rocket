using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Type")]
    public bool isBasic;
    public bool isShuttle;
    public bool isJetFighter;
    public bool loseFuel;

    [Header("Launch")]
    public bool isLaunching;
    public float MaxLaunch;
    public float launchSpeed = 8f;
    public float speed;
    public int turnSpeed;
    public float height;

    [Header("Speed Increase")]
    public float speedIncrease = 0.1f;
    public float speedIncreaseGrowth = 0.1f;
    private float speedTimer;

    [Header("Black Hole")]
    public float blackHolePullSpeed;
    public float blackHoleMaxPullSpeed;
    public float blackHolePullStrength;
    private float blackHoleVelocityX;

    private BlackHole currentBlackHole;

    [Space]

    public float baseValue;
    public float value;
    public float multiplier = 1f;

    [Space]

    public float shootCooldown;
    private float shootTimer;

    [Space]

    public GameObject camera;
    public ObstacleSpawner obstacleSpawner;
    public CashSystem cashSystem;
    public GameObject bullet;

    private float divisionTimer;

    public TMP_Text Cash;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLaunching = false;

        transform.position = new Vector3(0, -5, 0);

        value = baseValue * multiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isLaunching == false))
        {
            if (cashSystem.cash >= baseValue)
            {
                Launch();
                cashSystem.cash -= baseValue;
            }
            else
            {
                Debug.Log("Not Enough Cash");
            }
        }

        if (isLaunching)
        {
            float currentSpeed = transform.position.y < MaxLaunch ? launchSpeed : speed;

            transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);

            if (transform.position.y >= 0)
            {
                speedTimer += Time.deltaTime;

                if (speedTimer >= 5f)
                {
                    speed += speedIncrease;

                    speedIncrease += speedIncreaseGrowth;

                    speedTimer = 0f;
                }
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(Vector3.forward * turnSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.back * turnSpeed * Time.deltaTime);
            }

            Vector3 rotation = transform.eulerAngles;
            rotation.z = Mathf.Clamp(rotation.z > 180 ? rotation.z - 360 : rotation.z, -30f, 30f);
            transform.eulerAngles = rotation;

            if (transform.position.y >= 0 || transform.position.y == 0)
            {
                camera.transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z - 20);
            }
            else
            {
                camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);
            }

            if (transform.position.y >= MaxLaunch)
            {
                if (isShuttle)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        turnSpeed = 20;
                        loseFuel = true;
                    }
                }

                if (isJetFighter)
                {
                    shootTimer += Time.deltaTime;

                    if (Input.GetKeyDown(KeyCode.Mouse0) && shootTimer >= shootCooldown)
                    {
                        Debug.Log("Shoot");

                        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

                        Instantiate(bullet, position, transform.rotation);

                        shootTimer = 0f;
                    }
                }
            }


            if (loseFuel)
            {
                divisionTimer += Time.deltaTime;

                if (divisionTimer >= 0.1)
                {
                    value -= baseValue / 100;
                    divisionTimer = 0;

                    if (value < 0 || value == 0)
                    {
                        Die();
                        Debug.Log("No Fuel");
                    }
                }
            }

            height = transform.position.y + 5;
        }

        // Black hole aantrekkingskracht
        if (currentBlackHole != null && currentBlackHole.IsAttracting())
        {
            float targetX = currentBlackHole.transform.position.x;

            float direction = Mathf.Sign(targetX - transform.position.x);

            float mass = currentBlackHole.GetMass();

            float acceleration = mass * blackHolePullStrength;

            blackHoleVelocityX += direction * acceleration * Time.deltaTime;

            blackHoleVelocityX = Mathf.Clamp(
                blackHoleVelocityX,
                -blackHoleMaxPullSpeed,
                blackHoleMaxPullSpeed
            );

            transform.position += new Vector3(
                blackHoleVelocityX * Time.deltaTime,
                0f,
                0f
            );
        }
        else
        {
            // Geen multiplier/divider meer = direct stoppen met aantrekken
            blackHoleVelocityX = 0f;
            currentBlackHole = null;
        }

        Cash.text = value.ToString("F2");
    }

    public void Launch()
    {
        if (isShuttle)
        {
            turnSpeed = 10;
            loseFuel = false;
        }
        else
        {
            loseFuel = true;
        }

        isLaunching = true;
        multiplier = 1;
        speed = 2;
        turnSpeed = 20;

        speedIncrease = 0.1f;
        speedTimer = 0f;

        if (isJetFighter)
        {
            turnSpeed = 30;
            speed = 4;
        }

        obstacleSpawner.SpawnObjects();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("die");
            Die();
        }

        if (other.CompareTag("Earth"))
        {
            cashSystem.cash += value;
            Debug.Log("win " + value);
            Die();
        }

        if (other.CompareTag("Multiplier"))
        {
            MultiplierScript multiplierObject = other.GetComponent<MultiplierScript>();

            if (multiplierObject != null)
            {
                // + multiplier
                if (multiplierObject.activeMultiplier > 0)
                {
                    value += baseValue * multiplierObject.activeMultiplier;

                    Debug.Log("+" + baseValue * multiplierObject.activeMultiplier);
                    Debug.Log("Value: " + value);
                }

                // × multiplier
                if (multiplierObject.activePlus != "")
                {
                    float multiplyAmount = float.Parse(
                        multiplierObject.activePlus.Replace("x", "")
                    );

                    value *= multiplyAmount;

                    Debug.Log("×" + multiplyAmount);
                    Debug.Log("Value: " + value);
                }
            }
        }

        if (other.CompareTag("Divider"))
        {
            value /= 2;
            Debug.Log("/2");
            Debug.Log("Value: " + value);
        }

        if (other.CompareTag("BlackHole"))
        {
            BlackHole blackHole = other.GetComponent<BlackHole>();

            if (blackHole != null)
            {
                currentBlackHole = blackHole;

                float mass = blackHole.GetMass();

                // Waarden automatisch berekenen op basis van massa
                blackHolePullSpeed = 0.1f * mass;
                blackHoleMaxPullSpeed = 1f * mass;
                blackHolePullStrength = 0.1f * mass;

                Debug.Log("Black hole massa: " + mass);
                Debug.Log("Pull speed: " + blackHolePullSpeed);
                Debug.Log("Max pull speed: " + blackHoleMaxPullSpeed);
                Debug.Log("Pull strength: " + blackHolePullStrength);
            }
        }
    }

    public void Die()
    {
        ResetRocket();
    }

    public void ResetRocket()
    {
        transform.position = new Vector3(0, -5, 0);
        isLaunching = false;
        speed = 0;
        multiplier = 1;
        height = 0;
        value = baseValue;
        turnSpeed = 20;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        camera.transform.position = new Vector3(0, 5, -20);

        obstacleSpawner.ResetSpawnObstacles();
    }
}
