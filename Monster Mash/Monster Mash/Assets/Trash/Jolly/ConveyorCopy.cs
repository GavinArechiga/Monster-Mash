using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorCopy : MonoBehaviour
{
    public Transform mainConveyor;

    // called from PartSelector upon switching part categories.
    // parents this duplicate conveyor to the category being
    // enabled, so that it will display on top of the previous
    // active category, and move with the newly enabled
    // category during the switch-in animation.
    public void AssistSwitchIn(Transform p)
    {
        transform.position = new Vector3 (mainConveyor.position.x, mainConveyor.position.y, transform.position.z);
        transform.SetParent(p);
        transform.SetAsFirstSibling();
        gameObject.SetActive(true);
    }


    public void FinishAnimAssist()
    {
        mainConveyor.GetComponent<PartSelectorConveyorRepeat>().RestartGlowAnim();
        gameObject.SetActive(false);
    }

}
