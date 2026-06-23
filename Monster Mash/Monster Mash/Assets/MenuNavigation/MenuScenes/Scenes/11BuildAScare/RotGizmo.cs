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

    private Vector3 ogScale;
    private Transform target;

    [SerializeField] Material xRed;
    [SerializeField] Material yYellow;
    [SerializeField] Material zBlue;

    void Start()
    {
        ogScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        MakeGizmoStickWithParent();
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
        target = myObj.transform;
    }

    public void Detach()
    {
        target = null;
        gameObject.SetActive(false);
    }

    private void MakeGizmoStickWithParent() //i cant actually parent or it gets really sad with scale stuff :((
    {
        transform.position = target.position;

        Vector3 rot = transform.eulerAngles;
        rot.y = target.eulerAngles.y;
        transform.eulerAngles = rot;
    }

    public void MakeTransparent(string axis, bool trans) //true is transparent, false if opaque
    {
        int x = trans ? 1: 0;

        if (axis == "all")
        {
            MakeTransparent("x", trans);
            MakeTransparent("y", trans);
            MakeTransparent("z", trans);
        }
        else if (axis == "x" && xRed.GetFloat("_Surface") != x)
        {
            xRed.SetFloat("_Surface", x);
        }
        else if (axis == "y" && yYellow.GetFloat("_Surface") != x)
        {
            yYellow.SetFloat("_Surface", x);
        }
        else if (axis == "z" && zBlue.GetFloat("_Surface") != x)
        {
            zBlue.SetFloat("_Surface", x);
        }
    }
}
