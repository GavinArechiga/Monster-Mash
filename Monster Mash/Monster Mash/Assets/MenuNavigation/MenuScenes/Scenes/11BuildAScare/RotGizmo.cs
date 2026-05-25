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

    public Vector3 RotateX(float input)
    {
        Vector3 myRot = Vector3.right * -input * rotSpeed * Time.deltaTime;
        xAxis.Rotate(myRot, Space.Self);
        return myRot;
    }

    public Vector3 RotateY(float input)
    {
        Vector3 myRot = Vector3.up * -input * rotSpeed * Time.deltaTime;
        yAxis.Rotate(myRot, Space.Self);
        return myRot;
    }

    public Vector3 RotateZ(float input)
    {
        Vector3 myRot = Vector3.forward * -input * rotSpeed * Time.deltaTime;
        zAxis.Rotate(myRot, Space.Self);
        return myRot;
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
