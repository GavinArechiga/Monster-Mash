using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hazard : MonoBehaviour
{
    public enum typesOfHazard
    {
        fire,
        car,
        shark,
        teeth
    };

    public typesOfHazard hazardDropDown = new typesOfHazard();
    public string selectedHazard;
    //private int hazardIndex;
    private void Awake()
    {
        //hazardIndex = (int)hazardDropDown;
        selectedHazard = hazardDropDown.ToString();
    }
}
