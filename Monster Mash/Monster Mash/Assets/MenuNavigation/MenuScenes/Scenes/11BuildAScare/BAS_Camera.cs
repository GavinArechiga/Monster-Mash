using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BAS_Camera : MonoBehaviour
{
    private float rotSpeed = 100f;
    private float currentAngle = 0f;
    private float minAngle = -75f;
    private float maxAngle = 75f;

    private float defaultFOV = 60f;
    private float maxFOV = 90f;
    private float minFOV = 5f;
    private float zoomSpeed = 35f;

    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera rotCam;

    [SerializeField] private GameObject monster;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateMonster(Vector3 lookInput)
    {
        rotCam.transform.position = new Vector3(monster.transform.position.x, rotCam.transform.position.y, rotCam.transform.position.z);

        float delta = lookInput.y * rotSpeed * Time.deltaTime;

        float newAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

        float appliedRotation = newAngle - currentAngle;

        rotCam.transform.RotateAround(monster.transform.position, Vector3.right * 0.5f, appliedRotation);
        monster.transform.Rotate(Vector3.up, lookInput.x * -rotSpeed * Time.deltaTime, Space.World);

        currentAngle = newAngle;
    }

    public void CameraZoomIn()
    {
        float newFOV = mainCam.fieldOfView - (zoomSpeed * Time.deltaTime);

        mainCam.fieldOfView = Mathf.Max(newFOV, minFOV);
    }

    public void CameraZoomOut()
    {
        float newFOV = mainCam.fieldOfView + (zoomSpeed * Time.deltaTime);

        mainCam.fieldOfView = Mathf.Min(newFOV, maxFOV);
    }
}
