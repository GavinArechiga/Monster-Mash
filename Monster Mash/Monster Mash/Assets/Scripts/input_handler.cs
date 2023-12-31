using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Reflection;

public class input_handler : MonoBehaviour
{
    [SerializeField]
    private int playerIndex = 0;

    private PlayerInput playerInput;

    [Header("----------------Gamepad Controller Support----------------")]
    public List<availableControllerInputs> currentControllerMap;
    //here we would create a private list to hold edited controller maps before they are updated to default
    //here is where we would create a public array of texts from which the edited controller map derives its data from
    public List<availableControllerInputs> defaultControllerMap_Menu;
    public List<availableControllerInputs> defaultControllerMap_Combat;

    public List<availableKeyboardInputs> currentKeyboardMap;

    //Judah Added some items hehe
    [SerializeField] private Controller1 player;
    [SerializeField] private CustomCursor myCursor;
    private string controlType;
    private bool mapHasSwitched = false;

    private void Awake()
    {
        if (FindObjectOfType<Controller1>())
        {
            player = FindObjectOfType<Controller1>();
        }

        playerInput = GetComponent<PlayerInput>();
        //i changed this line because it was ruining my life
        //game_manager gameManager = GameObject.Find("Game Manager").GetComponent<game_manager>();
        game_manager gameManager = FindObjectOfType<game_manager>();
        gameManager.activePlayers.Add(this);
        for (int i = 0; i < gameManager.activePlayers.Count; i++)
        {
            if (gameManager.activePlayers[i] == this)
            {
                playerIndex = i;
                this.gameObject.name = "Player" + " " + (i + 1);
                break;
            }
        }
    }

    //Establishes the correct button inputs available, whether in menus or combat and if a certain control set is being used
    #region Switching Input Actions and Action Maps

    public void controllerSetUp()
    {
        if (!mapHasSwitched)
        {
            mapHasSwitched = true;
            //print("current map: " + playerInput.currentActionMap);
            //switch input from starter to controller
            playerInput.SwitchCurrentActionMap("XBOX");
            controlType = "XBOX";
        }
    }
    public void keyboardMouseSetUp()
    {
        if (!mapHasSwitched)
        {
            mapHasSwitched = true;
            //print("trace: " + UnityEngine.StackTraceUtility.ExtractStackTrace());
            //switch input from starter to keyboard/mouse
            playerInput.SwitchCurrentActionMap("keyboardmouse");
            controlType = "keyboardmouse";
            //print("current map: " + playerInput.currentActionMap);
        }
    }

    public void controller_SwitchToMenuActions()
    {
        for (int i = 0; i < defaultControllerMap_Menu.Count; i++)
        {
            currentControllerMap[i].inputFunction = defaultControllerMap_Menu[i].inputFunction;
        }
    }

    public void controller_SwitchToCombatActions()
    {
        for (int i = 0; i < defaultControllerMap_Combat.Count; i++)
        {
            currentControllerMap[i].inputFunction = defaultControllerMap_Combat[i].inputFunction;
        }
    }
    #endregion

    //A translator for communication between what our player presses and what actions take place on screen
    #region Gamepad Inputs

    public void A_button(CallbackContext context)
    {
        MethodInfo methodInfo = GetType().GetMethod(currentControllerMap[0].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void B_button(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[1].inputFunction, 0f);
        }
    }

