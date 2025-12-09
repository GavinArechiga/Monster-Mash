using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterPartReference : MonoBehaviour
{
    public monsterAttackSystem mainSystem;
    public NewMonsterPart partReference;
    public List<monsterPartReference> referencesToIgnore = new List<monsterPartReference>();
    private List<monsterAttackSystem> attackHistory = new List<monsterAttackSystem>();
    public bool isHitbox;
    public bool isHurtbox;
    public bool isStomp;
    public bool isProjectile;
    public bool isJabOrSlash;
    public bool isReel;
    public bool isGrapple;
    public bool isBoomerang;
    public bool isChargingJab;

    [Header("Damage, Direction, and Status Effects")]
    public int damage = 0;
    public bool hasTickDamage;
    public int tickDamage = 0;
    public float tickTiming = 0;
    public float tickDamageDuration = 0;
    public bool markedHeavy;
    public bool isFullyChargedHeavy;
    public int directionOfAttack = 1;
    private Vector3 pointOfContact;
    public bool hasStatusEffect;
    public bool burnedStatusEffect;
    public bool electrifiedStatusEffect;
    public bool poisonedStatusEffect;
    public bool stinkyStatusEffect;
    public bool cursedStatusEffect;
    public bool confusedStatusEffect;
    public bool slimedStatusEffect;
    public bool frozenStatusEffect;
    public bool squashedStatusEffect;
    public bool slowedStatusEffect;
    public bool grabbedStatusEffect;

    public void resetAttackHistory()
    {
        attackHistory.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isHitbox)
        {
            if (other.GetComponent<monsterPartReference>() != null)
            {
                monsterAttackSystem damagedMonster = other.GetComponent<monsterPartReference>().mainSystem;

                if (damagedMonster != null)
                {
                    if (attackHistory.Contains(damagedMonster) == false && damagedMonster != mainSystem && damagedMonster.myPlayer != null)
                    {
                        //attack
                        pointOfContact = other.ClosestPointOnBounds(transform.position);
                        playerController player = GetComponentInParent<playerController>();
                        damagedMonster.myPlayer.damaged(damage, markedHeavy, player.transform.position, pointOfContact);

                        if (hasStatusEffect && markedHeavy)
                        {
                            damagedMonster.myPlayer.statusGiven(burnedStatusEffect, electrifiedStatusEffect, confusedStatusEffect, stinkyStatusEffect, cursedStatusEffect, slowedStatusEffect,
                                                                poisonedStatusEffect, frozenStatusEffect, slimedStatusEffect, tickDamage, tickTiming, tickDamageDuration);
                        }

                        #region VFX & Misc Response to Successful Hit
                        if (isProjectile || isBoomerang)
                        {
                            GetComponent<projectile>().impact();
                        }

                        if (isJabOrSlash)
                        {
                            partReference.triggerJabOrSlashHitDetect();
                            mainSystem.myPlayer.forceStopLeap();

                            if (isChargingJab && isFullyChargedHeavy)
                            {
                                mainSystem.myPlayer.endChargeForward();
                                partReference.endChargeForward();
                                isFullyChargedHeavy = false;
                            }
                        }

                        if (isGrapple)
                        {
                            //mainSystem.myPlayer.leapAttackForward();

                            if (directionOfAttack == 1 || directionOfAttack == -1)
                            {
                                //grapple into a body smash
                                mainSystem.myPlayer.playerGrapple(damagedMonster.myPlayer);
                            }
                            else if (directionOfAttack == 2)
                            {
                                //grapple into a body smash upwards
                            }
                            else if (directionOfAttack == 0)
                            {
                                //grapple into a body smash & jump
                            }
                        }
                        #endregion

                        attackHistory.Add(damagedMonster);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        if (other.gameObject.tag == "Solid")
        {
            pointOfContact = other.ClosestPointOnBounds(transform.position);

            if (isGrapple)
            {
                //mainSystem.myPlayer.leapAttackForward();

                if (directionOfAttack == 1 || directionOfAttack == -1)
                {
                    //mainSystem.myPlayer.leftRightGrapple(false, pointOfContact);
                    //grapple into wall
                    mainSystem.myPlayer.wallGrapple(pointOfContact);
                    print("wall detected");
                }
                else if (directionOfAttack == 2)
                {
                    //grapple into ceiling
                }
                else if (directionOfAttack == 0)
                {
                    //grapple into floor
                }
            }

            if (isJabOrSlash)
            {
                partReference.triggerJabOrSlashHitDetect();
                mainSystem.myPlayer.forceStopLeap();

                if (isChargingJab && isFullyChargedHeavy)
                {
                    mainSystem.myPlayer.endChargeForward();
                    partReference.endChargeForward();
                    isFullyChargedHeavy = false;
                }
            }
        }

        if (other.GetComponent<collisionMaterial>() != null)
        {
            if (isStomp)
            {
                Vector3 storedCollisionPoint = other.ClosestPoint(transform.position);
                other.GetComponent<collisionMaterial>().spawnVFX(storedCollisionPoint);
                partReference.triggerStompDetectionOff();
            }
        }
    }
}
