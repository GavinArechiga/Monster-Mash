using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BAS_CurrentPart
{
    private Collider currTorso;
    private List<Collider> allHeads = new List<Collider>();
    public void SetCurrTorso(Collider torso)
    {
        currTorso = torso;
    }

    public void SetCurrTorso(GameObject torso)
    {
        currTorso = torso.GetComponentInChildren<Collider>();
    }
    public Collider GetCurrTorso()
    {
        return currTorso;
    }

    public void AddHead(Collider head)
    {
        allHeads.Add(head);
    }

    public void AddHead(GameObject head)
    {
        allHeads.Add(head.GetComponentInChildren<Collider>());
    }

    public void RemoveHead(Collider head)
    {
        allHeads.Remove(head);
    }

    public void RemoveHead(GameObject head)
    {
        allHeads.Remove(head.GetComponentInChildren<Collider>());
    }

    public List<Collider> GetAllHeads()
    {
        return allHeads;
    }
}
