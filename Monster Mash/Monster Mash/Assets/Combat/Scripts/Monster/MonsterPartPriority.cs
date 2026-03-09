using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonsterPartPriority
{
    public static Dictionary<MonsterPartLimb, int> partPriorityDictionary { get; private set; } =
        new Dictionary<MonsterPartLimb, int>()
        {
            {MonsterPartLimb.Torso, 0},
            {MonsterPartLimb.Head, 1},
            {MonsterPartLimb.Arm, 2},
            {MonsterPartLimb.Leg, 2},
            {MonsterPartLimb.Tail, 3},
            {MonsterPartLimb.Wing, 3},
            {MonsterPartLimb.Eye, 4},
            {MonsterPartLimb.Mouth, 4},
            {MonsterPartLimb.Decor, 5},
        };

    public static int SortPartPriority(MonsterDataStorage part1, MonsterDataStorage part2)
    {
        return ReturnPartPriority(part1._monsterPart.ReturnLimbType())
            .CompareTo(ReturnPartPriority(part2._monsterPart.ReturnLimbType()));
    }

    static int ReturnPartPriority(MonsterPartLimb limbType)
    {
        return partPriorityDictionary[limbType];
    }
}
