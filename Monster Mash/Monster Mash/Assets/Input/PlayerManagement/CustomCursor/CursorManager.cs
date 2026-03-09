using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PlayerManager>();

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
        int index = 3;

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == player)
            {
                index = i;
            }
        }

        cursors[index].anchoredPosition = startPos[index];
        cursors[index].gameObject.SetActive(false);
        player.window.transform.GetChild(0).gameObject.SetActive(false);
        player.window.transform.GetChild(1).gameObject.SetActive(true);
        controllers[index] = null;
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
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] != null && controllers[i].selection == null)
            {
                confirmButton.SetActive(false);
                return;
            }
        }

        confirmButton.SetActive(true);
    }
}
