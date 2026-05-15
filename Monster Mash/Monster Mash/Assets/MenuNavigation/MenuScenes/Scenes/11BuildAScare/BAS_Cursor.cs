using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BAS_Cursor : MonoBehaviour
{
    private RectTransform canvas;
    private Camera cam;

    private float crosshairSpeed = 1000f;
    private Vector2 leftStickValue;

    [SerializeField] private RectTransform cursor;

    [SerializeField] private bool editPart;
    private GameObject partToEdit;

    [SerializeField] private bool movePart = false;
    [SerializeField] private bool scalePart = false;
    [SerializeField] private bool rotatePart = false;
    private string[] tools = new string[] { "Move", "Scale", "Rotate" };
    [SerializeField] private int currTool = 0;

    private Transform currPotentialParent;

    private GameObject monsterMesh;
    private bool hoverIsArm = false;

    [SerializeField] private GameObject monster;

    [SerializeField] private List<GameObject> partList;

    private string selectedPart;

    private float minScale = 0.15f;
    private float maxScale = 1.5f;
    private Vector3 ogScale = Vector3.one;
    private float scaleSpeed = 2f;

    private float rotSpeed = 50f;

    private ToolWheel toolWheel;

    [SerializeField] private RotGizmo rotGizmo;
    [SerializeField] private bool usingRotGizmo = false;
    [SerializeField] private string currAxis;

    // Start is called before the first frame update
    void Start()
    {
        monsterMesh = monster.GetComponentInChildren<MeshCollider>().gameObject;
        cam = Camera.main;
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        partList.Add(monster);
        toolWheel = FindObjectOfType<ToolWheel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (usingRotGizmo)
        {
            RotateGizmo(leftStickValue);
        }
        else
        {
            MoveCursor();
            PreviewPart();
        }
    }

    private void PreviewPart()
    {
        Vector3 worldPos = cursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));
        Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);
        if (movePart)
        {
            RaycastHit hit;

            if (Physics.Raycast(myRay, out hit, 1000f))
            {
                if (hit.collider is MeshCollider)
                {
                    if (!hoverIsArm)
                    {
                        //partToHover.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    }
                    else
                    {
                        //partToHover.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    partToEdit.transform.position = hit.point;
                }
            }
        }
    }

    private void MoveCursor()
    {
        cursor.anchoredPosition += leftStickValue * crosshairSpeed * Time.deltaTime;

        Vector2 pos = cursor.anchoredPosition;

        float xOffset = 0f;//canvas.rect.xMax / 10f;
        float yOffset = 0f;//canvas.rect.yMax / 5f;
        pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin + xOffset, canvas.rect.xMax - xOffset);
        pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin + yOffset, canvas.rect.yMax - yOffset);

        cursor.anchoredPosition = pos;
    }

    public void AButton()
    {
        if (usingRotGizmo)
        {
            StopRotGizmo();
            return;
        }

        RaycastHit hit;

        Vector3 worldPos = cursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));

        if (editPart)
        {
            //partToEdit.transform.parent = currPotentialParent;
            partList.Add(partToEdit);

            editPart = false;
            setToolBools();

        }
        else if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.collider.GetComponent<WhichPartType>())
            {
                if (hit.collider.GetComponent<WhichPartType>()?.type is "Torso" or "Head")
                {
                    if (!editPart)
                    {
                        InstantiatePart(hit);
                    }
                }
                else if (hit.collider.GetComponent<WhichPartType>()?.type is not "Torso" or "Head")
                {
                    //code to select to edit monster part;
                }
            }
            else if(hit.collider.name is "X Axis" or "Y Axis" or "Z Axis")
            {
                currAxis = hit.collider.name;
                StartRotGizmo();
            }
            else { print("No WhichPartType script or anything else found " + hit.transform.gameObject); }
        }
        else if (!editPart)
        {
            UICast();
        }
    }

    private void UICast()
    {
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.position;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void LeftStickMove(Vector2 value)
    {
        leftStickValue = value;
    }

    public void SetSelectedPart(string newPart)
    {
        selectedPart = newPart;
    }

    public void BButton()
    {
        if (usingRotGizmo)
        {
            StopRotGizmo();
        }
        else if (editPart)
        {
            rotGizmo.Detach();
            Destroy(partToEdit);
            editPart = false;
            currTool = 0;
            toolWheel.SetToolWheel(currTool);
            setToolBools();
            partList.Remove(partToEdit);
        }
    }

    public void MirrorPreview()
    {
        if (movePart)
        {
            partToEdit.transform.localScale = new Vector3(partToEdit.transform.localScale.x * -1, partToEdit.transform.localScale.y, partToEdit.transform.localScale.z);
        }
    }

    public void RotatePreview()
    {
        if (movePart)
        {
            //partToEdit.transform.Rotate(Vector3.up, 1f);
        }
    }

    public void ToolWheelRight()
    {
        if (editPart && !toolWheel.GetWait())
        {
            if (currTool + 1 < tools.Length)
            {
                currTool++;
            }
            else
            {
                currTool = 0;
            }
            
            toolWheel.SetToolWheel(currTool);

            setToolBools();
        }
    }

    public void ToolWheelLeft()
    {
        if (editPart && !toolWheel.GetWait())
        {
            if (currTool - 1 >= 0)
            {
                currTool--;
            }
            else
            {
                currTool = tools.Length - 1;
            }
            
            toolWheel.SetToolWheel(currTool);

            setToolBools();
        }
    }

    private void setToolBools()
    {
        if (!editPart)
        {
            movePart = false;
            scalePart = false;
            rotatePart = false;
        }
        else if (currTool == 0)
        {
            movePart = true;
            scalePart = false;
            rotatePart = false;
        }
        else if (currTool == 1)
        {
            movePart = false;
            scalePart = true;
            rotatePart = false;
        }
        else if (currTool == 2)
        {
            movePart = false;
            scalePart = false;
            rotatePart = true;

            SetUpRotGizmo();
        }
    }

    public void DPadRight()
    {
        if (editPart)
        {
            if (scalePart)
            {
                Grow();
            }
            else if (rotatePart)
            {
                RotateRight();
            }
        }
    }

    public void DPadLeft()
    {
        if (editPart)
        {
            if (scalePart)
            {
                Shrink();
            }
            else if (rotatePart)
            {
                RotateLeft();
            }
        }
    }

    private void Grow()
    {
        Vector3 scale = partToEdit.transform.localScale;

        float scaleRate = ogScale.x / scaleSpeed;

        scale += Vector3.one * scaleRate * Time.deltaTime;

        if (scale.x <= ogScale.x * maxScale)
        {
            partToEdit.transform.localScale = scale;
        }
    }

    private void Shrink()
    {
        Vector3 scale = partToEdit.transform.localScale;

        float scaleRate = ogScale.x / scaleSpeed;

        scale -= Vector3.one * scaleRate * Time.deltaTime;

        if (scale.x >= ogScale.x * minScale)
        {
            partToEdit.transform.localScale = scale;
        }
    }

    private void RotateRight()
    {
        partToEdit.transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
    }

    private void RotateLeft()
    {
        partToEdit.transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);
    }

    private void SetUpRotGizmo()
    {
        rotGizmo.gameObject.SetActive(true);
        rotGizmo.SetUpGizmo(partToEdit);
    }

    private void InstantiatePart(RaycastHit hit)
    {
        var monsterPartLoad = Resources.Load<GameObject>(selectedPart);

        if (!monsterPartLoad)
        {
            Debug.Log("error monster part dont exist");
        }
        else
        {
            GameObject monsterPart = Instantiate(monsterPartLoad);
            monsterPart.transform.position = hit.point;
            editPart = true;
            partToEdit = monsterPart;
            ogScale = partToEdit.transform.localScale;
            currTool = 0;
            toolWheel.SetToolWheel(currTool);
            setToolBools();
            SetLayerRecursively(partToEdit, 2);

            currPotentialParent = hit.transform;
            partToEdit.transform.parent = currPotentialParent;
        }
    }

    private void StartRotGizmo()
    {
        usingRotGizmo = true;
    }

    private void StopRotGizmo()
    {
        usingRotGizmo = false;
    }

    private void RotateGizmo(Vector3 input)
    {
        int dir = 0;
        float total = input.x + input.y;

        Vector3 myRot = new Vector3();

        if (total > 0)
        {
            dir = 1;
        }
        else if (total < 0)
        {
            dir = -1;
        }

        if (currAxis == "X Axis")
        {
            myRot = rotGizmo.RotateX(dir);
        }
        else if(currAxis == "Y Axis")
        {
            myRot = rotGizmo.RotateY(dir);
        }
        else if (currAxis == "Z Axis")
        {
            myRot = rotGizmo.RotateZ(dir);
        }

        partToEdit.transform.Rotate(myRot, Space.Self);
    }
}
