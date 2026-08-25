using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Type")]
    public bool isBasic;
    public bool isShuttle;
    public bool isJetFighter;

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

    [Space]

    public float baseValue;
    public float value;
    public float multiplier = 1f;


    [Space]

    public GameObject camera;
    public ObstacleSpawner obstacleSpawner;

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
        Cash.text = "Cash: " + value.ToString("F2");

        if (!isLaunching)
        {
            //if ? raket = true;
            if (Input.GetKey(KeyCode.Alpha1))
            {
                baseValue = 1;
                value = 1;
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                baseValue = 3;
                value = 3;
            }

            if (Input.GetKey (KeyCode.Alpha3))
            {
                baseValue = 5;
                value = 5;
            }

            //kies raket
            if (Input.GetKey(KeyCode.Alpha8))
            {
                isBasic = true;
                isShuttle = false;
                isJetFighter = false;
            }

            if (Input.GetKey(KeyCode.Alpha9))
            {
                isBasic = false;
                isShuttle = true;
                isJetFighter = false;
            }

            if (Input.GetKey(KeyCode.Alpha0))
            {
                isBasic = false;
                isShuttle = false;
                isJetFighter = true;
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) && (isLaunching == false))
        {
            Launch();
        }


        if (isLaunching)
        {
            float currentSpeed = transform.position.y < MaxLaunch ? launchSpeed : speed;

            transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);

            if (transform.position.y >= 0)
            {
                speedTimer += Time.deltaTime;

                if (speedTimer >= 1f)
                {
                    speed += speedIncrease;

                    speedIncrease += speedIncreaseGrowth;

                    speedTimer = 0f;
                }
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) && transform.eulerAngles.y > -30)
            {
                transform.Rotate(Vector3.forward * turnSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) && transform.eulerAngles.y < 30)
            {
                transform.Rotate(Vector3.back * turnSpeed * Time.deltaTime);
            }

            if (transform.position.y >= 0 || transform.position.y == 0)
            {
                camera.transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z - 20);

                if (isShuttle)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        turnSpeed = 20;
                        speed = 3;
                    }
                }
            }
            else
            {
                camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);
            }

            if (isJetFighter)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Debug.Log("Shoot");
                }
            }


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

            height = transform.position.y + 5; 
        }
    }

    public void Launch()
    {
        isLaunching = true;
        multiplier = 1;
        speed = 2;
        turnSpeed = 20;

        speedIncrease = 0.1f;
        speedTimer = 0f;

        if (isShuttle)
        {
            turnSpeed = 10;
        }

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
