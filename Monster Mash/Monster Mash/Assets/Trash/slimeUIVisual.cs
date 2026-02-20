using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeUIVisual : MonoBehaviour
{

    public float scrollSpeedX;
    public float scrollSpeedY;
    //public MeshRenderer myMesh;
    public Material slime;

    // Update is called once per frame
    void Update()
    {
        //slime.SetVector("_DetailAlbedoMap", new Vector2(Time.realtimeSinceStartup * scrollSpeedX, Time.realtimeSinceStartup * scrollSpeedY));
        slime.SetTextureOffset("_DetailAlbedoMap", new Vector2(Time.realtimeSinceStartup * scrollSpeedX, Time.realtimeSinceStartup * scrollSpeedY));
    }
}
