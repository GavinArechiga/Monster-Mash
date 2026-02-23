using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LoadRing : MonoBehaviour
{
    Image ring;
    [SerializeField] private float loadTime = 3f;
    private float timer = 0f;

    private bool stop = true;

    private void Awake()
    {
        ring = GetComponent<Image>();

        loadTime = InputSystem.settings.defaultHoldTime - 0.01f;
        ring.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        ring.fillAmount = timer / loadTime;

        if (!stop && timer < loadTime)
        {
            timer += Time.deltaTime;
        }
    }

    public void StopTimer()
    {
        stop = true;
        timer = 0f;
    }

    public void StartTimer()
    {
        stop = false;
    }
}
