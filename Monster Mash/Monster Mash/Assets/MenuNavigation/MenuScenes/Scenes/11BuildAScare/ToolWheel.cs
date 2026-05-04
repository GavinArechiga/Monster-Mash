using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolWheel : MonoBehaviour
{
    [SerializeField] private GameObject toolWheel;

    private float[] toolWheelRots = new float[] { 0.0f, 97.0f, 226.0f };
    private string[] tools = new string[] { "Move", "Scale", "Rotate" };
    private int currentTool = 0;

    private bool wait = false;


    public void SetToolWheel(int x)
    {
        if (x >= 0 && x < 3)
        {
            currentTool = x;
            toolWheel.transform.eulerAngles = new Vector3(0, 0, toolWheelRots[x]);
        }
    }
    public void ToolWheelRotRight()
    {
        if (currentTool + 1 < tools.Length)
        {
            currentTool++;
        }
        else
        {
            currentTool = 0;
        }

        StopCoroutine("RotateTo");
        StartCoroutine("RotateTo");
    }

    public void ToolWheelRotLeft()
    {
        if (currentTool - 1 >= 0)
        {
            currentTool--;
        }
        else
        {
            currentTool = tools.Length - 1;
        }

        StopCoroutine("RotateTo");
        StartCoroutine("RotateTo");
    }

    public int GetCurrentToolInt()
    {
        return currentTool;
    }

    public string GetCurrentToolString()
    {
        return tools[currentTool];
    }

    public bool GetWait()
    {
        return wait;
    }

    private IEnumerator RotateTo()
    {
        wait = true;
        float duration = 0.4f;

        Quaternion startRot = toolWheel.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(new Vector3(0, 0, toolWheelRots[currentTool]));

        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        toolWheel.transform.rotation = targetRot;

        wait = false;
        yield break;
    }
}
