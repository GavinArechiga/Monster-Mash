using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResultsManager : MonoBehaviour
{
    [SerializeField] private Transform[] pos;

    // Start is called before the first frame update
    void Start()
    {
        SetScene();
    }

    private void SetScene()
    {
        int i = 0;

        foreach (PlayerInput x in PlayerInput.all)
        {
            Player player = x.GetComponent<Player>();

            if (player.GetCharacter())
            {
                GameObject character = player.GetCharacter();
                character.SetActive(true);
                character.GetComponent<Rigidbody>().useGravity = false;
                character.GetComponent<Rigidbody>().velocity *= 0f;
                character.transform.position = pos[i].position;
                character.transform.rotation = pos[i].rotation;
            }

            i++;
        }
    }

    public void Reset()
    {
        foreach (PlayerInput x in PlayerInput.all)
        {
            Player player = x.GetComponent<Player>();

            if (player.GetCharacter())
            {
                GameObject character = player.GetCharacter();
                character.SetActive(false);
            }
        }
    }
}
