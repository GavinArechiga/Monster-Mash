using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontDestroy : MonoBehaviour
{
    private string objectID;

    private void Awake()
    {
        objectID = name;
    }

    private void Start()
    {
        for (int i = 0; i < Object.FindObjectsOfType<dontDestroy>().Length; i++)
        {
            if (Object.FindObjectsOfType<dontDestroy>()[i] != this)
            {
                if (Object.FindObjectsOfType<dontDestroy>()[i].objectID == objectID)
                {
                    Destroy(gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
