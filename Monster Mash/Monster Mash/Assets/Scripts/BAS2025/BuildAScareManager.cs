using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;
public class BuildAScareManager : MonoBehaviour
{
    public static BuildAScareManager instance { get; private set; }
    enum CurrentBASScreen { StartScreen, PartSelector, PartPlacer, MenuHub}
    public enum ControlMode { DirectControl, DragAndDrop}

    enum PartControlMode { Move, Rotate, Scale}

    enum Axis { X,Y,Z};

    [Header("General")]

    CurrentBASScreen currentScreen = CurrentBASScreen.StartScreen;

    ControlMode controlMode;

    List<MonsterData> currentMonsters = new List<MonsterData>();

    EventSystem eventSystem;

    MonsterData currentMonster = new MonsterData();

    PlayerInput input;

    [SerializeField]
    Animator stateDrivenCam;

    [Header("Start Screen")]

    [SerializeField]
    MonsterSelectIconBAS[] monsterSelectButtons;

    [SerializeField]
    CanvasGroup startScreenUI;

    [SerializeField]
    CanvasGroup partSelectionUI;

    [Header("Part Selector")]

    [SerializeField]
    Toggle[] partTypeSelectors;

    [SerializeField]
    CanvasGroup sideTabs;

    [SerializeField]
    CanvasGroup[] partTypeParent;

    [SerializeField]
    Transform partSpawnPoint;

    [SerializeField]
    Animator partSelectorAnim;

    int currentPartType;

    bool partSelectorOpen;

    [Header("Part Placer")]

    [SerializeField]
    Transform partMovementEmpty;

    [SerializeField]
    Transform partRotationEmpty;

    [SerializeField]
    Transform partScaleEmpty;

    GameObject currentPartObj;

    Rigidbody partMovementRB;

    Rigidbody partRotationRB;

    [SerializeField]
    GameObject[] moveModeVisuals;

    InputAction partLocomotion;

    bool partSelected = false;

    bool torsoSelected;

    bool firstPart;

    bool zoneSelection = false;

    PartControlMode partLocomotionMode;

    [SerializeField]

    VirturalCursorControl virtualCursor;

    Bounds buildAScareBound;

    Bounds torsoBounds;

    BASTorsoController monsterTorso;

    int torsoLimbIndex = 0;

    int flippedModifier = 1;

    List<Coroutine> locomotionCoroutines;

    Axis rotAxis = Axis.X;

    Vector3 rotationControl;

    [SerializeField]
    CinemachineDollyCart cameraCart;

    [SerializeField]
    List<float> cameraPosNumb;

    Vector2 zoomClamp = new Vector2(40f, 30f);

    bool partZoom;

    [SerializeField]
    LayerMask connectionMask;

    string connectionTag = "ConnectionCheck";

    Transform connectionCheckObj;

