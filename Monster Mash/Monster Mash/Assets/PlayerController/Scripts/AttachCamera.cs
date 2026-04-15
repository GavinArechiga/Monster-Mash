using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    [SerializeField] private GameObject playerCameraRoot;
    
    void Start()
    {
        if (Camera.main != null) Camera.main.gameObject.AddComponent<CinemachineBrain>();
        GetComponent<CinemachineVirtualCamera>().Follow = playerCameraRoot.transform;
        GetComponent<CinemachineVirtualCamera>().LookAt = playerCameraRoot.transform;
        transform.SetParent(transform.parent.parent);
    }
}
