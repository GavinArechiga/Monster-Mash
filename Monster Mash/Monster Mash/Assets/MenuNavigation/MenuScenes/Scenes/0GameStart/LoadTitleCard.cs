using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTitleCard : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
