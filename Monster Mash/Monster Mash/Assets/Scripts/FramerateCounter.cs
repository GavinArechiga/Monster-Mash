using UnityEngine;
using TMPro;

public class FramerateCounter : MonoBehaviour
{
    public TextMeshProUGUI Frames;
    public float pollingTime = 1f;
    private float time;
    private int frameCount;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int framerate = Mathf.RoundToInt(frameCount / time);
            Frames.text = framerate.ToString() + " FPS";
            time -= pollingTime;
            frameCount = 0;
        }
    }
}
