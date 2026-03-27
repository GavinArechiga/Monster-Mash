using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    [SerializeField] private GameObject playerCameraRoot;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CinemachineVirtualCamera>().Follow = playerCameraRoot.transform;
        GetComponent<CinemachineVirtualCamera>().LookAt = playerCameraRoot.transform;
        transform.SetParent(transform.parent.parent);
    }
}
