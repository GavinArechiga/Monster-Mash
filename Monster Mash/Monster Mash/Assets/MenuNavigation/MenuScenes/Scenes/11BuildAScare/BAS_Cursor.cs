using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BAS_Cursor : MonoBehaviour
{
    [SerializeField] private Tools currTool = Tools.none;

    private RectTransform canvas;
    private Camera cam;

    private float crosshairSpeed = 1000f;
    private Vector2 leftStickValue;

    [SerializeField] private RectTransform cursor;

    [SerializeField] private bool editPart;
    [SerializeField] private GameObject partToEdit;

    [SerializeField] private bool movePart = false;
    [SerializeField] private bool rotatePart = false;

    private Transform currPotentialParent;

    [SerializeField] private GameObject monster;

    [SerializeField] private List<GameObject> partList;

    private string selectedPart;

    private float minScale = 0.15f;
    private float maxScale = 1.5f;
    private Vector3 ogScale = Vector3.one;
    private float scaleSpeed = 2f;

    private float rotSpeed = 50f;

    private Vector3 oldRot; //I have this so rotation can be canceled
    [SerializeField] private Vector3 oldPos = Vector3.zero; //^

    //private ToolWheel toolWheel;

    [SerializeField] private RotGizmo rotGizmo;
    [SerializeField] private bool usingRotGizmo = false;
    [SerializeField] private string currAxis;

    public BAS_CurrentPart bas_currentPart = new();
    private Collider currTorso;


    private List<Collider> toTurnBackOn = new List<Collider>();
    private List<Collider> toTurnBackOff = new List<Collider>();

    [SerializeField] private Camera rotCam;
    [SerializeField] private LayerMask partMask;
    [SerializeField] private LayerMask rotMask;

    private CursorArt cursorArt;
    [SerializeField] private GameObject editGizmos;

    private float floatingZ = -0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        partList.Add(monster);
        //toolWheel = FindObjectOfType<ToolWheel>();
        bas_currentPart.SetCurrTorso(monster);
        cursorArt = FindObjectOfType<CursorArt>();

        currTool = Tools.none;
    }

    // Update is called once per frame
    void Update()
    {
        if (currTool is not Tools.newPart && UICastCheck())
        {
            if (cursorArt.GetHand() is not Hands.open)
            {
                cursorArt.Change(Hands.open);
            }
        }
        else if (currTool is Tools.none && cursorArt.GetHand() is not Hands.point)
        {
            cursorArt.Change(Hands.point);
        }

        if (currTool is Tools.rotGizmo)
        {
            RotateGizmo(leftStickValue);
            return;
        }

        MoveCursor();

        if (currTool is Tools.newPart or Tools.move)
        {
            PreviewPart();
        }
        else if (currTool is Tools.rotate)
        { 
            cursorArt.Change(RotGizmoRaycast() ? Hands.open : Hands.point);
        }
    }

    private void PreviewPart()
    {
        Vector3 worldPos = cursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));
        
        Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);
        
        RaycastHit hit;

        if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.collider.GetComponent<WhichPartType>()?.type is PartType.Torso or PartType.Head)
            {
                partToEdit.transform.position = hit.point;
                partToEdit.transform.parent = hit.transform.parent;

                return;
            }
            else if (hit.collider.name == "Backdrop")
            {
                Vector3 newPos = hit.point;
                //newPos.z = floatingZ;
                partToEdit.transform.position = newPos;
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
        RaycastHit rotHit;
        RaycastHit hit;

        if (UICastCheck())
        {
            UICast();
        }
        else if (currTool is Tools.rotGizmo)
        {
            StopRotGizmo();
            StartRotateMode();
        }
        else if (currTool is Tools.rotate)
        {
            if (RotGizmoRaycast(out rotHit))
            {
                if (rotHit.collider.name is "X Axis" or "Y Axis" or "Z Axis")
                {
                    currAxis = rotHit.collider.name;
                    StartRotGizmo();
                    return;
                }
            }
        }
        else if (PartRaycast(out hit))
        {
            if (hit.collider.GetComponentInParent<WhichPartType>())
            {
                if (currTool is Tools.edit && (hit.collider.GetComponentInParent<WhichPartType>()?.type is not PartType.Torso) && partToEdit == hit.collider.GetComponentInParent<TempPartData>().gameObject)
                {
                    IgnorePartRaycast();
                    oldPos = partToEdit.transform.position;
                    StopEditMode();
                    StartMoveMode();
                    return;
                }

                if (hit.collider.GetComponentInParent<WhichPartType>()?.type is PartType.Torso or PartType.Head)
                {
                    if (currTool is Tools.newPart)
                    {
                        partToEdit.transform.parent = hit.transform.parent;
                        StopNewPartMode();
                        StartEditMode();
                        return;
                    }
                    else if (currTool is Tools.move)
                    {
                        partToEdit.transform.parent = hit.transform.parent;
                        StopMoveMode();
                        StartEditMode();
                    }
                }

                if (currTool is Tools.none)
                {
                    partToEdit = hit.collider.GetComponentInParent<TempPartData>().gameObject;
                    StartEditMode();
                }
            }
        }
        else if (currTool is Tools.edit)
        {
            StopEditMode();
            StartNoneMode();
            cursorArt.Change(Hands.point);
        }
    }


    private bool UICastCheck()
    {
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.position;

        List<RaycastResult> results = new List<RaycastResult>();

        foreach (GraphicRaycaster raycaster in raycasters)
        {
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<Button>())
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void UICast()
    {
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.position;

        List<RaycastResult> results = new List<RaycastResult>();

        foreach (GraphicRaycaster raycaster in raycasters)
        {
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.Invoke();
                    return;
                }
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
        StartNewPartMode();
    }

    public void BButton()
    {
        if (currTool is Tools.rotGizmo)
        {
            partToEdit.transform.eulerAngles = oldRot;
            StopRotGizmo();
        }
        else if (currTool is Tools.newPart)
        {
            partList.Remove(partToEdit);
            Destroy(partToEdit);
            partToEdit = null;
            StopNewPartMode();
            StartNoneMode();
        }
        else if (currTool is Tools.edit)
        {
            StopEditMode();
            StartNoneMode();
            return;
        }
        else if (currTool is Tools.move)
        {
            partToEdit.transform.position = oldPos;
            oldPos = Vector3.zero;
            DontIgnorePartRaycast();
            StartEditMode();
        }
        
    }

    public void MirrorPreview()
    {
        if (currTool is Tools.edit)
        {
            partToEdit.transform.localScale = new Vector3(partToEdit.transform.localScale.x * -1, partToEdit.transform.localScale.y, partToEdit.transform.localScale.z);
        }
    }

    public void RightBumper()
    {
        if (currTool is Tools.edit) Grow();
    }

    public void LeftBumper()
    {
        if (currTool is Tools.edit) Shrink();
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

    private void SetUpRotGizmo()
    {
        rotGizmo.gameObject.SetActive(true);
        rotGizmo.SetUpGizmo(partToEdit);
    }

    private void InstantiatePart()
    {
        var monsterPartLoad = Resources.Load<GameObject>(selectedPart);

        if (!monsterPartLoad)
        {
            Debug.Log("error monster part dont exist");
        }
        else
        {

            GameObject monsterPart = Instantiate(monsterPartLoad);
            partToEdit = monsterPart;
            ogScale = partToEdit.transform.localScale;

            if (partToEdit.GetComponentInParent<WhichPartType>()?.type is PartType.Torso or PartType.Head)
            {
                CorrectLimbColliders(monsterPart);
            }

            PreviewPart();
        }
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

        partToEdit?.transform.Rotate(myRot, Space.Self);
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
        foreach (Collider col in partToEdit?.GetComponentsInChildren<Collider>())
        {
            col.gameObject.layer = 2;
        }
    }

    void DontIgnorePartRaycast()
    {
        foreach (Collider col in partToEdit.GetComponentsInChildren<Collider>())
        {
            col.gameObject.layer = 0;
        }
    }

    private bool PartRaycast(out RaycastHit hit)
    {
        Vector3 worldPos = cursor.position;
        Ray myRay = cam.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f, partMask))
        {
            if (hit.collider.GetComponentInParent<WhichPartType>() || hit.collider.GetComponentInChildren<WhichPartType>())
            {
                return true;
            }
        }

        return false;
    }

    private bool PartRaycast()
    {
        Vector3 worldPos = cursor.position;
        RaycastHit hit;
        Ray myRay = cam.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f, partMask))
        {
            if (hit.collider.GetComponentInParent<WhichPartType>() || hit.collider.GetComponentInChildren<WhichPartType>())
            {
                return true;
            }
        }

        return false;
    }

    private bool RotGizmoRaycast(out RaycastHit hit)
    {
        Vector3 worldPos = cursor.position;
        Ray myRay = rotCam.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f, rotMask))
        {
            if (hit.collider.name is "X Axis" or "Y Axis" or "Z Axis")
            {
                return true;
            }
        }

        return false;
    }

    private bool RotGizmoRaycast()
    {
        Vector3 worldPos = cursor.position;
        RaycastHit hit;
        Ray myRay = rotCam.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(null, worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f, rotMask))
        {
            if (hit.collider.name is "X Axis" or "Y Axis" or "Z Axis")
            {
                return true;
            }
        }

        return false;
    }

    #region tools
    private void StartNoneMode()
    {
        currTool = Tools.none;
    }
    //
    private void StartNewPartMode()
    {
        cursorArt.Change(Hands.closed);
        currTool = Tools.newPart;
        InstantiatePart();
        IgnorePartRaycast();
    }
    private void StopNewPartMode()
    {

    }
    //
    private void StartEditMode()
    {
        currTool = Tools.edit;
        cursorArt.Change(Hands.point);
        editGizmos.SetActive(true);
        DontIgnorePartRaycast();
        SwapPartLayer(true);
    }
    private void StopEditMode()
    {
        cursorArt.Change(Hands.point);
        editGizmos.SetActive(false);
    }
    //
    private void StartMoveMode()
    {
        currTool = Tools.move;
        SwapPartLayer(false);
        IgnorePartRaycast();
    }
    private void StopMoveMode()
    {
        DontIgnorePartRaycast();
        SwapPartLayer(true);
    }
    //
    private void StartRecolorMode()
    {
        currTool = Tools.recolor;
    }
    private void StopRecolorMode()
    {

    }
    //
    private void StartScaleMode()
    {
        currTool = Tools.scale;
    }
    private void StopScaleMode()
    {

    }
    //
    public void StartRotateMode()
    {
        currTool = Tools.rotate;
        SetUpRotGizmo();
    }
    private void StopRotateMode()
    {
        rotGizmo.Detach();
    }
    //
    private void StartRotGizmo()
    {
        currTool = Tools.rotGizmo;
        cursorArt.Change(Hands.closed);
        oldRot = partToEdit.transform.eulerAngles;
    }

    private void StopRotGizmo()
    {
        cursorArt.Change(Hands.point);
    }
    #endregion

    private void SwapPartLayer(bool under)
    {
        int i = partToEdit.layer;

        if (under && i != 20)
        {
            SwapChildren(partToEdit.transform, 20);
        }
        else if (!under && i != 0)
        {
            SwapChildren(partToEdit.transform, 0);
        }
    }

    private void SwapChildren(Transform target, int x)
    {
        foreach (Transform child in target)
        {
            child.gameObject.layer = x;

            SwapChildren(child, x);
        }
    }
}