    public void X_button(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[2].inputFunction, 0f);
        }
    }

    public void Y_button(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[3].inputFunction, 0f);
        }
    }

    public void leftTrigger(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[4].inputFunction, 0f);
        }
    }

    public void rightTrigger(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[5].inputFunction, 0f);
        }
    }
    public void leftBumper(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[6].inputFunction, 0f);
        }
    }

    public void rightBumper(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[7].inputFunction, 0f);
        }
    }

    public void leftJoystick(CallbackContext context)
    {
        MethodInfo methodInfo = GetType().GetMethod(currentControllerMap[8].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void rightJoystick(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[9].inputFunction, 0f);
        }
    }

    public void Dpad_UP(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[10].inputFunction, 0f);
        }
    }

    public void Dpad_DOWN(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[11].inputFunction, 0f);
        }
    }

    public void Dpad_LEFT(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[12].inputFunction, 0f);
        }
    }

    public void Dpad_RIGHT(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentControllerMap[13].inputFunction, 0f);
        }
    }
    #endregion

    //A translator for communication between what our player presses and what actions take place on screen
    #region Keyboard/Mouse Inputs

    public void W_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[0].inputFunction, 0f);
        }
    }

    public void Horizontal_key(CallbackContext context) //A + D = horizontal axis
    {
        MethodInfo methodInfo = GetType().GetMethod(currentKeyboardMap[1].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void A_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[2].inputFunction, 0f);
        }
    }

    public void S_key(CallbackContext context)
    {
        MethodInfo methodInfo = GetType().GetMethod(currentKeyboardMap[3].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void D_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[4].inputFunction, 0f);
        }
    }

    public void Spacebar_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[5].inputFunction, 0f);
        }
    }

    public void Q_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[6].inputFunction, 0f);
        }
    }

    public void E_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[7].inputFunction, 0f);
        }
    }
    public void R_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[8].inputFunction, 0f);
        }
    }

    public void F_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[9].inputFunction, 0f);
        }
    }
    public void LeftShift_key(CallbackContext context)
    {
        MethodInfo methodInfo = GetType().GetMethod(currentKeyboardMap[10].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void LeftControl_key(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[11].inputFunction, 0f);
        }
    }

    public void Left_mouse(CallbackContext context)
    {
        MethodInfo methodInfo = GetType().GetMethod(currentKeyboardMap[12].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void Middle_mouse(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[13].inputFunction, 0f);
        }
    }

    public void Right_mouse(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[14].inputFunction, 0f);
        }
    }

    public void Move_mouse(CallbackContext context)
    {
        MethodInfo methodInfo = GetType().GetMethod(currentKeyboardMap[15].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }

    public void numPad_1(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[16].inputFunction, 0f);
        }
    }

    public void numPad_2(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[17].inputFunction, 0f);
        }
    }

    public void numPad_3(CallbackContext context)
    {
        if (context.started)
        {
            Invoke(currentKeyboardMap[18].inputFunction, 0f);
        }
    }

    /*public void Vertical_key(CallbackContext context) //A + D = horizontal axis
    {
        MethodInfo methodInfo = GetType().GetMethod(currentKeyboardMap[19].inputFunction, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo != null)
        {
            methodInfo.Invoke(this, new object[] { context });
        }
    }*/
    #endregion

    //A library of menu functions called by inputs. Essentially all the tangible events
    #region Menu Interaction
    public void sayHi()
    {
        print("hello!");
    }

    public void sayBye()
    {
        print("goodbye!");
    }

    public void moveCursor(CallbackContext context)
    {
        if (controlType == "XBOX")
        {
            Vector2 moveInput = context.ReadValue<Vector2>().normalized;

            if (GetComponent<CustomCursor>())
            {
                GetComponent<CustomCursor>().MoveCursor(moveInput);
            }
        }
        else if (controlType == "keyboardmouse")
        {
            Vector2 moveInput = context.ReadValue<Vector2>().normalized / 2;

            if (GetComponent<CustomCursor>())
            {
                GetComponent<CustomCursor>().MoveCursor(moveInput);
            }
        }
    }

    public void selectButton(CallbackContext context)
    {
        if (context.canceled)
        {
            myCursor.SetButtonHeld(false);
        }
        else if (context.started)
        {
            myCursor.SetButtonHeld(true);
            myCursor.Select();
        }
    }
    #endregion

    //A library of combat functions called by inputs. Essentially all the tangible events
    #region Combat Interaction
    private void runPlayer(CallbackContext context)
    {
        print("runninging");
        player.SetIsRun(context.performed);
    }
    private void movePlayer(CallbackContext context) //with left stick, move player either right or left
    {
        if (controlType == "XBOX")
        {
            int dir = 0;

            Vector2 moveInput = context.ReadValue<Vector2>();

            if (moveInput.x < 0)
            {
                dir = -1;
            }
            else if (moveInput.x > 0)
            {
                dir = 1;
            }

            player.Move(dir);
        }
        else if (controlType == "keyboardmouse") // A and D make horizontal Axis
        {
            float moveInput = context.ReadValue<float>();

            int dir = 0;

            if (moveInput < 0)
            {
                dir = -1;
            }
            else if (moveInput > 0)
            {
                dir = 1;
            }

            player.Move(dir);
        }
    }

    private void platformDrop(CallbackContext context)
    {
        if (controlType == "XBOX")
        {
            int dir = 0;

            Vector2 moveInput = context.ReadValue<Vector2>();

            if (moveInput.y < 0)
            {
            }

            player.Move(dir);
        }
        else if (controlType == "keyboardmouse")
        {
            bool myInput = false;

            if (context.performed)
            {
                myInput = true;
            }

            player.CanPlatformDrop(myInput);
        }
    }

    private void jumpPlayer()
    {
        player.Jump();
    }

    private void attackPlayerContext(CallbackContext context)
    {
        if (context.started)
        {
            //player.Attack1();
        }
    }

    private void attack1()
    {
        player.Attack1();
    }

    private void attack2()
    {
        player.Attack2();
    }

    private void attack3()
    {
        player.Attack3();
    }
    #endregion

}


//This is the magic that allows us to rename elements in lists
[System.Serializable]
public class availableControllerInputs
{
    public string inputName;
    public string inputFunction;
}

[System.Serializable]

public class availableKeyboardInputs
{
    public string inputName;
    public string inputFunction;
}


