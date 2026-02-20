using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSelectorConveyorRepeat : MonoBehaviour
{
    public Transform screen_top;
    public Transform screen_bottom;
    public Animation glowAnim;

    void Update()
    {
        if (transform.position.y > screen_top.position.y)
        {
            transform.position = new Vector3(transform.position.x, screen_bottom.position.y, transform.position.z);
        }
        else if (transform.position.y < screen_bottom.position.y)
        {
            transform.position = new Vector3(transform.position.x, screen_top.position.y, transform.position.z);
        }
    }

    public void RestartGlowAnim()
    {
        glowAnim.Stop();
        glowAnim.Play();
    }
}
