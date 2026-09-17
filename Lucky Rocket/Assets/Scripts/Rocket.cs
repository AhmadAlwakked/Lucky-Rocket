using System.Collections;
using TMPro;
using UnityEngine;
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

    public TMP_Text stars;

    [Space]

    public bool maxWin;

    [Space]

    public float shootCooldown;
    private float shootTimer;

    [Space]

    public ObstacleSpawner obstacleSpawner;
    public CashSystem cashSystem;
    public BlackHole blackHole;

    [Space]

    public GameObject camera;
    public GameObject bullet;
    public GameObject basicRocket;
    public GameObject shuttle;
    public GameObject jetFighter;

    public GameObject fuelTank;
    private Transform fuelTankOriginalParent;
    private Vector3 fuelTankOriginalPosition;
    private Quaternion fuelTankOriginalRotation;

    private float divisionTimer;

    [Space]

    public TMP_Text Cash;
    public TMP_Text Height;
    public TMP_Text Speed;
    public TMP_Text MaxWinText;
    public TMP_Text Bet;

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
    public float blackHolePullSpeed = 0.5f;

    public BlackHole currentBlackHole;
    private BlackHole exitedBlackHole;

    private float blackHoleOrbitRadius;
    private float blackHoleCurrentRadius;

    public bool inBlackHole = false;

    private bool blackHoleMiniGameStarted = false;

    private Vector3 blackHoleOriginalScale;

    private float blackHoleStartRadius;

    private float blackHoleAngle;

    private bool blackHoleMiniGameActive = false;

    private bool exitingBlackHole = false;

    public float blackHoleRocketSpeed;

    public int blackHoleTurnSpeed;

    [Header("Black Hole Mini Game")]
    public float blackHoleMiniGameScale;

    [Header("Black Hole Swing")]
    public float blackHoleSwingAmount = 5f;
    public float blackHoleSwingSpeed = 4f;
    public float blackHoleTurnSwingAmount = 8f;

    private float blackHoleSwingTime;

    private bool deathStarted = false;

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

        fuelTankOriginalParent = fuelTank.transform.parent;
        fuelTankOriginalPosition = fuelTank.transform.localPosition;
        fuelTankOriginalRotation = fuelTank.transform.localRotation;
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

            Bet.text = baseValue.ToString();
        }

        if (isShuttle)
        {
            basicRocket.SetActive(false);
            shuttle.SetActive(true);
            jetFighter.SetActive(false);

            Bet.text = (baseValue * 10).ToString();
        }

        if (isJetFighter)
        {
            basicRocket.gameObject.SetActive(false);
            shuttle.gameObject.SetActive(false);
            jetFighter.SetActive(true);

            Bet.text = (baseValue * 100).ToString();
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isLaunching == false))
        {
            if (isBasic)
            {
                if (cashSystem.cash >= baseValue)
                {
                    isLaunching = true;
                    Launch();
                    cashSystem.cash -= baseValue;
                }
                else
                {
                    Debug.Log("Not Enough Cash");
                }
            }

            if (isShuttle)
            {
                if (cashSystem.cash >= baseValue * 10)
                {
                    isLaunching = true;
                    Launch();
                    cashSystem.cash -= baseValue * 10;
                }
                else
                {
                    Debug.Log("Not Enough cash");
                }
            }

            if (isJetFighter)
            {
                if (cashSystem.cash >= baseValue * 100)
                {
                    isLaunching = true;
                    Launch();
                    if (cashSystem.cash >= baseValue)
                    {
                        cashSystem.cash -= baseValue * 100;
                    }
                }
                else
                {
                    Debug.Log("Not Enough Cash");
                }
            }
        }

        if (health <= 0 && !deathStarted)
        {
            deathStarted = true;

            cashSystem.die.gameObject.SetActive(true);

            StartCoroutine(Die());
        }

        stars.text = "stars: " + totalStarsCollected;

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

            if (Input.GetKey(KeyCode.R))
            {
                ResetRocket();
            }

            Vector3 rotation =
                transform.eulerAngles;

            rotation.z =
                Mathf.Clamp(
                    rotation.z > 180
                        ? rotation.z - 360
                        : rotation.z,
                    -40,
                    40f
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

                if (isShuttle)
                {
                    if (Input.GetKeyDown(KeyCode.Space) && !loseFuel)
                    {
                        turnSpeed = 30;
                        loseFuel = true;

                        // Tank losmaken van de shuttle
                        fuelTank.transform.SetParent(null);

                        Rigidbody rb = fuelTank.GetComponent<Rigidbody>();
                        rb.isKinematic = false;
                        rb.useGravity = true;
                    }
                }


                if (isJetFighter)
                {
                    shootTimer +=
                        Time.deltaTime;

                    if (Input.GetKeyDown(KeyCode.Mouse0) && shootTimer >= shootCooldown || Input.GetKeyDown(KeyCode.Space) &&
                        shootTimer >= shootCooldown)
                    {
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
                        cashSystem.SetDieText("Out of Fuel");

                        cashSystem.die.gameObject.SetActive(true);

                        StartCoroutine(Die());

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

    public void UpdateBlackHole()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ResetRocket();
        }

        if (currentBlackHole == null)
        {
            inBlackHole = false;
            return;
        }

        // --------------------------------
        // BLACK HOLE MINI GAME
        // --------------------------------

        if (blackHoleMiniGameActive)
        {
            Camera miniGameCam =
                camera.GetComponent<Camera>();

            float miniGameCameraOffset =
                5f *
                Mathf.Tan(
                    miniGameCam.fieldOfView * 0.5f *
                    Mathf.Deg2Rad
                ) /
                Mathf.Tan(
                    60f * 0.5f *
                    Mathf.Deg2Rad
                );

            camera.transform.position =
                new Vector3(
                    transform.position.x,
                    transform.position.y + miniGameCameraOffset,
                    camera.transform.position.z
                );


            // --------------------------------
            // ROCKET OMHOOG
            // --------------------------------

            float pullSwing =
                Mathf.Sin(
                    blackHoleSwingTime *
                    blackHoleSwingSpeed
                ) *
                0.15f;

            transform.Translate(
                Vector3.up *
                blackHoleRocketSpeed *
                Time.deltaTime
            );

            transform.position +=
                Vector3.right *
                pullSwing *
                Time.deltaTime;


            // --------------------------------
            // BESTURING
            // --------------------------------

            float inputDirection = 0f;

            if (Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.LeftArrow))
            {
                inputDirection = 1f;
            }

            if (Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.RightArrow))
            {
                inputDirection = -1f;
            }


            // --------------------------------
            // ROCKET BESTUREN
            // --------------------------------

            float currentRotation =
                transform.eulerAngles.z;

            if (currentRotation > 180f)
            {
                currentRotation -= 360f;
            }

            currentRotation +=
                inputDirection *
                blackHoleTurnSpeed *
                Time.deltaTime;


            // --------------------------------
            // BLACK HOLE SWING
            // --------------------------------

            blackHoleSwingTime += Time.deltaTime;

            float blackHoleSwing =
                Mathf.Sin(
                    blackHoleSwingTime *
                    blackHoleSwingSpeed
                ) *
                blackHoleSwingAmount;


            // --------------------------------
            // EXTRA BEWEGING TIJDENS STUREN
            // --------------------------------

            float steeringSwing =
                inputDirection *
                blackHoleTurnSwingAmount;


            // --------------------------------
            // TOTALE ROTATIE
            // --------------------------------

            float swingTargetRotation =
                currentRotation +
                blackHoleSwing +
                steeringSwing;


            // --------------------------------
            // MAXIMAAL 30 GRADEN
            // --------------------------------

            swingTargetRotation =
                Mathf.Clamp(
                    swingTargetRotation,
                    -40f,
                    40f
                );


            // --------------------------------
            // SOEPEL NAAR ROTATIE
            // --------------------------------

            float smoothRotation =
                Mathf.LerpAngle(
                    transform.eulerAngles.z,
                    swingTargetRotation,
                    8f * Time.deltaTime
                );

            transform.rotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    smoothRotation
                );

            if (isJetFighter)
            {
                shootTimer +=
                    Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.Mouse0) && shootTimer >= shootCooldown || Input.GetKeyDown(KeyCode.Space) &&
                    shootTimer >= shootCooldown)
                {
                    Vector3 position =
                        new Vector3(
                            transform.position.x,
                            transform.position.y,
                            transform.position.z
                        );
                    GameObject newBullet = Instantiate(
                        bullet,
                        position,
                        transform.rotation
                    );

                    Bullet bulletScript = newBullet.GetComponent<Bullet>();

                    if (bulletScript != null)
                    {
                        bulletScript.isBlackHoleBullet = true;
                    }

                    shootTimer = 0f;
                }
            }
            return;
        }

        Vector3 blackHolePosition =
                currentBlackHole.transform.position;


        // --------------------------------
        // NAAR DE EERSTE BAAN
        // --------------------------------

        blackHoleCurrentRadius =
            Mathf.MoveTowards(
                blackHoleCurrentRadius,
                blackHoleOrbitRadius,
                blackHolePullSpeed *
                Time.deltaTime
            );


        // --------------------------------
        // VOORTGANG
        // --------------------------------

        float pullProgress =
            Mathf.InverseLerp(
                blackHoleStartRadius,
                blackHoleOrbitRadius,
                blackHoleCurrentRadius
            );

        pullProgress =
            Mathf.Clamp01(
                pullProgress
            );


        // --------------------------------
        // BOOG NAAR BENEDEN
        // --------------------------------

        float targetAngle =
            -Mathf.PI / 2f;

        float currentAngle =
            Mathf.Lerp(
                blackHoleAngle,
                targetAngle,
                pullProgress
            );


        float x =
            blackHolePosition.x +
            Mathf.Cos(currentAngle) *
            blackHoleCurrentRadius;

        float y =
            blackHolePosition.y +
            Mathf.Sin(currentAngle) *
            blackHoleCurrentRadius;


        transform.position =
            new Vector3(
                x,
                y,
                transform.position.z
            );


        // --------------------------------
        // KLEINER WORDEN
        // --------------------------------

        float targetScale =
            Mathf.Lerp(
                1f,
                blackHoleMiniGameScale,
                pullProgress
            );

        transform.localScale =
            blackHoleOriginalScale *
            targetScale;

        // --------------------------------
        // CAMERA VOLGT ROCKET + ZOOM
        // --------------------------------
        Camera cam =
            camera.GetComponent<Camera>();

        // Inzoomen
        cam.fieldOfView =
            Mathf.Lerp(
                60f,
                6f,
                pullProgress
            );

        // Camera-offset aanpassen aan de zoom
        float cameraOffset =
            5f *
            Mathf.Tan(
                cam.fieldOfView * 0.5f *
                Mathf.Deg2Rad
            ) /
            Mathf.Tan(
                60f * 0.5f *
                Mathf.Deg2Rad
            );

        // Camera volgt de rocket
        camera.transform.position =
            new Vector3(
                transform.position.x,
                transform.position.y + cameraOffset,
                camera.transform.position.z
            );

        // --------------------------------
        // ROCKET DRAAIT MEE MET DE BLACK HOLE
        // --------------------------------

        float targetRotation =
            currentAngle *
            Mathf.Rad2Deg +
            90f;

        float rotationSpeed =
            blackHole.rotationSpeed /
            Mathf.Max(
                blackHole.size,
                1f
            );

        float newRotation =
            Mathf.MoveTowardsAngle(
                transform.eulerAngles.z,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                newRotation
            );

        // --------------------------------
        // MINI GAME STARTEN
        // --------------------------------

        if (targetScale <=
                blackHoleMiniGameScale)
        {
            transform.localScale =
                blackHoleOriginalScale *
                blackHoleMiniGameScale;

            if (!blackHoleMiniGameStarted)
            {
                blackHoleMiniGameStarted = true;
                blackHoleMiniGameActive = true;
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

        MaxWinText.gameObject.SetActive(false);

        obstacleSpawner.SpawnObjects();

        buttonNormalRocket.gameObject.SetActive(false);
        buttonShuttle.gameObject.SetActive(false);
        buttonJetFighter.gameObject.SetActive(false);

        betLower.gameObject.SetActive(false);
        betHigher.gameObject.SetActive(false);

        Bet.gameObject.SetActive(false);
    }

    // --------------------------------
    // ENTER BLACK HOLE
    // --------------------------------

    public void EnterBlackHole(BlackHole blackHole)
    {
        // Voorkom dat dezelfde Black Hole opnieuw wordt getriggerd
        if (blackHole == exitedBlackHole)
        {
            return;
        }

        currentBlackHole = blackHole;

        inBlackHole = true;

        blackHoleMiniGameStarted =
            false;


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
        // EERSTE / BUITENSTE BAAN
        // --------------------------------

        blackHoleOrbitRadius =
            (blackHole.size / 2f) *
            blackHole.maxOrbitRadius;


        // Als de rocket al binnen de baan zit,
        // gebruiken we de huidige afstand.
        if (currentDistance <
            blackHoleOrbitRadius)
        {
            blackHoleOrbitRadius =
                currentDistance;
        }


        blackHoleCurrentRadius =
            currentDistance;

        blackHoleStartRadius =
            currentDistance;

        blackHoleAngle =
    Mathf.Atan2(
        difference.y,
        difference.x
    );
    }

    public void ExitBlackHole()
    {
        if (exitingBlackHole)
            return;

        exitingBlackHole = true;

        StartCoroutine(SmoothExitBlackHole());
    }

    private IEnumerator SmoothExitBlackHole()
    {
        // Black Hole onthouden
        exitedBlackHole = currentBlackHole;

        // Referenties bewaren
        Camera cam = camera.GetComponent<Camera>();

        Vector3 startScale = transform.localScale;
        Vector3 targetScale =
            blackHoleOriginalScale == Vector3.zero
                ? Vector3.one
                : blackHoleOriginalScale;

        float startFOV = cam != null
            ? cam.fieldOfView
            : 60f;

        float targetFOV = 60f;

        // Hoe lang de exit duurt
        float duration = 1.2f;

        float timer = 0f;

        // Smooth exit
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            // SmoothStep geeft een veel zachtere beweging
            t = Mathf.SmoothStep(0f, 1f, t);

            // Rocket groter maken
            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    t
                );

            // Camera uitzoomen
            if (cam != null)
            {
                cam.fieldOfView =
                    Mathf.Lerp(
                        startFOV,
                        targetFOV,
                        t
                    );
            }

            yield return null;
        }

        // Exact eindpunt instellen
        transform.localScale = targetScale;

        if (cam != null)
        {
            cam.fieldOfView = targetFOV;
        }

        // Black Hole verlaten
        inBlackHole = false;

        currentBlackHole = null;

        blackHoleMiniGameStarted = false;
        blackHoleMiniGameActive = false;

        // Black Hole variabelen resetten
        blackHoleOrbitRadius = 0f;
        blackHoleCurrentRadius = 0f;
        blackHoleStartRadius = 0f;
        blackHoleAngle = 0f;

        exitingBlackHole = false;
    }


    // --------------------------------
    // DIE
    // --------------------------------

    public IEnumerator WinDie()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        ResetRocket();

        Time.timeScale = 1f;
    }

    public IEnumerator Die()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1f);

        ResetRocket();

        Time.timeScale = 1f;
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

        camera.GetComponent<Camera>().fieldOfView = 60f;

        // BlackHole reset
        currentBlackHole = null;

        inBlackHole = false;

        blackHoleMiniGameStarted = false;

        blackHoleOrbitRadius = 0f;

        blackHoleCurrentRadius = 0f;

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

        Bet.gameObject.SetActive(true);

        cashSystem.totalWin.gameObject.SetActive(false);

        cashSystem.die.gameObject.SetActive(false);

        Rigidbody rb = fuelTank.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        fuelTank.transform.SetParent(fuelTankOriginalParent);
        fuelTank.transform.localPosition = fuelTankOriginalPosition;
        fuelTank.transform.localRotation = fuelTankOriginalRotation;

        isLaunching = false;
        deathStarted = false;
    }

    public void MaxWin()
    {
        cashSystem.cash += (baseValue * 20000);

        Debug.Log("Max Win");

        MaxWinText.gameObject.SetActive(true);

        ResetRocket();
    }
}