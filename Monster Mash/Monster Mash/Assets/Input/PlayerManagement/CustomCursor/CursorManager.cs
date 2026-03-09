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
    [SerializeField] private PlayerCharacterSelectController[] controllers = new PlayerCharacterSelectController[4];

    public RectTransform canvas;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PlayerManager>();

        startPos = new Vector2[cursors.Length];

        for (int i = 0; i < cursors.Length; i++)
        {
            startPos[i] = cursors[i].position;
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
        print("removed at INDEX: " + index);

        cursors[index].position = startPos[index];
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
                index = i;
            }
        }

        return index;
    }
}
