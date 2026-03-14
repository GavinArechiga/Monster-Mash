using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    private PlayerManager manager;

    [SerializeField] private RectTransform[] cursors;
    [SerializeField] private GameObject[] playerWindows;
    [SerializeField] private Vector2[] startPos;
    [SerializeField] private Transform[] selectionPos;
    [SerializeField] private PlayerCharacterSelectController[] controllers;

    public RectTransform canvas;
    [SerializeField] public GameObject confirmButton;
    private SceneController sceneController;

    [SerializeField] private GameObject roster;

    [SerializeField] public bool thisIsCharacterSelect = true;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PlayerManager>();
        sceneController = FindObjectOfType<SceneController>();

        controllers = new PlayerCharacterSelectController[4];

        startPos = new Vector2[cursors.Length];

        for (int i = 0; i < cursors.Length; i++)
        {
            startPos[i] = cursors[i].anchoredPosition;
            cursors[i].gameObject.SetActive(false);
        }
    }
    public void AddPlayer(PlayerCharacterSelectController player)
    {
        int index = FindFirstNull();
        controllers[index] = player;
        cursors[index].gameObject.SetActive(true);
        player.myCursor = cursors[index];
        player.window = playerWindows[index];
        player.window.transform.GetChild(0).gameObject.SetActive(true);
        player.window.transform.GetChild(1).gameObject.SetActive(false);
        player.selectionPos = selectionPos[index];
        CheckAllPlayersSelected();
    }
    public void RemovePlayer(PlayerCharacterSelectController player)
    {
        if (cursors[0])
        {
            int index = 3;

            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] == player)
                {
                    index = i;
                    break;
                }
            }

            cursors[index].anchoredPosition = startPos[index];
            cursors[index].gameObject.SetActive(false);
            player.window.transform.GetChild(0).gameObject.SetActive(false);
            player.window.transform.GetChild(1).gameObject.SetActive(true);
            controllers[index] = null;
            CheckAllPlayersSelected();
        }
    }

    private int FindFirstNull()
    {
        int index = 3;

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
            {
                return i;
            }
        }

        return index;
    }

    public void CheckAllPlayersSelected()
    {
        int qualifyingPlayers = 0;

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] != null)
            {
                if (controllers[i].selection == null)
                {
                    confirmButton.SetActive(false);
                    return;
                }
                else
                {
                    qualifyingPlayers++;
                }
            }
        }

        if (qualifyingPlayers > 0)
        {
            confirmButton.SetActive(true);
        }
    }

    public void Confirmed()
    {
        if (controllers[0])
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i]?.selection)
                {
                    Player player = controllers[i].GetComponentInParent<Player>();
                    if (player.GetCharacter())
                    {
                        player.DestroyCharacter();
                    }
                    player.SetCharacter(controllers[i].selection);
                    player.GetCharacter().transform.parent = player.transform;
                    player.GetCharacter().SetActive(false);
                }
            }

            foreach (PlayerInput player in PlayerInput.all)
            {
                print("here is a player: " + player);
                if (!player.GetComponentInChildren<PlayerCharacterSelectController>())
                {
                    print("this plater is useless: " + player);
                    player.enabled = false;
                }
            }

            sceneController.LoadScene(5);
        }
    }

    public void ResetCursorPos(PlayerCharacterSelectController player)
    {
        if (cursors[0])
        {
            int index = 3;

            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] == player)
                {
                    index = i;
                    break;
                }
            }

            cursors[index].anchoredPosition = startPos[index];
        }
    }

    public GameObject RandomCharacter()
    {
        Transform[] characters = roster.GetComponentsInChildren<Transform>();

        int x = Random.Range(0, characters.Length - 1);

        return characters[x].gameObject;
    }
}
