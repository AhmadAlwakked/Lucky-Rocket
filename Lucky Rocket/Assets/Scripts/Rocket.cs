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
    public int height;

    [Space]

    public float value;
    public float multiplier;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLaunching = false;
        isBasic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }
    }

    public void Launch()
    {
        isLaunching = true;
        multiplier = 1f;
        speed = 0;
        height = 0;
    }

}
