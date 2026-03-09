using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMonsterPart : ConnectingMonsterPart
{
    [SerializeField]
    Vector3 overlapBoxDimensions;
    public override Collider[] ReturnCollidedBodies(Transform point)
    {
        Collider[] returnCollider;

        returnCollider = Physics.OverlapBox(point.position, overlapBoxDimensions, point.rotation, connectingPartMask);

        return returnCollider;
    }
}
