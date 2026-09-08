using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    public bool isBasic;
    public bool isShuttle;
    public bool isJetFighter;
    public bool loseFuel;

    [Space]

    public int startBet;
    float betCooldown = 0f;

    [Space]
    public bool isLaunching;
    public float MaxLaunch;
    public float launchSpeed = 8f;
    public float speed;
    public int turnSpeed;
    public float height;
    public int health;

    [Space]

    public float speedIncrease = 0.1f;
    public float speedIncreaseGrowth = 0.1f;
    private float speedTimer;

    [Space]

    public float baseValue;
    public float value;
    public float multiplier = 1f; 

    [Space]

    public int totalStarsCollected;
    public float starMultiplier;

    [Space]

    public bool maxWin;

    [Space]

    public float shootCooldown;
    private float shootTimer;

    [Space]

    public ObstacleSpawner obstacleSpawner;
    public CashSystem cashSystem;

    [Space]

    public GameObject camera;
    public GameObject bullet;
    public GameObject basicRocket;
    public GameObject shuttle;
    public GameObject jetFighter;

    private float divisionTimer;

    [Space]

    public TMP_Text Cash;
    public TMP_Text Height;
    public TMP_Text Speed;
    public TMP_Text MaxWinText;

    [Space]

    public Button buttonNormalRocket;
    public Button buttonShuttle;
    public Button buttonJetFighter;

    public Button betLower;
    public Button betHigher;

    // --------------------------------
    // BLACK HOLE
    // --------------------------------

    [Header("Black Hole")]
    public float blackHoleOrbitSpeed = 100f;
    public float blackHolePullSpeed = 0.5f;
    public float blackHoleCenterDistance = 0.1f;

    private BlackHole currentBlackHole;

    private float blackHoleOrbitRadius;
    private float blackHoleOrbitAngle;
    private float blackHoleCurrentRadius;

    private bool inBlackHole = false;
    private bool blackHoleMiniGameStarted = false;

    // Rotatie bewaren voordat de rocket de BlackHole ingaat
    private Quaternion blackHoleRocketRotation;

    // Originele schaal bewaren
    private Vector3 blackHoleOriginalScale;

    // Bepaalt of de rocket al in de orbit zit
    private bool blackHoleInOrbit = false;

    [Space]

    public float cameraSmoothSpeed = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isLaunching = false;

        transform.position = new Vector3(0, -5, 0);

        totalStarsCollected = 0;

        starMultiplier = 1;

        MaxWinText.gameObject.SetActive(false);

        health = 1;

        buttonNormalRocket.gameObject.SetActive(true);
        buttonShuttle.gameObject.SetActive(true);
        buttonJetFighter.gameObject.SetActive(true);

        betLower.gameObject.SetActive(true);
        betHigher.gameObject.SetActive(true);

        startBet = 0;

        buttonNormalRocket.onClick.AddListener(() =>
        {
            isBasic = true;
            isShuttle = false;
            isJetFighter = false;
        });

        buttonShuttle.onClick.AddListener(() =>
        {
            isBasic = false;
            isShuttle = true;
            isJetFighter = false;
        });

        buttonJetFighter.onClick.AddListener(() =>
        {
            isBasic = false;
            isShuttle = false;
            isJetFighter = true;
        });

        betLower.onClick.AddListener(() =>
        {
            if (betCooldown > 0f)
                return;

            if (startBet > 0)
            {
                startBet -= 1;
            }
        });

        betHigher.onClick.AddListener(() =>
        {
            if (betCooldown > 0f)
                return;

            if (startBet < 26)
            {
                startBet += 1;
            }
        });
    }


    // Update is called once per frame
    void Update()
    {
        // Cooldown aftellen
        if (betCooldown > 0f)
        {
            betCooldown -= Time.deltaTime;
        }

        if (isBasic)
        {
            basicRocket.gameObject.SetActive(true);
            shuttle.gameObject.SetActive(false);
            jetFighter.gameObject.SetActive(false);
        }

        if (isShuttle)
        {
            basicRocket.SetActive(false);
            shuttle.SetActive(true);
            jetFighter.SetActive(false);
        }

        if (isJetFighter)
        {
            basicRocket.gameObject.SetActive(false);
            shuttle.gameObject.SetActive(false);
            jetFighter.SetActive(true);
        }

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

        if (health == 0 && health <= 0)
        {
            Die();
        }

        // --------------------------------
        // BLACK HOLE MOVEMENT
        // --------------------------------

        if (inBlackHole)
        {
            UpdateBlackHole();

            Cash.text = value.ToString("F2");
            Height.text = "Height: " + height.ToString("F2");
            Speed.text = "Speed: " + speed.ToString();

            return;
        }


        // --------------------------------
        // NORMALE ROCKET MOVEMENT
        // --------------------------------

        if (isLaunching)
        {
            float currentSpeed =
                transform.position.y < MaxLaunch
                    ? launchSpeed
                    : speed;

            transform.Translate(
                Vector3.up *
                currentSpeed *
                Time.deltaTime
            );


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


            if (Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(
                    Vector3.forward *
                    turnSpeed *
                    Time.deltaTime
                );
            }


            if (Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(
                    Vector3.back *
                    turnSpeed *
                    Time.deltaTime
                );
            }


            Vector3 rotation =
                transform.eulerAngles;

            rotation.z =
                Mathf.Clamp(
                    rotation.z > 180
                        ? rotation.z - 360
                        : rotation.z,
                    -30f,
                    30f
                );

            transform.eulerAngles =
                rotation;


            if (transform.position.y >= 0 ||
                transform.position.y == 0)
            {
                camera.transform.position =
                    new Vector3(
                        transform.position.x,
                        transform.position.y + 5,
                        transform.position.z - 20
                    );
            }
            else
            {
                camera.transform.position =
                    new Vector3(
                        transform.position.x,
                        camera.transform.position.y,
                        camera.transform.position.z
                    );
            }


            if (transform.position.y >= MaxLaunch)
            {
                if (isShuttle)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        turnSpeed = 20;
                        loseFuel = true;

                        Rigidbody childRigidbody = GetComponentInChildren<Rigidbody>();

                        if (childRigidbody != null)
                        {
                            childRigidbody.useGravity = true;
                        }
                    }
                }


                if (isJetFighter)
                {
                    shootTimer +=
                        Time.deltaTime;

                    if (Input.GetKeyDown(KeyCode.Mouse0) && shootTimer >= shootCooldown || Input.GetKeyDown(KeyCode.Space) &&
                        shootTimer >= shootCooldown)
                    {
                        Debug.Log("Shoot");

                        Vector3 position =
                            new Vector3(
                                transform.position.x,
                                transform.position.y,
                                transform.position.z
                            );

                        Instantiate(
                            bullet,
                            position,
                            transform.rotation
                        );

                        shootTimer = 0f;
                    }
                }
            }


            if (loseFuel)
            {
                divisionTimer +=
                    Time.deltaTime;

                if (divisionTimer >= 0.1)
                {
                    value -=
                        baseValue / 100;

                    divisionTimer = 0;

                    if (value < 0 ||
                        value == 0)
                    {
                        Die();

                        Debug.Log(
                            "No Fuel"
                        );
                    }
                }
            }

            height = transform.position.y + 5;

            multiplier = Mathf.Round((value / baseValue) * 100f) / 100f;
        }

        if (startBet == 0)
        {
            baseValue = 0.1f;
        }
        else
        {
            if (startBet == 1)
            {
                baseValue = 0.2f;
            }
            else
            {
                if (startBet == 2)
                {
                    baseValue = 0.5f;
                }
                else
                {
                    if (startBet == 3)
                    {
                        baseValue = 1;
                    }
                    else
                    {
                        if (startBet == 4)
                        {
                            baseValue = 1.5f;
                        }
                        else
                        {
                            if (startBet == 5)
                            {
                                baseValue = 2;
                            }
                            else
                            {
                                if (startBet == 6)
                                {
                                    baseValue = 2.5f;
                                }
                                else
                                {
                                    if (startBet == 7)
                                    {
                                        baseValue = 3;
                                    }
                                    else
                                    {
                                        if (startBet == 8)
                                        {
                                            baseValue = 3.5f;
                                        }
                                        else
                                        {
                                            if (startBet == 9)
                                            {
                                                baseValue = 4;
                                            }
                                            else
                                            {
                                                if (startBet == 10)
                                                {
                                                    baseValue = 4.5f;
                                                }
                                                else
                                                {
                                                    if (startBet == 11)
                                                    {
                                                        baseValue = 5;
                                                    }
                                                    else
                                                    {
                                                        if (startBet == 12)
                                                        {
                                                            baseValue = 6;
                                                        }
                                                        else
                                                        {
                                                            if (startBet == 13)
                                                            {
                                                                baseValue = 7;
                                                            }
                                                            else
                                                            {
                                                                if (startBet == 14)
                                                                {
                                                                    baseValue = 8;
                                                                }
                                                                else
                                                                {
                                                                    if (startBet == 15)
                                                                    {
                                                                        baseValue = 9;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (startBet == 16)
                                                                        {
                                                                            baseValue = 10;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (startBet == 17)
                                                                            {
                                                                                baseValue = 11;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (startBet == 18)
                                                                                {
                                                                                    baseValue = 12;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (startBet == 19)
                                                                                    {
                                                                                        baseValue = 13;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (startBet == 20)
                                                                                        {
                                                                                            baseValue = 14;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (startBet == 21)
                                                                                            {
                                                                                                baseValue = 15;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (startBet == 22)
                                                                                                {
                                                                                                    baseValue = 16;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (startBet == 23)
                                                                                                    {
                                                                                                        baseValue = 17;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (startBet == 24)
                                                                                                        {
                                                                                                            baseValue = 18;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (startBet == 25)
                                                                                                            {
                                                                                                                baseValue = 19;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (startBet == 26)
                                                                                                                {
                                                                                                                    baseValue = 20;
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    Debug.Log("Invalid Startbet");
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (!isLaunching)
        {
            value = baseValue;
        }
        
        Cash.text =
                value.ToString("F2");

        Height.text =
            "Height: " +
            height.ToString("F2");

        Speed.text =
            "Speed: " +
            speed.ToString();
    }


    // --------------------------------
    // BLACK HOLE UPDATE
    // --------------------------------

    void UpdateBlackHole()
    {
        if (currentBlackHole == null)
        {
            inBlackHole = false;
            return;
        }


        Vector3 blackHolePosition =
            currentBlackHole.transform.position;


        // --------------------------------
        // EERST NAAR DE ORBIT
        // --------------------------------

        if (!blackHoleInOrbit)
        {
            blackHoleCurrentRadius =
                Mathf.MoveTowards(
                    blackHoleCurrentRadius,
                    blackHoleOrbitRadius,
                    blackHolePullSpeed *
                    Time.deltaTime
                );


            float radians =
                blackHoleOrbitAngle *
                Mathf.Deg2Rad;


            Vector3 offset =
                new Vector3(
                    Mathf.Cos(radians) *
                    blackHoleCurrentRadius,

                    Mathf.Sin(radians) *
                    blackHoleCurrentRadius,

                    0f
                );


            transform.position =
                blackHolePosition +
                offset;


            // Rocket blijft dezelfde kant op wijzen
            transform.rotation =
                blackHoleRocketRotation;


            // Zodra de rocket in de baan zit,
            // begint de orbit
            if (blackHoleCurrentRadius <=
                blackHoleOrbitRadius + 0.01f)
            {
                blackHoleInOrbit = true;
            }

            return;
        }


        // --------------------------------
        // ORBIT
        // --------------------------------

        blackHoleCurrentRadius =
            Mathf.MoveTowards(
                blackHoleCurrentRadius,
                blackHoleCenterDistance,
                blackHolePullSpeed *
                Time.deltaTime
            );


        // --------------------------------
        // ORBIT HOEK
        // --------------------------------

        blackHoleOrbitAngle +=
            blackHoleOrbitSpeed *
            Time.deltaTime;


        float orbitRadians =
            blackHoleOrbitAngle *
            Mathf.Deg2Rad;


        // --------------------------------
        // NIEUWE POSITIE
        // --------------------------------

        Vector3 orbitOffset =
            new Vector3(
                Mathf.Cos(orbitRadians) *
                blackHoleCurrentRadius,

                Mathf.Sin(orbitRadians) *
                blackHoleCurrentRadius,

                0f
            );


        transform.position =
            blackHolePosition +
            orbitOffset;


        // --------------------------------
        // ROCKET ROTATIE BEWAREN
        // --------------------------------

        transform.rotation =
            blackHoleRocketRotation;


        // --------------------------------
        // ROCKET KLEINER MAKEN
        // --------------------------------

        float normalizedRadius =
            Mathf.InverseLerp(
                blackHoleCenterDistance,
                blackHoleOrbitRadius,
                blackHoleCurrentRadius
            );


        transform.localScale =
            blackHoleOriginalScale *
            normalizedRadius;

        // --------------------------------
        // CAMERA SMOOTH INZOOMEN
        // --------------------------------

        float cameraZ = Mathf.Lerp(
            -10f,
            -20f,
            normalizedRadius
        );

        Vector3 targetCameraPosition =
            new Vector3(
                transform.position.x,
                transform.position.y,
                cameraZ
            );

        camera.transform.position =
            Vector3.Lerp(
                camera.transform.position,
                targetCameraPosition,
                cameraSmoothSpeed * Time.deltaTime
            );


        // --------------------------------
        // MINI GAME STARTEN
        // --------------------------------

        if (blackHoleCurrentRadius <=
            blackHoleCenterDistance + 0.01f)
        {
            if (!blackHoleMiniGameStarted)
            {
                blackHoleMiniGameStarted = true;

                BlackHoleMiniGame();
            }
        }
    }


    // --------------------------------
    // LAUNCH
    // --------------------------------

    public void Launch()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

        speed = 2.5f;

        if (isBasic)
        {
            turnSpeed = 20;
        }

        speedIncrease = 0.1f;

        speedTimer = 0f;


        if (isJetFighter)
        {
            turnSpeed = 30;
            speed = 5;
        }

        MaxWinText.gameObject.SetActive( false );

        obstacleSpawner.SpawnObjects();

        buttonNormalRocket.gameObject.SetActive(false);
        buttonShuttle.gameObject.SetActive(false);
        buttonJetFighter.gameObject.SetActive(false);

        betLower.gameObject.SetActive(false);
        betHigher.gameObject.SetActive(false);
    }


    // --------------------------------
    // COLLISION
    // --------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (health > 0)
            {
                health -= 1;
            }
            else
            {
                Die();
                Debug.Log("Die");
            }
        }


        if (other.CompareTag("Earth"))
        {
            cashSystem.cash += (value * starMultiplier);

            Debug.Log(
                "win " + value
            );

            Die();
        }


        if (other.CompareTag("Multiplier"))
        {
            MultiplierScript multiplierObject =
                other.GetComponent<MultiplierScript>();

            if (multiplierObject != null)
            {
                // + multiplier
                if (multiplierObject.activeMultiplier > 0)
                {
                    value +=
                        baseValue *
                        multiplierObject.activeMultiplier;

                    Debug.Log(
                        "+" +
                        baseValue *
                        multiplierObject.activeMultiplier
                    );

                    Debug.Log(
                        "Value: " +
                        value
                    );
                }


                // × multiplier
                if (multiplierObject.activePlus != "")
                {
                    float multiplyAmount =
                        float.Parse(
                            multiplierObject.activePlus
                                .Replace("x", "")
                        );

                    value *=
                        multiplyAmount;

                    Debug.Log(
                        "×" +
                        multiplyAmount
                    );

                    Debug.Log(
                        "Value: " +
                        value
                    );
                }
            }
        }


        if (other.CompareTag("Divider"))
        {
            value /= 2;

            Debug.Log("/2");

            Debug.Log(
                "Value: " +
                value
            );
        }


        if (other.CompareTag("BlackHole"))
        {
            BlackHole blackHole =
                other.GetComponent<BlackHole>();

            if (blackHole != null &&
                !inBlackHole)
            {
                EnterBlackHole(
                    blackHole
                );
            }
        }

        if (other.CompareTag("Star"))
        {
            if (totalStarsCollected == 0)
            {
                starMultiplier = 1.5f;
            }
            else
            {
                if (totalStarsCollected == 1)
                {
                    starMultiplier = 2f;
                }
                else
                {
                    if (totalStarsCollected == 2)
                    {
                        starMultiplier = 3;
                    }
                    else
                    {
                        if (totalStarsCollected == 3)
                        {
                            starMultiplier = 5;
                        }
                        else
                        {
                            if (totalStarsCollected == 4)
                            {
                                starMultiplier = 7.5f;
                            }
                            else
                            {
                                if (totalStarsCollected == 5)
                                {
                                    starMultiplier = 10;
                                }
                                else
                                {
                                    if (totalStarsCollected == 6)
                                    {
                                        starMultiplier = 20;
                                    }
                                    else
                                    {
                                        if (totalStarsCollected == 7)
                                        {
                                            starMultiplier = 50;
                                        }
                                        else
                                        {
                                            if (totalStarsCollected == 8)
                                            {
                                                starMultiplier = 100;
                                            }
                                            else
                                            {
                                                MaxWin();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            totalStarsCollected += 1;
        }

        if (other.CompareTag("Shield"))
        {
            health = 2;
        }
    }


    // --------------------------------
    // ENTER BLACK HOLE
    // --------------------------------

    void EnterBlackHole(
        BlackHole blackHole
    )
    {
        currentBlackHole =
            blackHole;

        inBlackHole = true;

        blackHoleMiniGameStarted =
            false;

        blackHoleInOrbit = false;


        // Normale beweging stoppen
        speed = 0;

        turnSpeed = 0;


        // --------------------------------
        // ROTATIE OPSLAAN
        // --------------------------------

        blackHoleRocketRotation =
            transform.rotation;


        // --------------------------------
        // SCHAAL OPSLAAN
        // --------------------------------

        blackHoleOriginalScale =
            transform.localScale;


        // --------------------------------
        // AFSTAND TOT BLACK HOLE
        // --------------------------------

        Vector3 difference =
            transform.position -
            blackHole.transform.position;


        float currentDistance =
            difference.magnitude;


        // --------------------------------
        // ORBIT RADIUS
        // --------------------------------

        // De rocket moet eerst naar de
        // buitenste baan van de BlackHole.

        blackHoleOrbitRadius =
            (blackHole.size / 2f) *
            blackHole.maxOrbitRadius;


        // Als de rocket al binnen de baan zit,
        // gebruiken we de huidige afstand
        // zodat hij niet ineens naar buiten springt.

        if (currentDistance <
            blackHoleOrbitRadius)
        {
            blackHoleOrbitRadius =
                currentDistance;
        }


        blackHoleCurrentRadius =
            currentDistance;


        // --------------------------------
        // BEGINHOEK
        // --------------------------------

        blackHoleOrbitAngle =
            Mathf.Atan2(
                difference.y,
                difference.x
            ) *
            Mathf.Rad2Deg;


        Debug.Log(
            "Rocket entered BlackHole"
        );
    }


    // --------------------------------
    // BLACK HOLE MINI GAME
    // --------------------------------

    public void BlackHoleMiniGame()
    {
        Debug.Log(
            "BLACK HOLE MINI GAME!"
        );

        // Hier kun je later je BlackHole mini-game starten.
    }


    // --------------------------------
    // DIE
    // --------------------------------

    public void Die()
    {
        ResetRocket();
    }


    // --------------------------------
    // RESET
    // --------------------------------

    public void ResetRocket()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        transform.position =
            new Vector3(
                0,
                -5,
                0
            );

        isLaunching = false;

        speed = 0;

        multiplier = 1;

        height = 0;

        value = baseValue;

        turnSpeed = 20;

        health = 1;

        transform.rotation =
            Quaternion.Euler(
                0,
                0,
                0
            );


        transform.localScale =
            blackHoleOriginalScale == Vector3.zero
                ? Vector3.one
                : blackHoleOriginalScale;


        camera.transform.position =
            new Vector3(
                0,
                5,
                -20
            );


        // BlackHole reset
        currentBlackHole = null;

        inBlackHole = false;

        blackHoleMiniGameStarted = false;

        blackHoleInOrbit = false;

        blackHoleOrbitRadius = 0f;

        blackHoleCurrentRadius = 0f;

        blackHoleOrbitAngle = 0f;

        blackHoleRocketRotation =
            Quaternion.identity;

        blackHoleOriginalScale =
            Vector3.zero;


        obstacleSpawner.ResetSpawnObstacles();

        totalStarsCollected = 0;
        starMultiplier = 1;

        buttonNormalRocket.gameObject.SetActive(true);
        buttonShuttle.gameObject.SetActive(true);
        buttonJetFighter.gameObject.SetActive(true);

        betLower.gameObject.SetActive(true);
        betHigher.gameObject.SetActive(true);
    }

    public void MaxWin()
    {
        cashSystem.cash += (baseValue * 20000);

        Debug.Log("Max Win");

        MaxWinText.gameObject.SetActive(true);

        ResetRocket();
    }
}