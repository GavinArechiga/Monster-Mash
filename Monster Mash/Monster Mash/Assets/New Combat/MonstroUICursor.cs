using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MonstroUICursor : MonoBehaviour
{
    private bool isActive = false;

    public RectTransform myCursor;
    private MonstroCursorManager cManager;
    private RectTransform canvas;
    public GameObject window;
    [SerializeField] public GameObject selection;
    public Transform selectionPos;
    [SerializeField] private Vector3 selectionOGPos;

    private monstroUIManager uiManager;
    private Vector2 move = new Vector2();
    private float speed = 400f;
    public void Start()
    {
        isActive = true;
        cManager = FindObjectOfType<MonstroCursorManager>();
        cManager.AddPlayer(this);
        canvas = cManager.canvas;
        uiManager = FindObjectOfType<monstroUIManager>();

        cManager.CheckAllPlayersSelected();
    }
    private void Navigate(Vector2 value)
    {
        move = value;
    }

    private void NavigateCancel()
    {
        move = Vector2.zero;
    }

    private void Submit()
    {
        RaycastHit hit;

        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.transform.GetComponent<CapsuleCollider>())
            {
                Select(hit.transform.gameObject);
            }
            else if (hit.transform.GetComponent<BoxCollider>() && hit.transform.GetComponent<BoxCollider>().enabled)
            {
                if (hit.transform.gameObject.name == "BackButton")
                {
                    hit.transform.GetComponent<BoxCollider>().enabled = false;
                    uiManager.LoadScene(2);
                }
            }
        }
    }

    private void Cancel()
    {
        if (selection)
        {
            selection.transform.position = selectionOGPos;
            selection.GetComponent<CapsuleCollider>().enabled = true;
            selection = null;
            cManager.CheckAllPlayersSelected();
        }
    }

    private void Confirm()
    {
        if (cManager.confirmButton.activeSelf)
        {
            cManager.confirmButton.SetActive(false);
            cManager.Confirmed();
        }
    }
    private void Move()
    {
        if (myCursor)
        {
            myCursor.anchoredPosition += move * speed * Time.deltaTime;

            Vector2 pos = myCursor.anchoredPosition;

            pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin, canvas.rect.xMax);
            pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin, canvas.rect.yMax);

            myCursor.anchoredPosition = pos;
        }
    }

    private void Update()
    {
        Move();
        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));
        Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);
    }

    private void Select(GameObject obj)
    {
        if (selection)
        {
            selection.transform.position = selectionOGPos;
            selection.GetComponent<CapsuleCollider>().enabled = true;
        }

        selection = obj;
        selectionOGPos = selection.transform.position;
        selection.transform.position = selectionPos.position;
        selection.GetComponent<CapsuleCollider>().enabled = false;
        cManager.CheckAllPlayersSelected();
        cManager.ResetCursorPos(this);
    }

    #region input actions

    //bool performed if true, false if canceled
    public void CursorNavigate(bool performed, Vector2 value)
    {
        if (performed)
        {
            Navigate(value);
        }
        else
        {
            NavigateCancel();
        }
    }

    public void CursorSubmit()
    {
        Submit();
    }

    public void CursorCancel()
    {
        Cancel();
    }

    public void CursorConfirm()
    {
        Confirm();
    }
    #endregion
}
