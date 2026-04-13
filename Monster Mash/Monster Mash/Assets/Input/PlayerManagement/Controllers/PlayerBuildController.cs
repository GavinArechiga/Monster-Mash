using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBuildController : MonoBehaviour, IPlayerController
{
    private PlayerInput playerInput;
    [SerializeField] private bool isActive = false;

    [SerializeField] public GameObject monster;
    [SerializeField] private GameObject crosshair;
    private Camera cam;
    private RectTransform canvas;

    private Vector2 lookInput;
    private float currentAngle = 0f;
    private float minAngle = -75f;
    private float maxAngle = 75f;

    private float stickMin = 0.45f;

    private float rotSpeed = 100f;

    [SerializeField] private RectTransform crossHair;
    private float crosshairSpeed = 300f;
    private Vector2 crosshairMove;

    private float defaultFOV = 60f;
    private float maxFOV = 60f;
    private float minFOV = 35f;
    private float zoomSpeed = 20f;

    private bool isHoldingRight = false;
    private bool isHoldingLeft = false;

    private RectTransform myCursor;

    private bool hoverPart = false;
    private GameObject partToHover;
    private GameObject monsterMesh;
    private bool hoverIsHead = false;

    [SerializeField] private List<GameObject> partList;

    private GameObject camTarget;

    private PartPool partPool;
    private int poolIndex = 0;
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("BuildAScare");
        isActive = true;

        monster = GameObject.Find("TestTorsos").transform.GetChild(0).gameObject;
        monsterMesh = monster.GetComponentInChildren<MeshCollider>().gameObject;
        cam = Camera.main;
        crossHair = GameObject.Find("crossHair").GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        myCursor = GameObject.Find("crossHair").GetComponent<RectTransform>();

        camTarget = monster;
        partList.Add(monster);

        partPool = FindObjectOfType<PartPool>();

        playerInput.actions["LeftStickBS"].performed += LeftStick;
        playerInput.actions["LeftStickBS"].canceled += LeftStickCancel;
        playerInput.actions["RightStickBS"].performed += RightStick;
        playerInput.actions["RightStickBS"].canceled += RightStickCancel;
        playerInput.actions["RightTriggerBS"].performed += CameraZoomIn;
        playerInput.actions["LeftTriggerBS"].performed += CameraZoomOut;
        playerInput.actions["RightTriggerBS"].canceled += RightTriggerCancel;
        playerInput.actions["LeftTriggerBS"].canceled += LeftTriggerCancel;
        playerInput.actions["SubmitBS"].performed += ClickA;
        playerInput.actions["CancelBS"].performed += ClickB;
        playerInput.actions["RightBumperBS"].performed += RightBumper;
        playerInput.actions["LeftBumperBS"].performed += LeftBumper;
        playerInput.actions["DPadRightBS"].performed += DPadRight;
        playerInput.actions["DPadLeftBS"].performed += DPadLeft;
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["LeftStickBS"].performed -= LeftStick;
        playerInput.actions["LeftStickBS"].canceled -= LeftStickCancel;
        playerInput.actions["RightStickBS"].performed -= RightStick;
        playerInput.actions["RightStickBS"].canceled -= RightStickCancel;
        playerInput.actions["RightTriggerBS"].performed -= CameraZoomIn;
        playerInput.actions["LeftTriggerBS"].performed -= CameraZoomOut;
        playerInput.actions["RightTriggerBS"].canceled -= RightTriggerCancel;
        playerInput.actions["LeftTriggerBS"].canceled -= LeftTriggerCancel;
        playerInput.actions["SubmitBS"].performed -= ClickA;
        playerInput.actions["CancelBS"].performed -= ClickB;
        playerInput.actions["RightBumperBS"].performed -= RightBumper;
        playerInput.actions["LeftBumperBS"].performed -= LeftBumper;
        playerInput.actions["DPadRightBS"].performed -= DPadRight;
        playerInput.actions["DPadLeftBS"].performed -= DPadLeft;
    }

    private void LeftStick(InputAction.CallbackContext context)
    {
        crosshairMove = context.ReadValue<Vector2>();
    }

    private void LeftStickCancel(InputAction.CallbackContext context)
    {
        crosshairMove = Vector2.zero;
    }

    private void RightStick(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        lookInput.x = Deadzone(lookInput.x);
        lookInput.y = Deadzone(lookInput.y);
    }

    private void RightStickCancel(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    private void ClickA(InputAction.CallbackContext context)
    {
        RaycastHit hit;

        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.collider is MeshCollider)
            {
                if (!hoverPart)
                {
                    var monsterPartLoad = Resources.Load<GameObject>("Build-A-Scare Parts/Arms/Arm 15");

                    if (!monsterPartLoad)
                    {
                        Debug.Log("error monster part dont exist");
                    }
                    else
                    {
                        GameObject monsterPart = Instantiate(monsterPartLoad);
                        monsterPart.transform.rotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
                        monsterPart.transform.position = hit.point;
                        //monsterPart.transform.parent = monster.transform;
                        hoverPart = true;
                        partToHover = monsterPart;
                        hoverIsHead = false;
                        SetLayerRecursively(partToHover, 2);
                    }
                } else
                {
                    partToHover.transform.parent = hit.transform;
                    partList.Add(partToHover);
                    SetLayerRecursively(partToHover, 0);
                    hoverPart = false;
                    hoverIsHead = false;
                    partToHover = null;
                }
            } else { print("No mesh collider? " + hit.transform.gameObject.GetComponent<MeshCollider>()); }
        }
    }
    private void ClickB(InputAction.CallbackContext context)
    {
        RaycastHit hit;

        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.collider is MeshCollider)
            {
                if (!hoverPart)
                {
                    var monsterPartLoad = Resources.Load<GameObject>("Build-A-Scare Parts/Heads/Head 11");

                    if (!monsterPartLoad)
                    {
                        Debug.Log("error monster part dont exist");
                    }
                    else
                    {
                        GameObject monsterPart = Instantiate(monsterPartLoad);
                        monsterPart.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        monsterPart.transform.position = hit.point;
                        //monsterPart.transform.parent = monster.transform;
                        hoverPart = true;
                        partToHover = monsterPart;
                        hoverIsHead = true;
                        SetLayerRecursively(partToHover, 2);
                    }
                }
                else
                {
                    partToHover.transform.parent = hit.transform;
                    partList.Add(partToHover);
                    SetLayerRecursively(partToHover, 0);
                    hoverPart = false;
                    hoverIsHead = false;
                    partToHover = null;
                }
            }
            else { print("No mesh collider? " + hit.transform.gameObject.GetComponent<MeshCollider>()); }
        }
    }

    private void Update()
    {
        RotateMonster();
        MoveCrosshair();
        CameraZoom();

        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));
        Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);

        if (hoverPart)
        {
            RaycastHit hit;

            if (Physics.Raycast(myRay, out hit, 1000f))
            {
                if (hit.collider is MeshCollider)
                {
                    if (hoverIsHead)
                    {
                        partToHover.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    }
                    else
                    {
                        partToHover.transform.rotation = Quaternion.FromToRotation(Vector3.right, hit.normal);
                    }
                    partToHover.transform.position = hit.point;
                }
            }
        }
    }

    private void RotateMonster()
    {
        cam.transform.position = new Vector3(camTarget.transform.position.x, cam.transform.position.y, cam.transform.position.z);

        float delta = lookInput.y * rotSpeed * Time.deltaTime;

        float newAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

        float appliedRotation = newAngle - currentAngle;

        cam.transform.RotateAround(camTarget.transform.position, Vector3.right * 0.5f, appliedRotation);
        monster.transform.Rotate(Vector3.up, lookInput.x * -rotSpeed * Time.deltaTime, Space.World);

        currentAngle = newAngle;
    }

    private void MoveCrosshair()
    {
        crossHair.anchoredPosition += crosshairMove * crosshairSpeed * Time.deltaTime;

        Vector2 pos = crossHair.anchoredPosition;

        float xOffset = canvas.rect.xMax / 10f;
        float yOffset = canvas.rect.yMax / 5f;
        pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin + xOffset, canvas.rect.xMax - xOffset);
        pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin + yOffset, canvas.rect.yMax - yOffset);

        crossHair.anchoredPosition = pos;
    }
    private float Deadzone(float x)
    {
        if (x > 0f)
        {
            if (x < stickMin)
            {
                x = 0f;
            }
        }
        else if (x < 0f)
        {
            if (x > -stickMin)
            {
                x = 0f;
            }
        }

        return x;
    }

    private void CameraZoomIn(InputAction.CallbackContext context)
    {
        isHoldingRight = true;
    }

    private void RightTriggerCancel(InputAction.CallbackContext context)
    {
        isHoldingRight = false;
    }

    private void CameraZoomOut(InputAction.CallbackContext context)
    {
        isHoldingLeft = true;
    }

    private void LeftTriggerCancel(InputAction.CallbackContext context)
    {
        isHoldingLeft = false;
    }
    private void CameraZoom()
    {
        if (isHoldingRight)
        {
            float newFOV = cam.fieldOfView - (zoomSpeed * Time.deltaTime);

            cam.fieldOfView = Mathf.Max(newFOV, minFOV);
        }

        if (isHoldingLeft)
        {
            float newFOV = cam.fieldOfView + (zoomSpeed * Time.deltaTime);

            cam.fieldOfView = Mathf.Min(newFOV, maxFOV);
        }
    }

    private void RightBumper(InputAction.CallbackContext context)
    {
        int index = 0;

        for (int i = 0; i < partList.Count; i++)
        {
            if (partList[i] == camTarget)
            {
                index = i;
            }
        }

        if (index + 1 < partList.Count)
        {
            camTarget = partList[index + 1];
        } else
        {
            camTarget = partList[0];
        }

        cam.transform.rotation = new Quaternion();
        cam.transform.position = new Vector3(camTarget.transform.position.x, camTarget.transform.position.y, cam.transform.position.z);
    }

    private void LeftBumper(InputAction.CallbackContext context)
    {
        int index = 0;

        for (int i = 0; i < partList.Count; i++)
        {
            if (partList[i] == camTarget)
            {
                index = i;
            }
        }

        if (index - 1 >= 0)
        {
            camTarget = partList[index - 1];
        }
        else
        {
            camTarget = partList[partList.Count - 1];
        }

        cam.transform.rotation = new Quaternion();
        cam.transform.position = new Vector3(camTarget.transform.position.x, camTarget.transform.position.y, cam.transform.position.z);
    }

    private void DPadRight(InputAction.CallbackContext context)
    {

    }

    private void DPadLeft(InputAction.CallbackContext context)
    {

    }
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}