using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ASyncLoader : MonoBehaviour
{
    [SerializeField] int scene;

    PlayerManager manager;
    SceneController sceneController;

    [SerializeField] private Slider slider;

    private void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        manager = FindObjectOfType<PlayerManager>();
        scene = manager.selectedStage;

        StartCoroutine("LoadLevelASync");
    }
    private IEnumerator LoadLevelASync()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneController.myScenes[scene]);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            slider.value = progressValue;
            yield return null;
        }
    }
}
