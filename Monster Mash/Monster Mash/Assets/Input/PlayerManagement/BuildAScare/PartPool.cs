using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPool : MonoBehaviour
{
    [SerializeField] string[] torsoList;
    [SerializeField] string[] armList;
    [SerializeField] string[] legList;
    [SerializeField] string[] headList;
    [SerializeField] string[] eyeList;
    [SerializeField] string[] mouthList;
    [SerializeField] string[] tailList;
    [SerializeField] string[] wingList;
    [SerializeField] string[] hornList;
    [SerializeField] string[] decorList;

    private string[][] monsterParts;

    private void Start()
    {
        monsterParts = new string[][] { torsoList, armList, legList, headList, eyeList, mouthList, tailList, wingList, hornList, decorList };
    }

    public string[][] GetMonsterParts()
    {
        return monsterParts;
    }
}
