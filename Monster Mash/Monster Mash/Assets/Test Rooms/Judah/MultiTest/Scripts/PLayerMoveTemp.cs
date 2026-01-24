using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PLayerMoveTemp : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    [SerializeField]
    private bool clickerMode = false; //mouse or hand, player is in menu navigation mode, not actively a monster or in combat

    private Scene UIscene;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public void OnMove(InputValue input)
    {
        moveInput = input.Get<Vector2>();
    }

    public void OnClick()
    {
        if (clickerMode)
        {

        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        Debug.Log("Active scene changed from " + oldScene.name + " to " + newScene.name);
        // Your code here to run when the scene changes
        SwitchPlayerMode();
    }

    private void SwitchPlayerMode()
    {
        if (UIscene == SceneManager.GetActiveScene())
        {
            PlayerUIMode();
        }
        else
        {
            PlayerMonsterMode();
        }
    }

    private void PlayerUIMode()
    {
        clickerMode = true;
    }

    private void PlayerMonsterMode()
    {
        clickerMode = false;
    }
}
