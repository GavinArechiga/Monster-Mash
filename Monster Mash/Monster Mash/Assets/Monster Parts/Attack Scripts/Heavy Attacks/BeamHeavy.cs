using System.Collections;
using UnityEngine;

public class BeamHeavy : HeavyAttack
{
    public BeamHeavy()
    {
        DamageRange = DamageRange.Range0;
    }

    public override void triggerHeavyAttackVisuals()
    {
        monsterPartVisualRef.heavyHitVFXManager.unleashBeamVisual();
    }
    public override void endRemainingVFX()
    {
        monsterPartVisualRef.heavyHitVFXManager.endBeamVisual();
    }

    public override void TriggerAttack()
    {
        monsterPartRef.heavyAttackInMotion = true;
    }
    public override void CancelAttack()
    {
        //end functions + turn off visuals
        monsterPartVisualRef.triggerAttackToIdle();
        monsterPartRef.StartCoroutine(beamAttackDelay());
    }

    IEnumerator beamAttackDelay()
    {
        yield return new WaitForSeconds(0.2f);
        monsterPartRef.triggerAttackCorrections(); //delaying this to allow the body time to unbrace
    }
}
