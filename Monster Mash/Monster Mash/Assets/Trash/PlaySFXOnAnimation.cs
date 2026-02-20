using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXOnAnimation : MonoBehaviour
{
    [SerializeField] NewMonsterPart part;
    [SerializeField] SFXManager sfxManager;


    public void playFootstepSFXOnAnimation()
    {
        if (part.PartType is MonsterPartType.Leg)
        {
            sfxManager.footstepSFX(part);
        }
    }
}
