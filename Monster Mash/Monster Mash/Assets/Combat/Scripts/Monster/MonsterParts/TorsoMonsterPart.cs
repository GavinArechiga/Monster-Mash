using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoMonsterPart : ConnectingMonsterPart
{
    [SerializeField]
    float connectingRadius;

    [Header("Torso Animation Blend")]

    [SerializeField]
    Vector2[] connectionBlendDir;

    Dictionary<string, Vector2> connectionPointsBlend;

    public void InitializeBlendDictionary()
    {
        if(connectionBlendDir.Length > 0)
        {
            connectionPointsBlend = new Dictionary<string, Vector2>();

            for(int i = 0; i < connectionPoints.Length; i++)
            {
                string name = connectionPoints[i].gameObject.name;

                connectionPointsBlend.Add(name, connectionBlendDir[i]);
            }
        }
    }

    public Vector2 ReturnBlendDir(Transform parent)
    {
        if(parent == null)
        {
            return new Vector2(0, 0);
        }

        string name = parent.gameObject.name;

        if(connectionPointsBlend.ContainsKey(name))
        {
            return connectionPointsBlend[name];
        }

        else
        {
            return new Vector2(0, 0);
        }
    }
    public override Collider[] ReturnCollidedBodies(Transform point)
    {
        Collider[] returnCollider;

        returnCollider = Physics.OverlapSphere(point.position, connectingRadius, connectingPartMask);

        return returnCollider;
    }
}
