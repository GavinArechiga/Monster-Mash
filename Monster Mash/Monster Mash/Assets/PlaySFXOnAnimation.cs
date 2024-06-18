using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXOnAnimation : MonoBehaviour
{
    [SerializeField] monsterPart part;
    [SerializeField] SFXManager sfxManager;


    public void playFootstepSFXOnAnimation()
    {
        if (part.isLeg)
        {
            sfxManager.footstepSFX(part);
        }
    }
}
