using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class hubCameraFocuser : MonoBehaviour
{
    public CinemachineVirtualCamera myCameraToFocus;
    private void OnTriggerEnter(Collider other)
    {
        myCameraToFocus.Priority = 100;
    }

    private void OnTriggerExit(Collider other)
    {
        myCameraToFocus.Priority = 0;
    }
}
