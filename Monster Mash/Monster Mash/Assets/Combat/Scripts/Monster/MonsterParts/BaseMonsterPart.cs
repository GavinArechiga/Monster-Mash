using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonsterPart : MonoBehaviour
{
    [SerializeField] protected MonsterPartLimb partLimbType;

    //Temp Should Probably Move Part Animation Control To Its Own Script
    //Though it may be best practice to use this Combat Monster Part Script as the Initializer for the animator
    [SerializeField] protected Animator partAnim;

    public virtual void InitializeMonsterPart()
    {
        //Some Decor Pieces Might Use This for Something
    }
}
