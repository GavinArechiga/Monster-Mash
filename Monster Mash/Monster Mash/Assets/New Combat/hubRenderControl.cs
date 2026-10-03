using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hubRenderControl : MonoBehaviour
{

    public GameObject[] renderedHubAreas;
    public GameObject[] derenderedHubAreas;

    private void OnTriggerEnter(Collider other)
    {
        deRenderHubAreas();
        renderHubAreas();
    }

    public void deRenderHubAreas()
    {
        if (derenderedHubAreas.Length == 0) return;

        for (int i = 0; i < derenderedHubAreas.Length; i++)
        {
            derenderedHubAreas[i].SetActive(false);
        }
    }

    public void renderHubAreas()
    {
        if (renderedHubAreas.Length == 0) return;

        for (int i = 0; i < renderedHubAreas.Length; i++)
        {
            renderedHubAreas[i].SetActive(true);
        }
    }
}
