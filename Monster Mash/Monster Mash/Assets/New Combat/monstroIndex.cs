using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroIndex : MonoBehaviour
{
    public int playerIndex;
    private playerIndexManager playerIndexManager;

    private void Awake()
    {
        if (playerIndex == 0)
        {
            playerIndexManager = Object.FindFirstObjectByType<playerIndexManager>();
            playerIndexManager.addNewPlayer(this.gameObject);
        }


    }
}
