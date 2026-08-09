using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hands
{
    point,
    open,
    closed
}
public class CursorArt : MonoBehaviour
{
    [SerializeField] private Hands cursor;

    [SerializeField] private GameObject[] handAssets = new GameObject[3];

    private Hands currHand;

    public void Change(Hands pose)
    {
        currHand = pose;

        if (pose == Hands.point)
        {
            ChangeFR(0);
        }
        else if (pose == Hands.open)
        {
            ChangeFR(1);
        }
        else if (pose == Hands.closed)
        {
            ChangeFR(2);
        }
    }

    private void ChangeFR(int x)
    {
        for (int i = 0; i < handAssets.Length; i++)
        {
            handAssets[i].SetActive(x == i);
        }
    }

    public Hands GetHand()
    {
        return currHand;
    }
}