    [SerializeField]
    CinemachineVirtualCamera[] mainScreenCameras;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            instance = this;
        }

        eventSystem = FindObjectOfType<EventSystem>();

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        input = FindObjectOfType<PlayerInput>();

        partMovementRB = partMovementEmpty.GetComponent<Rigidbody>();

        partRotationRB = partRotationEmpty.GetComponent<Rigidbody>();

        buildAScareBound = new Bounds(partSpawnPoint.position, new Vector3(2, 4, 2));

        torsoBounds = new Bounds(partSpawnPoint.position, new Vector3(0, 1, 0));

       for(int i = 0; i < monsterSelectButtons.Length; i++)
        {
            if(currentMonsters.Count >= 1 && currentMonsters[i] != null)
            {
                monsterSelectButtons[i].IconSetUp(true, currentMonsters[i]);
            }

            else
            {
                monsterSelectButtons[i].IconSetUp(false, null);
            }
        }

        locomotionCoroutines = new List<Coroutine>();

        //Ray testRay = new Ray(transform.position, transform.forward);

        //print(testRay.GetPoint(10));

        
    }

    public void StartNewBuildAScare()
    {
        eventSystem.SetSelectedGameObject(null);

        //Temp

        SetControlMode(ControlMode.DirectControl);

        //

        DisableCanvasGroup(startScreenUI);

        currentScreen = CurrentBASScreen.PartSelector;

        firstPart = true;

        currentMonster = new MonsterData();

        currentMonster.monsterParts = new List<MonsterPartData>();

        OpenPartSelector(4);
    }

    public void SpawnPart(bool isTorso, GameObject partPrefab)
    {
        if(!partSelected)
        {
            if (isTorso)
            {
                GameObject torso = Instantiate(partPrefab, partSpawnPoint.position, partPrefab.transform.rotation);

                if(currentMonster.monsterParts.Count < 1)
                {
                    monsterTorso = torso.GetComponent<BASTorsoController>();
                }

                currentPartObj = torso;

                torso.transform.parent = partScaleEmpty;

                partSelected = true;

                partLocomotionMode = PartControlMode.Move;

                moveModeVisuals[(int)partLocomotionMode].SetActive(true);

                /*
                if(controlMode == ControlMode.DirectControl)
                {
                    StartCoroutineList(locomotionCoroutines, PartLocomotionControl());
                }

                */

                torsoSelected = true;
            }

            else
            {
                if(controlMode == ControlMode.DirectControl)
                {
                    torsoLimbIndex = 0;

                    partMovementEmpty.position = monsterTorso.limbPoints[torsoLimbIndex].position;

                    GameObject part = Instantiate(partPrefab, partMovementEmpty.position, partPrefab.transform.rotation);

                    part.transform.parent = partScaleEmpty;

                    currentPartObj = part;

                    //Temp will find a better solution

                    foreach(Transform child in currentPartObj.transform)
                    {
                        if(child.gameObject.tag == connectionTag)
                        {
                            connectionCheckObj = child;
                        }
                    }

                    zoneSelection = true;

                }

                else
                {
                     /*
                      
                    GameObject part = Instantiate(partPrefab, partSpawnPoint.position, partPrefab.transform.rotation);

                    currentPartObj = part;

                    part.transform.parent = partMovementEmpty;

                    partSelected = true;

                    torsoSelected = false;

                    */
                }

            }

            if (controlMode == ControlMode.DirectControl)
            {
                ClosePartSelector();
            }
        }

    }

    void DirectControlEvents(bool status)
    {
        if(status)
        {
            InputActionMap directControl = input.actions.FindActionMap("BuildAScare-DirectControl");

            partLocomotion = directControl.FindAction("Part Locomotion");

            directControl.FindAction("Open Part Selector").performed += OpenPartSelectorInput;

            directControl.FindAction("Rotate - Left").performed += RotateLeft;

            directControl.FindAction("Rotate - Left").canceled += RotateLeft;

            directControl.FindAction("Snap - Left").performed += SnapLeft;

            directControl.FindAction("Rotate - Right").performed += RotateRight;

            directControl.FindAction("Rotate - Right").canceled += RotateRight;

            directControl.FindAction("Snap - Right").performed += SnapRight;

            directControl.FindAction("Select - Left").performed += DirectControlSelectLeft;

            directControl.FindAction("Select - Right").performed += DirectControlSelectRight;

            directControl.FindAction("Zoom - In").performed += ZoomIn;

            directControl.FindAction("Zoom - In").canceled += ZoomIn;

            directControl.FindAction("Zoom - Out").performed += ZoomOut;

            directControl.FindAction("Zoom - Out").canceled += ZoomOut;

            directControl.FindAction("Confirm").performed += Confirm;

            directControl.FindAction("Back").performed += Back;

            directControl.FindAction("Toggle").performed += Toggle;

            directControl.FindAction("SecondaryToggle").performed += SecondaryToggle;
        }

        else
        {
            InputActionMap directControl = input.actions.FindActionMap("BuildAScare-DirectControl");

            partLocomotion = null;

            directControl.FindAction("Open Part Selector").performed -= OpenPartSelectorInput;

            directControl.FindAction("Rotate - Left").performed -= RotateLeft;

            directControl.FindAction("Rotate - Left").canceled -= RotateLeft;

            directControl.FindAction("Snap - Left").performed -= SnapLeft;

            directControl.FindAction("Rotate - Right").performed -= RotateRight;

            directControl.FindAction("Rotate - Right").canceled -= RotateRight;

            directControl.FindAction("Snap - Right").performed -= SnapRight;

            directControl.FindAction("Select - Left").performed -= DirectControlSelectLeft;

            directControl.FindAction("Select - Right").performed -= DirectControlSelectRight;

            directControl.FindAction("Zoom - In").performed -= ZoomIn;

            directControl.FindAction("Zoom - In").canceled -= ZoomIn;

            directControl.FindAction("Zoom - Out").performed -= ZoomOut;

            directControl.FindAction("Zoom - Out").canceled -= ZoomOut;

            directControl.FindAction("Confirm").performed -= Confirm;

            directControl.FindAction("Back").performed -= Back;

            directControl.FindAction("Toggle").performed -= Toggle;

            directControl.FindAction("SecondaryToggle").performed -= SecondaryToggle;
        }
    }

    void DragAndDropEvents(bool status)
    {
        if(status)
        {
            //Subscribe Events

            virtualCursor.EnableCursor();
        }

        else
        {
            //Unsubscribe Events

            virtualCursor.DisableCursor();
        }
    }

    public void SetControlMode(ControlMode mode)
    {
        controlMode = mode;

        input.SwitchCurrentActionMap("BuildAScare-" + mode.ToString());

        switch(mode)
        {
            case ControlMode.DirectControl:

                DirectControlEvents(true);

                break;

            case ControlMode.DragAndDrop:

                DragAndDropEvents(true);

                break;
        }
    }

    public void ChangeControlMode(ControlMode mode)
    {
        controlMode = mode;

        input.SwitchCurrentActionMap("BuildAScare-" + mode.ToString());

        switch (mode)
        {
            case ControlMode.DirectControl:

                DragAndDropEvents(false);

                DirectControlEvents(true);

                break;

            case ControlMode.DragAndDrop:

                DirectControlEvents(false);

                DirectControlEvents(true);

                break;
        }
    }

    void DisableCanvasGroup(CanvasGroup group)
    {
        group.alpha = 0;

        group.interactable = false;

        switch(controlMode)
        {
            case ControlMode.DirectControl:

                eventSystem.SetSelectedGameObject(null);

                break;

            case ControlMode.DragAndDrop:

                group.blocksRaycasts = false;

                break;
        }
    }

    void OpenPartSelector(int partType)
    {
        partSelectorAnim.SetTrigger("open");

        SetCamAnimTrigger("psOpen");

        partSelectionUI.interactable = true;

        switch (controlMode)
        {
            case ControlMode.DirectControl:

                GameObject firstSelectedPart = partTypeParent[partType].transform.GetChild(0).GetChild(0).gameObject;

                eventSystem.SetSelectedGameObject(firstSelectedPart);

                break;

            case ControlMode.DragAndDrop:

                partSelectionUI.blocksRaycasts = true;

                partTypeParent[partType].blocksRaycasts = true;

                break;
        }

        //Maybe move these to the end of the animation

        partSelectorOpen = true;

        currentScreen = CurrentBASScreen.PartSelector;

        OpenPartTypeMenu(partType);
    }

    void OpenPartTypeMenu(int index)
    {
        partTypeSelectors[index].GetComponent<Animator>().SetTrigger("Pressed");

        partTypeParent[index].interactable = true;

        partTypeParent[index].alpha = 1;

        currentPartType = index;
    }

    public void PartTypeMenuToggle(int index)
    {
        foreach(CanvasGroup group in partTypeParent)
        {
            group.interactable = false;

            group.alpha = 0;

            if(controlMode == ControlMode.DragAndDrop)
            {
                group.blocksRaycasts = false;
            }
        }

        eventSystem.SetSelectedGameObject(null);

        partTypeSelectors[index].GetComponent<Animator>().SetTrigger("Pressed");

        partTypeParent[index].interactable = true;

        partTypeParent[index].alpha = 1;

        switch(controlMode)
        {
            case ControlMode.DirectControl:

                GameObject firstSelectedPart = partTypeParent[index].transform.GetChild(0).GetChild(0).gameObject;

                eventSystem.SetSelectedGameObject(firstSelectedPart);

                break;

            case ControlMode.DragAndDrop:

                partTypeParent[index].blocksRaycasts = true;

                break;
        }

        currentPartType = index;
    }

    void ClosePartTypeMenu(int index)
    {
        partTypeSelectors[index].GetComponent<Animator>().SetTrigger("Normal");

        partTypeParent[index].interactable = false;

        partTypeParent[index].alpha = 0;
    }

    void ClosePartSelector()
    {
        partSelectorOpen = false;

        partSelectorAnim.SetTrigger("close");

        currentScreen = CurrentBASScreen.PartPlacer;

        SetCamAnimTrigger("psClose");

        ClosePartTypeMenu(currentPartType);

        partSelectionUI.interactable = false;

        switch (controlMode)
        {
            case ControlMode.DirectControl:

                eventSystem.SetSelectedGameObject(null);

                break;

            case ControlMode.DragAndDrop:

                partSelectionUI.blocksRaycasts = false;

                partTypeParent[currentPartType].blocksRaycasts = true;

                break;
        }
    }

    void SetCamAnimTrigger(string trigger)
    {
        stateDrivenCam.SetTrigger(trigger);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PartLocomotion();
    }

    private void LateUpdate()
    {
        PartClamp();
    }

    bool ConnectionCheck(Transform partConnectionPoint)
    {
        Collider[] connectors = Physics.OverlapSphere(partConnectionPoint.position, 0.075f, connectionMask);

        if(connectors.Length >= 1)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if(partSelected && !torsoSelected)
        {
            Gizmos.DrawSphere(connectionCheckObj.position, 0.05f);
        }
    }

    #region Direct Control Functions

    void StartCoroutineList(List<Coroutine> crs, IEnumerator function)
    {
        Coroutine cr = StartCoroutine(function);

        crs.Add(cr);
    }

    IEnumerator PartLocomotionControl()
    {
        PartLocomotion();

        yield return new WaitForEndOfFrame();

        PartClamp();

        Coroutine cr = StartCoroutine(PartLocomotionControl());

        locomotionCoroutines.Add(cr);
    }

    void StopCoroutineList(List<Coroutine> crs)
    {
        foreach(Coroutine cr in crs)
        {
            StopCoroutine(cr);
        }
    }

    void PartLocomotion()
    {
        if(partSelected && controlMode == ControlMode.DirectControl)
        {
            Vector2 partControl = partLocomotion.ReadValue<Vector2>();

            switch (partLocomotionMode)
            {
                case PartControlMode.Move:

                    DirectControlMovement(partControl);

                    break;

                case PartControlMode.Rotate:

                    DirectControlRotation(partControl);

                    break;

                case PartControlMode.Scale:

                    DirectControlScale(partControl);

                    break;
            }
        }
    }

    void DirectControlMovement(Vector2 partControl)
    {
        Vector3 verticalMovementHandler = new Vector3(0, partControl.y, 0);

        Vector3 horizontalMovementHandler = Camera.main.transform.right * partControl.x;

        Vector3 movementControl = horizontalMovementHandler + verticalMovementHandler;

        partMovementRB.MovePosition(partMovementEmpty.transform.position + movementControl * Time.fixedDeltaTime * 5);
    }

    void DirectControlRotation(Vector2 partControl)
    {
        if(!torsoSelected)
        {
            switch (rotAxis)
            {
                case Axis.X:

                    rotationControl = new Vector3(partControl.y, 0, 0);

                    break;

                case Axis.Y:

                    rotationControl = new Vector3(0, -partControl.x, 0);

                    break;

                case Axis.Z:

                    rotationControl = new Vector3(0, 0, partControl.x);

                    break;
            }

            Quaternion deltaRot = Quaternion.Euler(rotationControl * Time.fixedDeltaTime * 50);

            partRotationRB.MoveRotation(partRotationEmpty.rotation * deltaRot);
        }

    }

    void DirectControlScale(Vector2 partControl)
    {
        Vector3 scaleControl = new Vector3(partControl.x/2 * flippedModifier, partControl.x/2, partControl.x/2);

        partScaleEmpty.transform.localScale += scaleControl;
    }

    void PartClamp()
    {
        if(partSelected && controlMode == ControlMode.DirectControl)
        {
            switch (partLocomotionMode)
            {
                case PartControlMode.Move:

                    MovementClamp();

                    break;

                case PartControlMode.Rotate:

                    RotationClamp();

                    break;

                case PartControlMode.Scale:

                    ScaleClamp();

                    break;
            }


        }
    }

    void MovementClamp()
    {
        if (torsoSelected)
        {
            Vector3 clampedObjectPos = partMovementEmpty.transform.position;

            clampedObjectPos.x = Mathf.Clamp(clampedObjectPos.x, torsoBounds.min.x, torsoBounds.max.x);

            clampedObjectPos.y = Mathf.Clamp(clampedObjectPos.y, torsoBounds.min.y, torsoBounds.max.y);

            clampedObjectPos.z = Mathf.Clamp(clampedObjectPos.z, torsoBounds.min.z, torsoBounds.max.z);

            partMovementEmpty.transform.position = clampedObjectPos;
        }

        else
        {
            Vector3 clampedObjectPos = partMovementEmpty.transform.position;

            clampedObjectPos.x = Mathf.Clamp(clampedObjectPos.x, buildAScareBound.min.x, buildAScareBound.max.x);

            clampedObjectPos.y = Mathf.Clamp(clampedObjectPos.y, buildAScareBound.min.y, buildAScareBound.max.y);

            clampedObjectPos.z = Mathf.Clamp(clampedObjectPos.z, buildAScareBound.min.z, buildAScareBound.max.z);

            partMovementEmpty.transform.position = clampedObjectPos;
        }
    }

    void RotationClamp()
    {

    }

    void ScaleClamp()
    {
        if(torsoSelected)
        {
            Vector3 clampedObjectScale = partScaleEmpty.localScale;

            clampedObjectScale.x = Mathf.Clamp(clampedObjectScale.x, 0.75f, 1.25f);

            clampedObjectScale.y = Mathf.Clamp(clampedObjectScale.y, 0.75f, 1.25f);

            clampedObjectScale.z = Mathf.Clamp(clampedObjectScale.z, 0.75f, 1.25f);

            partScaleEmpty.localScale = clampedObjectScale;
        }

        else
        {
            Vector3 clampedObjectScale = partScaleEmpty.localScale;

            if (flippedModifier == 1)
            {
                clampedObjectScale.x = Mathf.Clamp(clampedObjectScale.x, 0.75f, 1.25f);
            }

            else
            {
                clampedObjectScale.x = Mathf.Clamp(clampedObjectScale.x, -1.25f, -0.75f);
            }

            clampedObjectScale.y = Mathf.Clamp(clampedObjectScale.y, 0.75f, 1.25f);

            clampedObjectScale.z = Mathf.Clamp(clampedObjectScale.z, 0.75f, 1.25f);

            partScaleEmpty.localScale = clampedObjectScale;
        }
    }

    void OpenPartSelectorInput(InputAction.CallbackContext context)
    {
        if(!partSelected && controlMode == ControlMode.DirectControl)
        {
           if(!firstPart)
            {
                if(!partSelectorOpen)
                {
                    OpenPartSelector(currentPartType);
                }

                else
                {
                    ClosePartSelector();
                }
            }

        }
    }

    IEnumerator CameraZoom(int modifier)
    {
        float  lensModifer = 0.05f;

        lensModifer *= modifier;

        while(partZoom)
        {
            foreach(CinemachineVirtualCamera cam in mainScreenCameras)
            {
                cam.m_Lens.FieldOfView += lensModifer;

                float currentFov = cam.m_Lens.FieldOfView;

                currentFov = Mathf.Clamp(currentFov, zoomClamp.y, zoomClamp.x);

                cam.m_Lens.FieldOfView = currentFov;
            }

            yield return null;
        }

        StopCoroutine(CameraZoom(modifier));
    }

    void RotateLeft(InputAction.CallbackContext context)
    {
        if(currentScreen == CurrentBASScreen.PartPlacer)
        {
            if (context.performed)
            {
                cameraCart.m_Speed = -0.1f;
            }

            else if (context.canceled)
            {
                cameraCart.m_Speed = 0;
            }
        }
    }

    void SnapLeft(InputAction.CallbackContext context)
    {
        if (currentScreen == CurrentBASScreen.PartPlacer)
        {


            if(Mathf.Approximately(cameraCart.m_Position, 0.75f))
            {
                cameraCart.m_Position = cameraPosNumb[2];
            }

            else if (cameraCart.m_Position > 0.75f && !Mathf.Approximately(cameraCart.m_Position, 0.75f))
            {
                cameraCart.m_Position = cameraPosNumb[3];
            }

            else
            {
                int snapIndex = 3;

                float dist = 0;

                for (int i = 0; i < cameraPosNumb.Count; i++)
                {
                    float diff = (cameraPosNumb[i] - cameraCart.m_Position);

                    if (Mathf.Sign(diff) != 1 && Mathf.Approximately(diff, 0) == false)
                    {
                        if (dist == 0)
                        {
                            dist = diff;

                            snapIndex = i;
                        }

                        else
                        {
                            if (diff > dist)
                            {
                                dist = diff;

                                snapIndex = i;
                            }
                        }
                    }
                }

                cameraCart.m_Position = cameraPosNumb[snapIndex];
            }
        }
    }

    void RotateRight(InputAction.CallbackContext context)
    {
        if(currentScreen == CurrentBASScreen.PartPlacer)
        {
            if (context.performed)
            {
                cameraCart.m_Speed = 0.1f;
            }

            else if (context.canceled)
            {
                cameraCart.m_Speed = 0;
            }
        }
    }

    void SnapRight(InputAction.CallbackContext context)
    {
        if (currentScreen == CurrentBASScreen.PartPlacer)
        {
            if(cameraCart.m_Position >= 0.75f)
            {
                cameraCart.m_Position = cameraPosNumb[0];
            }

            else
            {
                int snapIndex = 0;

                float dist = 0;

                for(int i = 0; i < cameraPosNumb.Count; i++)
                {
                    float diff = (cameraPosNumb[i] - cameraCart.m_Position);

                    if(Mathf.Sign(diff) != -1 && diff != 0)
                    {
                        if(dist == 0)
                        {
                            dist = diff;

                            snapIndex = i;
                        }

                        else
                        {
                            if(diff < dist)
                            {
                                dist = diff;

                                snapIndex = i;
                            }
                        }
                    }
                }

                cameraCart.m_Position = cameraPosNumb[snapIndex];
            }
        }
    }

    void DirectControlSelectLeft(InputAction.CallbackContext context)
    {
        if (zoneSelection && currentScreen == CurrentBASScreen.PartPlacer)
        {
            torsoLimbIndex--;

            if (torsoLimbIndex < 0)
            {
                torsoLimbIndex = monsterTorso.limbPoints.Length - 1;
            }

            partMovementEmpty.position = monsterTorso.limbPoints[torsoLimbIndex].position;
        }

        else if(!partSelectorOpen && currentScreen == CurrentBASScreen.PartSelector)
        {
            if(currentMonster.monsterParts.Count >= 2)
            {

            }
        }
    }

    void DirectControlSelectRight(InputAction.CallbackContext context)
    {
        if(zoneSelection && currentScreen == CurrentBASScreen.PartPlacer)
        {
            torsoLimbIndex++;

            if (torsoLimbIndex >= monsterTorso.limbPoints.Length)
            {
                torsoLimbIndex = 0;
            }

            partMovementEmpty.position = monsterTorso.limbPoints[torsoLimbIndex].position;
        }

    }

    void ZoomIn(InputAction.CallbackContext context)
    {
        if(currentScreen == CurrentBASScreen.PartPlacer)
        {
            if (context.performed)
            {
                partZoom = true;

                StartCoroutine(CameraZoom(-1));
            }

            else if (context.canceled)
            {
                partZoom = false;

                //StopCoroutine(CameraZoom(-1));
            }
        }

    }

    void ZoomOut(InputAction.CallbackContext context)
    {
        if (currentScreen == CurrentBASScreen.PartPlacer)
        {
            if (context.performed)
            {
                partZoom = true;

                StartCoroutine(CameraZoom(1));
            }

            else if (context.canceled)
            {
                partZoom = false;

                //StopCoroutine(CameraZoom(1));
            }
        }
    }

    void Confirm(InputAction.CallbackContext context)
    {
        if (controlMode == ControlMode.DirectControl)
        {
            if(partSelected)
            {
                PartPlacement(); 
            }

            else if(zoneSelection)
            {
                zoneSelection = false;

                partSelected = true;

                moveModeVisuals[(int)partLocomotionMode].SetActive(true);

                //StartCoroutineList(locomotionCoroutines, PartLocomotionControl());
            }
        }
    }

    void PartPlacement()
    {
        if (torsoSelected)
        {
            PlaceTorso();
        }

        else
        {
            PlacePart();
        }

        rotAxis = Axis.X;
    }

    void PlacePart()
    {
        if(ConnectionCheck(connectionCheckObj))
        {
            currentPartObj.transform.parent = partSpawnPoint.transform;

            MonsterPartData part = currentPartObj.GetComponent<TempPartData>().monsterPart;

            part.partPosition = currentPartObj.transform.localPosition;

            part.partRotation = currentPartObj.transform.localRotation;

            part.partScale = currentPartObj.transform.localScale;

            currentMonster.monsterParts.Add(part);

            currentPartObj = null;

            partSelected = false;

            moveModeVisuals[(int)partLocomotionMode].SetActive(false);

            partMovementEmpty.transform.position = partSpawnPoint.transform.position;

            partRotationEmpty.transform.rotation = partSpawnPoint.transform.rotation;

            partScaleEmpty.transform.localScale = partSpawnPoint.transform.localScale;
        }

        else
        {
            //Drop
        }

    }

    void PlaceTorso()
    {
        currentPartObj.transform.parent = partSpawnPoint.transform;

        MonsterPartData part = currentPartObj.GetComponent<TempPartData>().monsterPart;

        part.partPosition = currentPartObj.transform.localPosition;

        part.partRotation = currentPartObj.transform.localRotation;

        part.partScale = currentPartObj.transform.localScale;

        currentMonster.monsterParts.Add(part);

        currentPartObj = null;

        partSelected = false;

        moveModeVisuals[(int)partLocomotionMode].SetActive(false);

        partMovementEmpty.transform.position = partSpawnPoint.transform.position;

        partRotationEmpty.transform.rotation = partSpawnPoint.transform.rotation;

        partScaleEmpty.transform.localScale = partSpawnPoint.transform.localScale;

        if (controlMode == ControlMode.DirectControl)
        {
            currentPartType = 0;

            //OpenPartSelector(currentPartType);
        }

        if (firstPart)
        {
            firstPart = false;

            sideTabs.interactable = true;
        }

        torsoSelected = false;

    }

    void Back(InputAction.CallbackContext context)
    {

    }

    void Toggle(InputAction.CallbackContext context)
    {
        if(partSelected && controlMode == ControlMode.DirectControl)
        {
            int currentLocomotionMode = (int)partLocomotionMode;

            moveModeVisuals[currentLocomotionMode].SetActive(false);

            currentLocomotionMode++;

            if (currentLocomotionMode > 2)
            {
                currentLocomotionMode = 0;
            }

            partLocomotionMode = (PartControlMode)currentLocomotionMode;

            moveModeVisuals[currentLocomotionMode].SetActive(true);
        }
    }

    void SecondaryToggle(InputAction.CallbackContext context)
    {
        if(controlMode == ControlMode.DirectControl)
        {
            if(partSelected)
            {
                switch(partLocomotionMode)
                {
                    case PartControlMode.Move:

                        //Maybe Mirror?

                        break;

                    case PartControlMode.Rotate:

                        ToggleRotationAxis();

                        break;

                    case PartControlMode.Scale:

                        FlipPart();

                        break;
                }
            }
        }
    }
    
    void ToggleRotationAxis()
    {
        if(!torsoSelected)
        {
            int currentAxis = (int)rotAxis;

            currentAxis++;

            if (currentAxis > 2)
            {
                currentAxis = 0;
            }

            rotAxis = (Axis)currentAxis;
        }

    }

    void FlipPart()
    {
        if(!torsoSelected)
        {
            partScaleEmpty.transform.localScale = new Vector3(-partScaleEmpty.transform.localScale.x,
            partScaleEmpty.transform.localScale.y,
            partScaleEmpty.transform.localScale.z);
            currentPartObj.GetComponent<TempPartData>().monsterPart.isFlipped = !currentPartObj.GetComponent<TempPartData>().monsterPart.isFlipped;

            flippedModifier *= -1;
        }
    }

    #endregion
}
