using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    SceneController sceneController;
    public float cutSceneDuration;

    [SerializeField] private int nextScene;
    private void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        StartCoroutine("WaitAndLoadScene");
    }
    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(cutSceneDuration);

        LoadScene();

        yield break;
    }

    public void LoadScene()
    {
        StopCoroutine("WaitAndLoadScene");
        sceneController.LoadScene(nextScene);
    }
}
