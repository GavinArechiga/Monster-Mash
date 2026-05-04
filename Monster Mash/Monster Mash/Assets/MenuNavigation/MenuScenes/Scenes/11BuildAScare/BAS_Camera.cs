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

    private Camera cam;

    [SerializeField] private GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateMonster(Vector3 lookInput)
    {
        transform.position = new Vector3(monster.transform.position.x, transform.position.y, transform.position.z);

        float delta = lookInput.y * rotSpeed * Time.deltaTime;

        float newAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

        float appliedRotation = newAngle - currentAngle;

        transform.RotateAround(monster.transform.position, Vector3.right * 0.5f, appliedRotation);
        monster.transform.Rotate(Vector3.up, lookInput.x * -rotSpeed * Time.deltaTime, Space.World);

        currentAngle = newAngle;
    }

    public void CameraZoomIn()
    {
        float newFOV = cam.fieldOfView - (zoomSpeed * Time.deltaTime);

        cam.fieldOfView = Mathf.Max(newFOV, minFOV);
    }

    public void CameraZoomOut()
    {
        float newFOV = cam.fieldOfView + (zoomSpeed * Time.deltaTime);

        cam.fieldOfView = Mathf.Min(newFOV, maxFOV);
    }
}
