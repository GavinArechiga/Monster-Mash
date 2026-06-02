using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerIndexManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> players = new List<GameObject>();

    public void addNewPlayer(GameObject newPlayer)
    {
        if(players.Contains(newPlayer) == false)
        {
            players.Add(newPlayer);
            newPlayer.GetComponent<monstroIndex>().playerIndex = players.Count;
            newPlayer.name = "Player " + players.Count;
        }
    }
}
