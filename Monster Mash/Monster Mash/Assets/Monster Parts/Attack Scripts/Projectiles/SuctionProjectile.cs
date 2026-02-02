using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuctionProjectile : NewProjectile
{
    public float TotalTickTime { set => totalTickTime = value; }

    private Vector3 contactPoint;
    private Vector3 originalScale;
    private float totalTickTime;
    private float elapsedTickTime;
    private Coroutine tickCoroutine;

    protected override void OnPlayerHit(Collider other, playerController player)
    {
        contactPoint = other.ClosestPointOnBounds(transform.position);
        AttatchToPlayer(other);
        tickCoroutine ??= StartCoroutine(TickDamage(player));
    }

    private void AttatchToPlayer(Collider other)
    {
        if (delayedDeactivateCoroutine != null)
        {
            StopCoroutine(delayedDeactivateCoroutine);
            delayedDeactivateCoroutine = null;
        }

        originalScale = transform.localScale;
        transform.SetParent(other.transform);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        
    }

    private void DetatachFromPlayer()
    {
        transform.SetParent(null);
        transform.localScale = originalScale;
        tickCoroutine = null;
        DeactivateProjectile();
    }

    private IEnumerator TickDamage(playerController target)
    {
        float tickDelay = 1f;

        while (elapsedTickTime < totalTickTime)
        {
            elapsedTickTime += tickDelay;
            target.damaged(damage, false, transform.position, contactPoint);
            yield return new WaitForSeconds(tickDelay);
        }

        if (elapsedTickTime >= totalTickTime)
        {
            elapsedTickTime = 0f;
            DetatachFromPlayer();
        }
    }
}
