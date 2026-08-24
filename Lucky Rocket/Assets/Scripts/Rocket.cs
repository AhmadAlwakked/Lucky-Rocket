using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Type")]
    public bool isBasic;
    public bool isShuttle;
    public bool isJetFighter;

    [Header("Launch")]
    public bool isLaunching;
    public int speed;
    public int turnSpeed;
    public int height;

    [Space]

    public float baseValue;
    public float value;
    public float multiplier = 1f;


    [Space]

    public List<GameObject> Obstacles;
    public GameObject camera;

    private CapsuleCollider capsuleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLaunching = false;
        isBasic = true;

        transform.position = new Vector3(0, -5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        value = baseValue * multiplier;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        CheckCollider();
    }

    public void Launch()
    {
        isLaunching = true;
        multiplier = 1f;
        height = 0;
    }

    public void LateUpdate()
    {
        if (isLaunching)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);

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
                camera.transform.Translate(Vector3.up * speed * Time.deltaTime);
            }
        }
    }

    public void CheckCollider()
    {
        CapsuleCollider cc = GetComponent<CapsuleCollider>();

        Collider[] geraakteObjecten = Physics.OverlapCapsule(transform.position, transform.position, cc.radius);

        foreach (Collider hit in geraakteObjecten)
        {
            if (Obstacles.Contains(hit.gameObject))
            {
                Debug.Log("hit");

                Die();
            }
        }
    }

    public void Die()
    {
        Reset();
    }

    public void Reset()
    {
        transform.position = new Vector3(0, -5, 0);
        isLaunching = false;
        speed = 0;
        multiplier = 0;
        height = 0;
    }
}
