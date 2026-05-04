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
    private Vector2 crosshairMove;

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

    private float minScale = 0.5f;
    private float maxScale = 2f;
    private float scaleSpeed = 1.5f;

    private float rotSpeed = 50f;

    private ToolWheel toolWheel;

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
        MoveCursor();
        PreviewPart();
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
        cursor.anchoredPosition += crosshairMove * crosshairSpeed * Time.deltaTime;

        Vector2 pos = cursor.anchoredPosition;

        float xOffset = 0f;//canvas.rect.xMax / 10f;
        float yOffset = 0f;//canvas.rect.yMax / 5f;
        pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin + xOffset, canvas.rect.xMax - xOffset);
        pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin + yOffset, canvas.rect.yMax - yOffset);

        cursor.anchoredPosition = pos;
    }

    public void PlacePart()
    {
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
            if (hit.collider is MeshCollider)
            {
                if (!editPart)
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
                        currTool = 0;
                        toolWheel.SetToolWheel(currTool);
                        setToolBools();
                        SetLayerRecursively(partToEdit, 2);

                        currPotentialParent = hit.transform;
                        partToEdit.transform.parent = currPotentialParent;
                    }
                }
            }
            else { print("No mesh collider? " + hit.transform.gameObject.GetComponent<MeshCollider>()); }
        } else if (!editPart)
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

    public void SetCrossHairMove(Vector2 value)
    {
        crosshairMove = value;
    }

    public void SetSelectedPart(string newPart)
    {
        selectedPart = newPart;
    }

    public void CancelEdit()
    {
        if (editPart)
        {
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

        scale += Vector3.one * scaleSpeed * Time.deltaTime;

        if (scale.x <= maxScale)
        {
            partToEdit.transform.localScale = scale;
        }
    }

    private void Shrink()
    {
        Vector3 scale = partToEdit.transform.localScale;

        scale -= Vector3.one * scaleSpeed * Time.deltaTime;

        if (scale.x >= minScale)
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
}
