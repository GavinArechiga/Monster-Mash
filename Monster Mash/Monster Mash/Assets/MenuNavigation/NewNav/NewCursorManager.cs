using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NewCursorManager : MonoBehaviour
{
    private playerIndexManager playerManager;
    private tempSceneLoader sceneLoader;

    [SerializeField] private RectTransform[] cursors;
    [SerializeField] private GameObject[] playerWindows;
    [SerializeField] private Vector2[] startPos;
    [SerializeField] private Transform[] selectionPos;
    [SerializeField] private MonstroUICursor[] controllers;

    public RectTransform canvas;
    [SerializeField] public GameObject confirmButton;
    private monstroUIManager uiManager;
    [SerializeField] private GameObject roster;

    [SerializeField] private int nextScene = 6;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<playerIndexManager>();
        uiManager = FindObjectOfType<monstroUIManager>();
        sceneLoader = FindObjectOfType<tempSceneLoader>();

        controllers = new MonstroUICursor[4];

        startPos = new Vector2[cursors.Length];

        for (int i = 0; i < cursors.Length; i++)
        {
            startPos[i] = cursors[i].anchoredPosition;
            cursors[i].gameObject.SetActive(false);
        }
    }
    public void AddPlayer(MonstroUICursor player)
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

    public void RemovePlayer(MonstroUICursor player)
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
            sceneLoader.monsterCombo1();
            StartCoroutine("Wait3Seconds");
        }
    }

    private IEnumerator Wait3Seconds()
    {
        yield return new WaitForSeconds(3f);

        uiManager.SwitchControlsOff();

        yield return new WaitForSeconds(3f);

        uiManager.LoadSelectedStage();

        yield break;
    }

    public void ResetCursorPos(MonstroUICursor player)
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
}
