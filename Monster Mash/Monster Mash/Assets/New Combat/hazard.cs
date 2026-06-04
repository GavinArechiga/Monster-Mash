using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hazard : MonoBehaviour
{
    //this script is defunct but i keep it around to see how to do drop down menus
    public enum typesOfHazard
    {
        fire,
        car,
        nail
    };

    public typesOfHazard hazardDropDown = new typesOfHazard();
    private int hazardIndex;
    private void Awake()
    {
        hazardIndex = (int)hazardDropDown;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Rigidbody smackedMonster = other.GetComponent<Rigidbody>();

        if (hazardIndex == 1)//car behavior
        {
            Vector3 pushDirection = (hitPoint - other.transform.position).normalized;
            smackedMonster.AddForceAtPosition(pushDirection * 4, hitPoint, ForceMode.Impulse);
            print("Hit by car!");
        }
    }
}
