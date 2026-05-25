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

    public BAS_CurrentPart bas_currentPart = new();
    private Collider currTorso;


    private List<Collider> toTurnBackOn = new List<Collider>();
    private List<Collider> toTurnBackOff = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        partList.Add(monster);
        toolWheel = FindObjectOfType<ToolWheel>();
        bas_currentPart.SetCurrTorso(monster);
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
        if (!movePart) return;
            RaycastHit hit;

        if (Physics.Raycast(myRay, out hit, 1000f))
        {
            Debug.Log(hit.collider.gameObject);

            if (hit.collider.GetComponent<WhichPartType>()?.type is PartType.Torso or PartType.Head)
            {
                partToEdit.transform.position = hit.point;
            }
        }

        /*if (!movePart) return;

        Vector3 worldpos = cursor.position;

        Ray myRay = Camera.main.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldpos));
        Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);

        Plane plane = new Plane(Camera.main.transform.forward * -1f, new Vector3(0, 0, -5f));

        if (plane.Raycast(myRay, out float enter))
        {
            Vector3 cursorWorldPoint = myRay.GetPoint(enter);

            Vector3 closestPoint = bas_currentPart.GetCurrTorso().ClosestPoint(cursorWorldPoint);
            float closestDist = Vector3.Distance(cursorWorldPoint, closestPoint);

            List<Collider> allHeads = bas_currentPart.GetAllHeads();

            if (allHeads.Count > 0)
            {
                for (int i = 0; i < allHeads.Count; i++)
                {
                    Vector3 headPoint = allHeads[i].ClosestPoint(cursorWorldPoint);

                    float headDist = Vector3.Distance(cursorWorldPoint, headPoint);

                    if (headDist < closestDist)
                    {
                        closestDist = headDist;
                        closestPoint = headPoint;
                    }
                }
            }

        partToEdit.transform.position = closestPoint;
        }*/
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

            if (partToEdit.GetComponentInChildren<WhichPartType>().type is PartType.Head)
            {
                bas_currentPart.AddHead(partToEdit);
            }

            editPart = false;
            setToolBools();

        }
        else if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.collider.GetComponent<WhichPartType>())
            {
                if (hit.collider.GetComponent<WhichPartType>()?.type is PartType.Torso or PartType.Head)
                {
                    if (!editPart)
                    {
                        InstantiatePart(hit);
                    }
                }
                else if (hit.collider.GetComponent<WhichPartType>()?.type is not PartType.Torso or PartType.Head)
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
            DontIgnorePartRaycast();
        }
        else if (currTool == 0)
        {
            movePart = true;
            scalePart = false;
            rotatePart = false;

            IgnorePartRaycast();
        }
        else if (currTool == 1)
        {
            movePart = false;
            scalePart = true;
            rotatePart = false;
            DontIgnorePartRaycast();
        }
        else if (currTool == 2)
        {
            movePart = false;
            scalePart = false;
            rotatePart = true;

            SetUpRotGizmo();
            DontIgnorePartRaycast();
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

            CorrectLimbColliders(monsterPart);
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
        float dir = 0;
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

        dir *= Mathf.Abs(total);

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


    private void CorrectLimbColliders(GameObject limb)
    {
        Collider[] allCols = limb.GetComponentsInChildren<Collider>();

        foreach (Collider col in allCols)
        {
            if (col.GetComponent<WhichPartType>())
            {
                col.enabled = true;
                toTurnBackOff.Add(col);
            }
            else
            {
                col.enabled = false;
                toTurnBackOn.Add(col);
            }
        }
    }

    private void ResetAllLimbColliders()
    {
        for (int i = 0; i < toTurnBackOff.Count; i++)
        {
            toTurnBackOff[i].enabled = false;
        }

        for (int i = 0; i < toTurnBackOn.Count; i++)
        {
            toTurnBackOn[i].enabled = true;
        }
    }

    void IgnorePartRaycast()
    {
        WhichPartType[] partCols = partToEdit.GetComponentsInChildren<WhichPartType>();

        foreach (WhichPartType col in partCols)
        {
            col.gameObject.layer = 2; //ignore raycast
        }
    }

    void DontIgnorePartRaycast()
    {
        WhichPartType[] partCols = partToEdit.GetComponentsInChildren<WhichPartType>();

        foreach (WhichPartType col in partCols)
        {
            col.gameObject.layer = 0;
        }
    }
}
