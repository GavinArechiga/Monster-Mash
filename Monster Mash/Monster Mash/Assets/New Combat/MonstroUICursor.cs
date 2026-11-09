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
    
    private Camera myCam;
    private bool leftPress; //register left joystick left press as a single input
    private bool rightPress; //register left joystick right press as a single input
    private float pressThreshold = 0.4f;
    private float interval = 2.150f;
    private bool scrollControlMode = true; //before a character is selected, the control mode for scrolling through the character options
    private float targetDepth = 14f;
    private bool moveCharacterWithCursor = false;
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

    private void SelectCharacter()
    {
        RaycastHit hit;

        if (Physics.Raycast(myCam.transform.position, myCam.transform.forward, out hit, 20f))
        {
            if (hit.transform.GetComponent<CapsuleCollider>())
            {
                scrollControlMode = false;
                cManager.LoadMonster();

                moveCharacterWithCursor = true;
                transform.position = selectionPos.position;
            }
        }
    }

    private void PlaceCharacter()
    {
        selection = this.gameObject;
        moveCharacterWithCursor = false;
        cManager.CheckAllPlayersSelected();
    }

    private void Cancel()
    {
        if (selection)
        {
            //selection.transform.position = selectionOGPos;
            //selection.GetComponent<CapsuleCollider>().enabled = true;
            selection = null;
            moveCharacterWithCursor = true;
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


            if (moveCharacterWithCursor)//move monster with cursor
            {
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, myCursor.position);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, targetDepth));

                transform.position = worldPosition;
            }
        }
    }

    private void Update()
    {
        if (scrollControlMode)
        { 
            //code
        }
        else
        {
            Move();
            Vector3 worldPos = myCursor.position;
            Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));
            Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);
        }
    }
    private void OldSelect(GameObject obj)
    {
        selection = obj;
        selectionOGPos = selection.transform.position;
        selection.transform.position = selectionPos.position;
        selection.GetComponent<CapsuleCollider>().enabled = false;
        cManager.CheckAllPlayersSelected();
        //cManager.ResetCursorPos(this);
    }

    public void SetCamera(Camera cam)
    {
        myCam = cam;
        myCam.enabled = true;
    }
    private void MoveCamRight()
    {
        myCam.transform.position += new Vector3(interval, 0.0f, 0.0f);
    }

    private void MoveCamLeft()
    {
        myCam.transform.position -= new Vector3(interval, 0.0f, 0.0f);
    }

    #region input actions

    //bool performed if true, false if canceled
    public void CursorNavigate(bool performed, Vector2 value)
    {
        if (scrollControlMode)
        {
            float moveX = value.x;

            if (moveX >= pressThreshold)
            {
                if (!rightPress)
                {
                    MoveCamRight();
                    rightPress = true;
                }
            }
            else
            {
                rightPress = false;
            }

            if (moveX <= -pressThreshold)
            {
                if (!leftPress)
                {
                    MoveCamLeft();
                    leftPress = true;
                }
            }
            else
            {
                leftPress = false;
            }
        }
        else
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
    }

    public void CursorSubmit()
    {
        if (scrollControlMode)
        {
            SelectCharacter();
        }
        else
        {
            PlaceCharacter();
        }
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
