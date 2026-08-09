using System.Collections;
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

    public enum typesOfLightAttack
    {
        none,
        physical,
        projectile,
        AOE
    }

    public enum typesOfHeavyAttack
    {
        none,
        physical,
        projectile,
        AOE
    }

    public enum typesOfHeavyStatusEffect
    {
        none, 
        burning,
        electrocution
    }

    [System.Serializable]
    public class animationList
    {
        public AnimationClip staticAnimation;
        public AnimationClip idleAnimation;
        public AnimationClip runAnimation;
        public AnimationClip walkAnimation;
        public AnimationClip fallAnimation;
        public AnimationClip jumpAnimation;
        public AnimationClip doubleJumpAnimation;
        public AnimationClip landAnimation;
        public AnimationClip windUpAnimation;
        public AnimationClip lightAttackAnimation;
        public AnimationClip altLightAttackAnimation;
        public AnimationClip heavyAttackAnimation;
        public AnimationClip braceAnimation;
        public AnimationClip altBraceAnimation;
        public AnimationClip lightHitAnimation;
        public AnimationClip heavyHitAnimation;
        public AnimationClip dissolveAnimation;
        public AnimationClip burningAnimation;
        public AnimationClip electrocutionAnimation;
    }

    [Header("Monstro Part Info")]
    public typesOfMonstroPart monstroPartDropDown = new typesOfMonstroPart();
    public typesOfLightAttack lightAttackTypeDropDown = new typesOfLightAttack();
    public int lightAttackDamage;
    public typesOfHeavyAttack heavyAttackTypeDropDown = new typesOfHeavyAttack();
    public typesOfHeavyStatusEffect heavyStatusEffectDropDown = new typesOfHeavyStatusEffect();
    public int heavyAttackDamage;
    public typesOfBodyPlacement bodyPlacementDropDown = new typesOfBodyPlacement();
    private bool isAttacking = false;
    private bool isRemoved = false;

    [Header("Animation Set Up")]
    public RuntimeAnimatorController animationControllerTemplate;
    public animationList animationSystem = new animationList();
    private Animator monstroPartAnimator;
    AnimatorOverrideController instancedAnimator;

    [Header("Meshes, Damage Visuals, and Colliders")]
    public MeshRenderer[] myMeshes;
    public SkinnedMeshRenderer[] mySkinnedMeshes;
    public SpriteRenderer[] mySpriteRenderers;
    public MeshRenderer[] myDamageMeshes;
    public SkinnedMeshRenderer[] myDamageSkinnedMeshes;
    public GameObject breakawayColliders;

    public void createNewAnimator() // this works, Im really just looking to make sure all the transitions feel good before a mass roll out
    {
        monstroPartAnimator = GetComponent<Animator>();
        instancedAnimator = new AnimatorOverrideController(animationControllerTemplate);

        //static
        if(animationSystem.staticAnimation != null)
        {
            instancedAnimator["static"] = animationSystem.staticAnimation;
        }

        if (animationSystem.idleAnimation != null)
        {
            instancedAnimator["idle"] = animationSystem.idleAnimation;
        }

        if (animationSystem.runAnimation != null)
        {
            instancedAnimator["run"] = animationSystem.runAnimation;
        }

        if (animationSystem.walkAnimation != null)
        {
            instancedAnimator["walk"] = animationSystem.walkAnimation;
        }

        if (animationSystem.fallAnimation != null)
        {
            instancedAnimator["fall"] = animationSystem.fallAnimation;
        }

        if (animationSystem.jumpAnimation != null)
        {
            instancedAnimator["jump"] = animationSystem.jumpAnimation;
        }

        if (animationSystem.doubleJumpAnimation != null)
        {
            instancedAnimator["double jump"] = animationSystem.doubleJumpAnimation;
        }

        if (animationSystem.landAnimation != null)
        {
            instancedAnimator["land"] = animationSystem.landAnimation;
        }

        if (animationSystem.windUpAnimation != null)
        {
            instancedAnimator["wind up"] = animationSystem.windUpAnimation;
        }

        if (animationSystem.lightAttackAnimation != null)
        {
            instancedAnimator["light attack"] = animationSystem.lightAttackAnimation;
        }

        if (animationSystem.altLightAttackAnimation != null)
        {
            instancedAnimator["alt light attack"] = animationSystem.altLightAttackAnimation;
        }

        if (animationSystem.heavyAttackAnimation != null)
        {
            instancedAnimator["heavy attack"] = animationSystem.heavyAttackAnimation;
        }

        if (animationSystem.braceAnimation != null)
        {
            instancedAnimator["brace"] = animationSystem.braceAnimation;
        }

        if (animationSystem.altBraceAnimation != null)
        {
            instancedAnimator["alt brace"] = animationSystem.altBraceAnimation;
        }

        if (animationSystem.lightHitAnimation != null)
        {
            instancedAnimator["light hit"] = animationSystem.lightHitAnimation;
        }

        if (animationSystem.heavyHitAnimation != null)
        {
            instancedAnimator["heavy hit"] = animationSystem.heavyHitAnimation;
        }

        if (animationSystem.dissolveAnimation != null)
        {
            instancedAnimator["dissolve"] = animationSystem.dissolveAnimation;
        }

        if (animationSystem.burningAnimation != null)
        {
            instancedAnimator["burning"] = animationSystem.burningAnimation;
        }

        if (animationSystem.electrocutionAnimation != null)
        {
            instancedAnimator["electrocution"] = animationSystem.electrocutionAnimation;
        }

        monstroPartAnimator.runtimeAnimatorController = instancedAnimator;

        if (this.transform.localScale.x < 0 && monstroPartDropDown.ToString() == "leg")
        {
            monstroPartAnimator.SetFloat("legOffset", 0.5f);
        }
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
        if (isAttacking) return;
        if (animationSystem.idleAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetBool("isRunning", false);
        monstroPartAnimator.SetBool("isWalking", false);
        monstroPartAnimator.SetTrigger("idle");
    }

    public void playWalk()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.walkAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetBool("isWalking", true);
        monstroPartAnimator.SetBool("isRunning", false);
        monstroPartAnimator.SetTrigger("walk");
    }

    public void playRun()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.runAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetBool("isRunning", true);
        monstroPartAnimator.SetBool("isWalking", false);
        monstroPartAnimator.SetTrigger("run");
    }

    public void playJump()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.jumpAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("jump");
        monstroPartAnimator.SetBool("isGrounded", false);
    }

    public void playDoubleJump()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.doubleJumpAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("double jump");
    }

    public void playFall()
    {
        if (monstroPartAnimator == null) return;
        if (isAttacking) return;
        if (isRemoved) return;
        if (animationSystem.fallAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("fall");
        monstroPartAnimator.SetBool("isGrounded", false);
    }

    public void playLand()
    {
        if (monstroPartAnimator == null) return;
        if (isAttacking) return;
        if (isRemoved) return;
        if (animationSystem.landAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("land");
        monstroPartAnimator.SetBool("isGrounded", true);
    }

    public void playWindUp()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.windUpAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("wind up");
    }

    public void playLightAttack()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.lightAttackAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("light attack");
    }

    public void playHeavyAttack()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.heavyAttackAnimation == null) return;
        cleanAnimations();

        monstroPartAnimator.SetTrigger("heavy attack");
    }

    public void playRightBrace()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (isAttacking) return;
        if (animationSystem.braceAnimation == null) return;
        cleanAnimations();

        if (this.transform.localScale.x < 0 && animationSystem.altBraceAnimation != null)
        {
            monstroPartAnimator.SetTrigger("alt brace");
        }
        else
        {
            monstroPartAnimator.SetTrigger("brace");
        }
    }

    public void playLeftBrace()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (isAttacking) return;
        if (animationSystem.braceAnimation == null) return;
        cleanAnimations();

        if (this.transform.localScale.x > 0 && animationSystem.altBraceAnimation != null)
        {
            monstroPartAnimator.SetTrigger("alt brace");
        }
        else
        {
            monstroPartAnimator.SetTrigger("brace");
        }
    }

    public void playLightHit()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.lightHitAnimation == null) return;

        cleanAnimations();
        unlockAttackAnimation();

        monstroPartAnimator.SetTrigger("light hit");
    }

    public void playHeavyHit()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.heavyHitAnimation == null) return;

        cleanAnimations();
        unlockAttackAnimation();

        monstroPartAnimator.SetTrigger("heavy hit");
    }

    public void playBurningReaction()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.burningAnimation == null) return;

        cleanAnimations();

        monstroPartAnimator.SetBool("onFire", true);
        monstroPartAnimator.SetTrigger("burning");
    }

    public void playElectrocutionReaction()
    {
        if (monstroPartAnimator == null) return;
        if (isRemoved) return;
        if (animationSystem.electrocutionAnimation == null) return;

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
