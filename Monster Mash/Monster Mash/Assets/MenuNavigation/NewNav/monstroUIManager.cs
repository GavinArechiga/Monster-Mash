using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum controlScheme
{
    fight,
    hub,
    UIbuttons,
    UIcursor
}
public class monstroUIManager : MonoBehaviour
{
    public string[] myScenes;

    static int selectedStage;

    playerIndexManager indexManager;
    [SerializeField] List<GameObject> players;
    int previousPlayerCount = 0;

    [SerializeField] private controlScheme controls;
    public void Awake()
    {
        indexManager = FindObjectOfType<playerIndexManager>();
        players = indexManager.GetPlayers();

        SwitchAllControls();
    }

    private void Update()
    {
        NewPlayerCheck();
    }

    private void NewPlayerCheck()
    {
        if (previousPlayerCount < players.Count)
        {
            previousPlayerCount = players.Count;
            SwitchAllControls();
        }
    }
    private void SwitchAllControls()
    {
        foreach (GameObject player in players)
        {
            if (controls == controlScheme.fight)
            {
                player.GetComponent<monstroInputHandler>().switchToFightControls();
            }
            else if (controls == controlScheme.hub)
            {
                player.GetComponent<monstroInputHandler>().switchToHubInteractionControls();
            }
            else if (controls == controlScheme.UIbuttons)
            {
                player.GetComponent<monstroInputHandler>().switchToUIbuttonsControls();
            }
            else if (controls == controlScheme.UIcursor)
            {
                player.GetComponent<monstroInputHandler>().switchToUICursorControls();
            }
        }
    }
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(myScenes[i]);
    }

    public void LoadSelectedStage()
    {
        SceneManager.LoadScene(myScenes[selectedStage]);
    }
    public void SetSelectedStage(int i)
    {
        selectedStage = i;
    }

    public void SwitchControlsOff()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<monstroInputHandler>().cursorHandler.enabled = false;
        }
    }
}
