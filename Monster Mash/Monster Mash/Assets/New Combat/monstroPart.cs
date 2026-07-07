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
    public MeshRenderer[] myDamageMeshes;
    public SkinnedMeshRenderer[] myDamageSkinnedMeshes;
    public GameObject breakawayColliders;

    private bool isAttacking = false;
    private bool isRemoved = false;

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
        if (isRemoved) return;

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
        if (isRemoved) return;

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

    public void showDamageVisual()
    {
        if (isRemoved) return;

        if (myDamageMeshes.Length != 0)
        {
            for (int i = 0; i < myDamageMeshes.Length; i++)
            {
                myDamageMeshes[i].enabled = true;
            }
        }

        if (myDamageSkinnedMeshes.Length != 0)
        {
            for (int i = 0; i < myDamageSkinnedMeshes.Length; i++)
            {
                myDamageSkinnedMeshes[i].enabled = true;
            }
        }

    }

    public void hideDamageVisual()
    {
        if (isRemoved) return;

        if (myDamageMeshes.Length != 0)
        {
            for (int i = 0; i < myDamageMeshes.Length; i++)
            {
                myDamageMeshes[i].enabled = false;
            }
        }

        if (myDamageSkinnedMeshes.Length != 0)
        {
            for (int i = 0; i < myDamageSkinnedMeshes.Length; i++)
            {
                myDamageSkinnedMeshes[i].enabled = false;
            }
        }
    }

    public void removeThisPart()
    {
        if (breakawayColliders == null) return;
        if (isRemoved) return;

        stopAnimations();
        showMonster();
        hideDamageVisual();
        isRemoved = true;
        this.transform.parent = null;
        breakawayColliders.SetActive(true);
        this.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(dissolveDelay());
        //note that in full implimentation, the part's true scale has to be reapplied in case it ends up squished from damage animations
    }

    IEnumerator dissolveDelay()
    {
        yield return new WaitForSeconds(3);
        this.GetComponent<Rigidbody>().isKinematic = true;
        breakawayColliders.SetActive(false);
        monstroPartAnimator.SetTrigger("dissolve");
    }

    //these functions stop attacking limbs from stopping mid attack while landing or falling
    //these are mainly called from above in the input handler but we also call these here when damage animations are required
    public void lockAttackAnimation()
    {
        isAttacking = true;
    }

    public void unlockAttackAnimation()
    {
        isAttacking = false;
    }

    public void playIdle()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetBool("isRunning", false);
        monstroPartAnimator.SetBool("isWalking", false);
        monstroPartAnimator.SetTrigger("idle");
    }

    public void playWalk()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetBool("isWalking", true);
        monstroPartAnimator.SetBool("isRunning", false);
        monstroPartAnimator.SetTrigger("walk");
    }

    public void playRun()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetBool("isRunning", true);
        monstroPartAnimator.SetBool("isWalking", false);
        monstroPartAnimator.SetTrigger("run");
    }

    public void playJump()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("jump");
    }

    public void playDoubleJump()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("double jump");
    }

    public void playFall()
    {
        if (monstroPartAnimator == null) return;
        if (isAttacking) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("fall");
    }

    public void playLand()
    {
        if (monstroPartAnimator == null) return;
        if (isAttacking) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("land");
    }

    public void playWindUp()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("wind up");
    }

    public void playLightAttack()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("light attack");
    }

    public void playHeavyAttack()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("heavy attack");
    }

    public void playBrace()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("brace");
    }

    public void playLightHit()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;

        cleanAnimations();
        unlockAttackAnimation();

        monstroPartAnimator.SetTrigger("light hit");
    }

    public void playHeavyHit()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;

        cleanAnimations();
        unlockAttackAnimation();

        monstroPartAnimator.SetTrigger("heavy hit");
    }

    public void playBurningReaction()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;

        cleanAnimations();

        monstroPartAnimator.SetBool("onFire", true);
        monstroPartAnimator.SetTrigger("burning");
    }

    public void playElectrocutionReaction()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;

        cleanAnimations();

        monstroPartAnimator.SetTrigger("electrocution");
    }

    public void endStatusEffectReaction()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;

        cleanAnimations();

        monstroPartAnimator.SetBool("onFire", false);
        monstroPartAnimator.SetTrigger("remove status");
    }
}
