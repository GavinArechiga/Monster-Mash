using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroMiscVisuals : MonoBehaviour
{
    public Transform groundCheck;
    public GameObject playerRingPrefab;
    private GameObject playerRing;
    private bool playerRingActive = true;


    void Update()
    {
        if (playerRing != null)
        {
            RaycastHit detectedGroundPoint;

            if (Physics.Raycast(groundCheck.position, Vector3.down, out detectedGroundPoint, 100))
            {
                Vector3 normalizedPlayerPoint = new Vector3(detectedGroundPoint.point.x, detectedGroundPoint.point.y + 0.2f, detectedGroundPoint.point.z);

                playerRing.transform.position = normalizedPlayerPoint;

                if (playerRingActive == false)
                {
                    playerRing.SetActive(true);
                    playerRingActive = true;
                }
            }
        }
        else
        {
            generatePlayerRing();
        }
    }

    private void generatePlayerRing()
    {
        playerRing = Instantiate(playerRingPrefab);
    }
}
