using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingMonsterPart : BaseMonsterPart
{
    [SerializeField]
    Transform[] connectionPoints;

    List<Transform> connectedParts;

    [SerializeField]
    LayerMask connectingPartMask;
    [SerializeField]
    Vector3 overlapBoxDimensions;
    public override void InitializeMonsterPart()
    {
        ConnectAllMonsterParts();
    }

    void ConnectAllMonsterParts()
    {
        connectedParts = new List<Transform>();

        foreach (Transform point in connectionPoints)
        {
            Collider[] collidedBodies = ReturnCollidedBodies(point);

            if (collidedBodies.Length == 0)
            {
                continue;
            }

            foreach (Collider col in collidedBodies)
            {
                if (connectedParts.Contains(col.transform))
                {
                    continue;
                }

                connectedParts.Add(col.transform);

                col.transform.parent = point;
            }
        }
    }

    Collider[] ReturnCollidedBodies(Transform point)
    {
        Collider[] returnCollider;

        switch(partLimbType)
        {
            default:
            case MonsterPartLimb.Torso:

                returnCollider =  Physics.OverlapSphere(point.position, 0.05f, connectingPartMask);

                break;

            case MonsterPartLimb.Head:

                returnCollider = Physics.OverlapBox(point.position, overlapBoxDimensions, point.rotation, connectingPartMask);

                break;
        }

        return returnCollider;
    }
}
