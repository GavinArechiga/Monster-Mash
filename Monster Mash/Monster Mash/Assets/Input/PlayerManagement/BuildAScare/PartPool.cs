using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPool : MonoBehaviour
{
    [SerializeField] string[] partList;

    public string[] GetList()
    {
        return partList;
    }
    public string GetIndexString(int i)
    {
        return partList[i];
    }
}
