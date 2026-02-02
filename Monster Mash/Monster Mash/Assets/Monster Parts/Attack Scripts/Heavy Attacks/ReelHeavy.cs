public class ReelHeavy : HeavyAttack
{
    public ReelHeavy()
    {
        DamageRange = DamageRange.Range2;
    }

    public override void triggerHeavyAttackVisuals()
    {
        if (!monsterPartRef.reelAttackLanded)
        {
            //miss visual
            triggerReelCollisionsOff();
        }

        monsterPartRef.reelAttackBuiltUpPower = 0;
        monsterPartRef.reelAttackCurrentThreshold = 0;
        monsterPartRef.powerUpCheckAllowed = false;
    }

    // Reel attacks need to be rewritten. Damage system no longer uses built up power
    public override void triggerHeavyAttackPowerCheck()
    {
        if (monsterPartRef.powerUpCheckAllowed)
        {
            monsterPartRef.reelAttackCurrentThreshold++;

            if (monsterPartRef.reelAttackCurrentThreshold == monsterPartRef.reelAttackBuiltUpPower)
            {
                monsterPartRef.reelAttackBuiltUpPower = 0;
                monsterPartRef.reelAttackCurrentThreshold = 0;
                monsterPartRef.powerUpCheckAllowed = false;
                monsterPartRef.myAnimator.ResetTrigger("Reel Back");
                monsterPartRef.myAnimator.SetTrigger("Reel Back");
            }
        }
    }

    public void triggerReelCollisionsOff() //called in attack animation
    {
        //turn off neutral vfx holder
        monsterPartRef.reelAttackLanded = false;
        monsterPartRef.reelAttackBuiltUpPower = 0;
        monsterPartRef.reelAttackCurrentThreshold = 0;
        monsterPartRef.powerUpCheckAllowed = false;

        if (monsterPartRef.attackMarkedHeavy == true)
        {
            monsterPartRef.heavyCollider.enabled = false;
        }
    }

    public void CancelGrab()
    {
        monsterPartRef.myMainSystem.grabbingCanceled();
    }
}
