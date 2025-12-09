using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NewProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    public IObjectPool<NewProjectile> ObjectPool { set => objectPool = value; }
    public float Speed { set => speed = value; }
    public int Damage { set => damage = value; }
    public bool IsReleased {  set => isReleased = value; }
    public playerController PlayerRef { set => playerRef = value; }

    private IObjectPool<NewProjectile> objectPool;
    private float speed;
    private int damage;
    private bool isReleased;
    private playerController playerRef;

    public void Fire()
    {
        rb.AddForce(transform.forward *  speed, ForceMode.VelocityChange);

        // Safety measure incase the projectile escapes the stage
        float delayTime = 5f;
        StartCoroutine(DelayedDeactivate(delayTime));
    }


    private void OnTriggerEnter(Collider other)
    {
        playerController potentialPlayer = other.GetComponentInParent<playerController>();

        if (other.CompareTag("Solid"))
        {
            DeactivateProjectile();
        }
        else if (potentialPlayer != null && potentialPlayer != playerRef)
        {
            potentialPlayer.damaged(damage, false, transform.position, other.ClosestPointOnBounds(transform.position));
            DeactivateProjectile();
        }
    }

    IEnumerator DelayedDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateProjectile();
    }

    private void DeactivateProjectile()
    {
        if (isReleased) { return; }
        isReleased = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        objectPool.Release(this);
    }
}
