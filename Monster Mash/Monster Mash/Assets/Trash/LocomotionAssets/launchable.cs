using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launchable : MonoBehaviour
{
    [SerializeField] private Vector2 launchVector;

    [SerializeField] private float coolDown = 1.25f;
    [SerializeField] private bool isLaunchable = true;
    [SerializeField] private bool isLaunched;

    [SerializeField] private float launchSpeed = 5f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        launchVector = CalculateLaunch();
    }

    void FixedUpdate()
    {
        LaunchThatBoi();
    }

    private void LaunchThatBoi()
    {
        if (isLaunchable && !isLaunched)
        {
            StopCoroutine("Cooldown");
            isLaunchable = false;
            isLaunched = true;
            rb.AddForce(launchVector * launchSpeed);
            StartCoroutine("Cooldown");
        }
    }

    private Vector2 CalculateLaunch()
    {
        Vector2 xIn = new Vector2();
        Vector2 yIn = new Vector2();

        if (Input.GetAxis("Horizontal") > 0)
        {
            xIn = Vector2.right;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            xIn = Vector2.left;
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            yIn = Vector2.up;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            yIn = Vector2.down;
        }

        return xIn + yIn;
    }

    private IEnumerator Cooldown()
    {
        isLaunched = true;
        isLaunchable = false;

        yield return new WaitForSeconds(coolDown);

        isLaunched = false;
        isLaunchable = true;

        yield break;
    }
}
