using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroPart : MonoBehaviour
{   
    public enum typesOfMonstroPart
    {
        none,
        head,
        torso,
        arm,
        leg, 
        tail,
        wing,
        eye,
        mouth,
        horn,
        decor
    }

    public typesOfMonstroPart monstroPartDropDown = new typesOfMonstroPart();

    public enum typesOfLightAttack
    {
        none,
        physical,
        projectile,
        AOE
    }

    public typesOfLightAttack lightAttackTypeDropDown = new typesOfLightAttack();

    public enum typesOfHeavyAttack
    {
        none,
        physical,
        projectile,
        AOE
    }

    public typesOfHeavyAttack heavyAttackTypeDropDown = new typesOfHeavyAttack();

    public enum typesOfBodyPlacement
    {
        none,
        leftShoulder,
        rightShoulder,
        leftMidSection,
        rightMidSection,
        leftPelvis,
        rightPelvis,
        upperBack,
        lowerBack,
        chest,
        belly,
        neck,
        leftHead,
        rightHead,
        face,
        backHead,
        topHead
    }

    public typesOfBodyPlacement bodyPlacementDropDown = new typesOfBodyPlacement();


    public Animator monstroPartAnimator;
    AnimatorOverrideController instancedAnimator;
    public AnimationClip idleAnimation;
    public AnimationClip runAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip fallAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip doubleJumpAnimation;
    public AnimationClip landAnimation;
    public AnimationClip windUpAnimation;
    public AnimationClip lightAttackAnimation;
    public AnimationClip heavyAttackAnimation;
    public MeshRenderer[] myMeshes;
    public SkinnedMeshRenderer[] mySkinnedMeshes;
    public SpriteRenderer[] mySpriteRenderers;

    public void createNewAnimator() // this works, Im really just looking to make sure all the transitions feel good before a mass roll out
    {
        monstroPartAnimator = GetComponent<Animator>();
        instancedAnimator = new AnimatorOverrideController(monstroPartAnimator.runtimeAnimatorController);

        instancedAnimator["idle"] = idleAnimation;
        instancedAnimator["run"] = runAnimation;
        instancedAnimator["walk"] = walkAnimation;
        instancedAnimator["fall"] = fallAnimation;
        instancedAnimator["jump"] = jumpAnimation;
        instancedAnimator["double jump"] = doubleJumpAnimation;
        instancedAnimator["land"] = landAnimation;
        instancedAnimator["wind up"] = windUpAnimation;
        instancedAnimator["light attack"] = lightAttackAnimation;
        instancedAnimator["heavy attack"] = heavyAttackAnimation;

        monstroPartAnimator.runtimeAnimatorController = instancedAnimator;
    }

    public void cleanAnimations()
    {
        if (monstroPartAnimator == null) return;

        monstroPartAnimator.ForceResetTriggers();
    }

    public void stopAnimations()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("static");
    }

    public void hideMonster()
    {
        if (myMeshes.Length != 0)
        {
            for (int i = 0; i < myMeshes.Length; i++)
            {
                myMeshes[i].enabled = false;
            }
        }

        if (mySkinnedMeshes.Length != 0)
        {
            for (int i = 0; i < mySkinnedMeshes.Length; i++)
            {
                mySkinnedMeshes[i].enabled = false;
            }
        }

        if (mySpriteRenderers.Length != 0)
        {
            for (int i = 0; i < mySpriteRenderers.Length; i++)
            {
                mySpriteRenderers[i].enabled = false;
            }
        }
    }

    public void showMonster()
    {
        if (myMeshes.Length != 0)
        {
            for (int i = 0; i < myMeshes.Length; i++)
            {
                myMeshes[i].enabled = true;
            }
        }

        if(mySkinnedMeshes.Length != 0)
        {
            for (int i = 0; i < mySkinnedMeshes.Length; i++)
            {
                mySkinnedMeshes[i].enabled = true;
            }
        }

        if(mySpriteRenderers.Length != 0)
        {
            for (int i = 0; i < mySpriteRenderers.Length; i++)
            {
                mySpriteRenderers[i].enabled = true;
            }
        }
    }

    public void playIdle()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("idle");
    }

    public void playWalk()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("walk");
    }

    public void playRun()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("run");
    }

    public void playJump()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("jump");
    }

    public void playDoubleJump()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("double jump");
    }

    public void playFall()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("fall");
    }

    public void playLand()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("land");
    }

    public void playWindUp()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("wind up");
    }

    public void playLightAttack()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("light attack");
    }

    public void playHeavyAttack()
    {
        cleanAnimations();

        if (monstroPartAnimator == null) return;

        monstroPartAnimator.SetTrigger("heavy attack");
    }
}
