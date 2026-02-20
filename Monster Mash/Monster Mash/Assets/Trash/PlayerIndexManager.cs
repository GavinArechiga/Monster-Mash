using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIndexManager : MonoBehaviour
{
    private Controller2D[] players;

    private void OnEnable()
    {
        StartCoroutine("MonsterLoadDelay");
    }

    private IEnumerator MonsterLoadDelay()
    {
        yield return new WaitForSeconds(3f);

        SetupControllers();

        yield break;
    }

    private void SetupControllers()
    {
        players = FindObjectsOfType<Controller2D>();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetPlayerIndex(i);
            print("player: " + players[i]);
        }

        if (GetComponent<PlayerInputManager>()) GetComponent<PlayerInputManager>().enabled = true;
    }
}
