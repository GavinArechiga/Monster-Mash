using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingMonsterPart : BaseMonsterPart
{
    [Header("Connecting Part Attributes")]
    [SerializeField]
    protected Transform[] connectionPoints;

    List<Transform> connectedParts;

    [SerializeField]
    protected LayerMask connectingPartMask;
    public override void InitializeMonsterPart(AttackButtons partButton, CombatMonster combatMonster)
    {
        SetBaseMonsterPart(partButton, combatMonster);

        ConnectAllMonsterParts();

        if(ReturnLimbType() is MonsterPartLimb.Torso)
        {
            var torso = this as TorsoMonsterPart;

            torso.InitializeBlendDictionary();
        }
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

    public virtual Collider[] ReturnCollidedBodies(Transform point)
    {
        Collider[] returnCollider;

        switch(partLimbType)
        {
            default:
            case MonsterPartLimb.Torso:

                returnCollider =  Physics.OverlapSphere(point.position, 0.05f, connectingPartMask);

                break;

            case MonsterPartLimb.Head:

                returnCollider = Physics.OverlapBox(point.position, new Vector3(0.5f,0.5f,0.5f), point.rotation, connectingPartMask);

                break;
        }

        return returnCollider;
    }
}
