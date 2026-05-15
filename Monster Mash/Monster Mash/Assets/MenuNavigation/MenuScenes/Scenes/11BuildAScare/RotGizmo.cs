using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotGizmo : MonoBehaviour
{
    [SerializeField] private Transform xAxis;
    [SerializeField] private Transform yAxis;
    [SerializeField] private Transform zAxis;

    [SerializeField] private float rotSpeed = 1.5f;
    private float scaleMultiplier = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            RotateX(1);
        }

        if (Input.GetKey(KeyCode.J))
        {
            RotateY(1);
        }

        if (Input.GetKey(KeyCode.H))
        {
            RotateZ(1);
        }
    }

    public void RotateX(int dir)
    {
        //xAxis.localEulerAngles += Vector3.right * dir * rotSpeed * Time.deltaTime;
        xAxis.Rotate(Vector3.right * dir * rotSpeed * Time.deltaTime, Space.Self);
    }

    public void RotateY(int dir)
    {
        //yAxis.localEulerAngles += Vector3.up * dir * rotSpeed * Time.deltaTime;
        yAxis.Rotate(Vector3.up * dir * rotSpeed * Time.deltaTime, Space.Self);
    }

    public void RotateZ(int dir)
    {
        //zAxis.localEulerAngles += Vector3.forward * dir * rotSpeed * Time.deltaTime;
        zAxis.Rotate(Vector3.forward * dir * rotSpeed * Time.deltaTime, Space.Self);
    }

    public void MoveGizmo(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void ScaleGizmo(Vector3 newScale)
    {
        transform.localScale = newScale * scaleMultiplier;
    }

    public void RotateGizmo(Vector3 newRot)
    {
        transform.eulerAngles = newRot;
    }

    public void SetUpGizmo(GameObject myObj)
    {
        transform.position = myObj.transform.position;
        transform.eulerAngles = myObj.transform.eulerAngles;
        ScaleGizmo(myObj.transform.lossyScale);
        transform.parent = myObj.transform;
    }

    public void Detach()
    {
        transform.parent = null;
        gameObject.SetActive(false);
    }
}
