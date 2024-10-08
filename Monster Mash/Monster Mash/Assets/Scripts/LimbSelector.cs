using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.XR;

public class LimbSelector : MonoBehaviour
{
    public struct State
    {
        public enum States
        {
            noSelection,
            limbSelected,
            onTorso
        }

        States currState;
        public void ChangeState(States state)
        {
            currState = state;
        }

        public States GetState() { return currState; }
    }

    State limbState;

    public Camera editorCamera;
    public Camera mainCamera;

    [SerializeField] GameObject cursor_control;

    [HideInInspector]
    public bool cameraRotating = false;

    [HideInInspector]
    public GameObject limbToPlace = null;

    private Vector3 limbHomePos = Vector3.zero;
    private Quaternion limbHomeRotation = Quaternion.identity;

    private bool firstIntersectTorso = true;

    // Start is called before the first frame update
    void Awake()
    {
        limbState.ChangeState(State.States.noSelection);
    }

    // Update is called once per frame
    void Update()
    {
        switch (limbState.GetState())
        {
            case State.States.noSelection:
                NoLimbSelected();
                break;
            case State.States.limbSelected:
                LimbSelected();
                break;
            case State.States.onTorso:
                OnTorso();
                break;
            default:
                break;
        }
    }


    void NoLimbSelected()
    {
        // scenario when a limb has yet to be selected
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast and check if it hits a collider
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the collider has a reference to a Limb
                if (hit.collider.CompareTag("Limb"))
                {
                    //Raycast collided with the monster part's "Cube" collider, therefore take
                    //the cube's parent (i.e. the monster part) and make it the selected prefab
                    SelectionManager.Instance.SetSelectedPrefab(hit.collider.transform.parent.gameObject);

                    //Assign the prefab to the local Limb
                    limbToPlace = SelectionManager.Instance.GetSelectedPrefab();

                    // the collider is necessary for raycasting, but once the part has been selected,
                    // it should no longer have a collider as that may cause problems
                    //hit.collider.enabled = false;
                    limbToPlace.GetComponentInChildren<Collider>().enabled = false;

                    //change what the MainCamera is looking at
                    //mainCamera.cullingMask = LayerMask.GetMask("UI");
                    //editorCamera.cullingMask = LayerMask.GetMask("Default", "Torso", "Parts");

                    //StartCoroutine(MoveLimbToCenter());

                    //cursor_control.GetComponent<LimbOnTorso>().limbSelected = true;

                    limbHomePos = limbToPlace.transform.parent.position;
                    limbHomeRotation = limbToPlace.transform.parent.rotation;

                    limbState.ChangeState(State.States.limbSelected);

                    Debug.Log("limbAssigned: " + limbToPlace.name);
                }
            }
        }
    }

    private void LimbSelected()
    {
        limbToPlace.transform.parent.position = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 70f));

        Ray ray = editorCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast and check if it hits a collider
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the collider has a reference to the GameObject you want to detect clicks on
            if (hit.collider.CompareTag("Limbable"))
            {
                if (firstIntersectTorso)
                {
                    // change layer so that the editor camera is looking at the body part
                    SetGameLayerRecursive(limbToPlace.transform.parent.gameObject, LayerMask.NameToLayer("Player"));

                    firstIntersectTorso = false;

                    limbState.ChangeState(State.States.onTorso);
                }
            }
        }

        // mouse was clicked on screen other than on a torso will cause the monster part to
        // return to its home position
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //move back to home position
            StartCoroutine(MoveLimbHome());
            //change the state
            limbState.ChangeState(State.States.noSelection);
        }
    }

    private void OnTorso()
    {
        Ray torsoRay = editorCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(torsoRay, out hit))
        {
            if (hit.collider.CompareTag("Limbable"))
            {
                //limbToPlace.transform.parent.position = Vector3.Lerp(limbToPlace.transform.parent.position, hit.point, 0.15f);
                limbToPlace.transform.parent.position = Vector3.Lerp(limbToPlace.transform.parent.position, hit.point, 0.15f);
                limbToPlace.transform.parent.LookAt(-hit.normal);

                // Calculate the direction from the limb to the torso
                Vector3 directionToTorso = hit.transform.position - hit.point;

                // Determine the rotation to face the torso
                Quaternion targetRotation = Quaternion.LookRotation(directionToTorso, -hit.normal);

                targetRotation = Quaternion.RotateTowards(targetRotation, Quaternion.identity, 50);

                limbToPlace.transform.parent.rotation = targetRotation;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CreateLimb(hit, limbToPlace.transform.rotation);

                    //move back to home position
                    StartCoroutine(MoveLimbHome());

                    // reset torso intersection bool
                    firstIntersectTorso = true;

                    //Change the state to no limb selected
                    limbState.ChangeState(State.States.noSelection);
                }

            }
            else if (hit.collider.CompareTag("Ground"))
            {
                // change layer so that the editor camera is looking at the body part
                SetGameLayerRecursive(limbToPlace.transform.parent.gameObject, LayerMask.NameToLayer("UI"));

                firstIntersectTorso = true;

                limbState.ChangeState(State.States.limbSelected);
            }
        }
    }

    IEnumerator MoveLimbHome()
    {
        while (Vector3.Distance(limbToPlace.transform.position, limbHomePos) > 0.05f)
        {
            limbToPlace.transform.parent.position = Vector3.Lerp(limbToPlace.transform.parent.position, limbHomePos, 0.5f);
            limbToPlace.transform.parent.rotation = limbHomeRotation;
            yield return null;
        }

        // once a limb has returned home, reactivate its collider
        limbToPlace.GetComponentInChildren<MeshCollider>().enabled = true;

        // ensure that the main camera is the one looking at this limb
        SetGameLayerRecursive(limbToPlace.transform.parent.gameObject, LayerMask.NameToLayer("UI"));

        // no limb should be selected
        limbToPlace = null;

        yield return new WaitForSeconds(0.5f);
    }

    void CreateLimb(RaycastHit hit, Quaternion rot)
    {
        if (SelectionManager.Instance.GetSelectedPrefab())
        {
            GameObject newLimb = Instantiate(limbToPlace, hit.point, Quaternion.identity);

            newLimb.transform.localScale /= 13f;

            newLimb.transform.rotation = rot;

            Transform closestBone = FindClosestBone(hit);

            //trying this new thing
            //GameObject emptyParent = new GameObject();
            //emptyParent.transform.parent = closestBone;
            //emptyParent.transform.position = newLimb.transform.position;
            //emptyParent.transform.rotation = Quaternion.identity;

            newLimb.transform.parent = closestBone; //emptyParent.transform

            SelectionManager.Instance.SetSelectedPrefab(null);
        }
    }

    private void SetGameLayerRecursive(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            SetGameLayerRecursive(child.gameObject, layer);
        }
    }

    private Transform FindClosestBone(RaycastHit hit)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = hit.collider.GetComponent<SkinnedMeshRenderer>();
        Transform closestBone = null;
        float closestDistance = Mathf.Infinity;

        if (skinnedMeshRenderer != null)
        {
            Transform[] bones = skinnedMeshRenderer.bones;

            foreach (Transform bone in bones)
            {
                float distance = Vector3.Distance(bone.position, hit.point);
                if (distance < closestDistance)
                {
                    closestBone = bone;
                    closestDistance = distance;
                }
            }
        }

        return closestBone;
    }
}