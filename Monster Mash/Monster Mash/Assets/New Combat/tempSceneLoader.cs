using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tempSceneLoader : MonoBehaviour
{
    public GameObject[] flightButtons;
    public GameObject[] nonFlightButtons;

    public void loadBlockParty()
    {
        SceneManager.LoadScene(1);
    }

    public void loadNewBugsburg()
    {
        SceneManager.LoadScene(2);
    }

    public void loadMonsterberryMunchMania()
    {
        SceneManager.LoadScene(3);
    }

    public void loadBeastdome()
    {
        SceneManager.LoadScene(4);
    }

    public void givePlayerFlight(int playerNumber)
    {
        GameObject playerObject = GameObject.Find("Player " + playerNumber);

        if (playerObject != null)
        {
            monstroLocomotion chosenMonster = GameObject.Find("Player " + playerNumber).GetComponent<monstroLocomotion>();
            chosenMonster.wingedMonster = true;
            flightButtons[playerNumber - 1].SetActive(false);
            nonFlightButtons[playerNumber - 1].SetActive(true);
        }
    }

    public void givePlayerNonFlight(int playerNumber)
    {
        GameObject playerObject = GameObject.Find("Player " + playerNumber);

        if (playerObject != null)
        {
            monstroLocomotion chosenMonster = GameObject.Find("Player " + playerNumber).GetComponent<monstroLocomotion>();
            chosenMonster.wingedMonster = false;
            nonFlightButtons[playerNumber - 1].SetActive(false);
            flightButtons[playerNumber - 1].SetActive(true);
        }
    }
}
