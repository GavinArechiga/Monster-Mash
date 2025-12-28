using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public sealed class EnemyChase : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Max distance the enemy can see the player.")]
    public float detectionRadius = 12f;

    [Tooltip("Height of the enemy 'eyes' above its position for LOS checks.")]
    public float eyeHeight = 1.6f;

    [Tooltip("Only colliders on this layer will be considered as a player.")]
    public LayerMask playerLayer;

    [Tooltip("Layers that block vision (e.g., Default, Environment).")]
    public LayerMask obstructionMask;

    [Header("Movement")]
    [Tooltip("Max chase speed.")]
    public float moveSpeed = 5f;

    [Tooltip("How quickly the enemy reaches the desired speed.")]
    public float acceleration = 20f;

    [Tooltip("Stop moving when within this distance of the player.")]
    public float stoppingDistance = 1.25f;

    [Tooltip("How quickly the enemy rotates to face the player.")]
    public float turnSpeed = 10f;

    [Header("Platform Jumping")]
    [Tooltip("Layers considered as platforms for raycasts and overlap checks.")]
    public LayerMask platformLayer;

    [Tooltip("Max vertical gap the enemy is willing to jump up to.")]
    public float maxJumpRise = 6f;

    [Tooltip("Max horizontal distance the enemy will attempt to jump.")]
    public float maxJumpReach = 12f;

    [Tooltip("Extra downward raycast distance to detect ground landing after jump.")]
    public float groundProbeDistance = 0.75f;

    [Tooltip("Cooldown after a jump before another can be attempted.")]
    public float jumpCooldown = 0.75f;

    [Tooltip("When searching for PlatformJumpPoints near the player, use this radius.")]
    public float platformSearchRadius = 8f;

    [Tooltip("Initial speed hint for the ballistic solver. 0 lets the solver estimate.")]
    public float jumpInitialSpeed = 10f;

    [Tooltip("If true, draw a debug line of the attempted jump arc at runtime.")]
    public bool drawRuntimeArc = true;

    Rigidbody _rb;
    Transform _target; // Player transform when visible, null otherwise.

    // Jump state
    bool _isJumping;
    float _lastJumpTime;
    Vector3 _lastJumpVelocity;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void FixedUpdate()
    {
        // Try to acquire or validate target visibility each physics step.
        AcquireTarget();

        if (_isJumping)
        {
            MaintainAirborneFacing();
            DetectLandingAndReset();
            return;
        }

        // If we see the player, first raycast from enemy to player and check if it hits a platform.
        if (_target != null)
        {
            if (TryRaycastPlatformBetweenEnemyAndPlayer(_target, out PlatformJumpPoints pjp, out Vector3 hitPointOnPlatform))
            {
                if (TryJumpViaPlatform(pjp, _target, hitPointOnPlatform))
                {
                    return; // jump initiated; skip ground movement this frame
                }
            }

            // If no platform directly between, still try the previous heuristic (player above platform nearby).
            if (TryJumpTowardsPlayerPlatform(_target))
            {
                return;
            }
        }

        // Ground chase movement
        Vector3 desiredVelocity = Vector3.zero;

        if (_target != null)
        {
            Vector3 toTarget = _target.position - transform.position;
            float distance = toTarget.magnitude;

            if (distance > stoppingDistance)
            {
                Vector3 dir = toTarget.normalized;
                desiredVelocity = dir * moveSpeed;

                if (dir.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
                    Quaternion newRot = Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
                    _rb.MoveRotation(newRot);
                }
            }
        }

        Vector3 newVel = Vector3.MoveTowards(_rb.velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
        newVel.y = _rb.velocity.y;
        _rb.velocity = newVel;
    }

    void AcquireTarget()
    {
        Transform candidate = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer, QueryTriggerInteraction.Ignore);
        if (hits.Length > 0)
        {
            float bestDist = float.PositiveInfinity;
            Vector3 eye = transform.position + Vector3.up * eyeHeight;

            for (int i = 0; i < hits.Length; i++)
            {
                Transform t = hits[i].transform;
                Vector3 to = (TargetCenter(hits[i]) - eye);
                float dist = to.magnitude;
                if (dist < 0.001f) continue;

                Vector3 dir = to / dist;

                // Check line of sight (blocked by obstructionMask)
                if (Physics.Raycast(eye, dir, out RaycastHit hit, dist, obstructionMask, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                if (dist < bestDist)
                {
                    bestDist = dist;
                    candidate = t;
                }
            }
        }

        _target = candidate;
    }

    static Vector3 TargetCenter(Collider c)
    {
        if (c is CapsuleCollider cap)
        {
            Vector3 up = cap.transform.up * cap.height * 0.5f;
            return cap.transform.TransformPoint(cap.center + up * 0.5f);
        }
        if (c is CharacterController chr)
        {
            return chr.transform.TransformPoint(chr.center + Vector3.up * chr.height * 0.25f);
        }
        return c.bounds.center;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // -------- Raycast to platform between enemy and player --------

    bool TryRaycastPlatformBetweenEnemyAndPlayer(Transform player, out PlatformJumpPoints platformJumpPoints, out Vector3 platformHitPoint)
    {
        platformJumpPoints = null;
        platformHitPoint = default;

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = player.position - origin;
        float dist = toPlayer.magnitude;
        if (dist < 0.001f) return false;

        Vector3 dir = toPlayer / dist;

        // Raycast only against platforms to detect a platform along the line
        int raycastMask = platformLayer;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, raycastMask, QueryTriggerInteraction.Ignore))
        {
            platformJumpPoints = hit.collider.GetComponentInParent<PlatformJumpPoints>();
            if (platformJumpPoints != null)
            {
                platformHitPoint = hit.point;
                return true;
            }
        }

        return false;
    }

    // -------- Platform integration using raycast result --------

    bool TryJumpViaPlatform(PlatformJumpPoints pjp, Transform player, Vector3 platformHitPoint)
    {
        // Cooldown guard
        if (Time.time - _lastJumpTime < jumpCooldown) return false;

        // Player must be above us by some margin
        float verticalDelta = player.position.y - transform.position.y;
        if (verticalDelta <= 0.5f || verticalDelta > maxJumpRise) return false;

        var starts = pjp.GetJumpStartsWorld();
        var landings = pjp.GetLandingsWorld();
        if (starts.Count == 0 || landings.Count == 0) return false;

        // Choose start closest to enemy on XZ
        int startIdx = FindClosestIndexXZ(transform.position, starts);
        Vector3 start = starts[startIdx];

        // Prefer landing closest to the raycast hit point on that platform
        int landingIdx = FindClosestIndexXZ(platformHitPoint, landings);
        Vector3 landing = landings[landingIdx];

        // Reachability checks
        Vector3 toLanding = landing - start;
        float horiz = new Vector3(toLanding.x, 0f, toLanding.z).magnitude;
        float rise = toLanding.y;
        if (horiz > maxJumpReach || rise > maxJumpRise) return false;

        if (!pjp.TryComputeLaunchVelocity(start, landing, jumpInitialSpeed, out Vector3 v0))
            return false;

        // Move enemy closer to start if needed
        Vector3 toStart = start - transform.position;
        Vector3 toStartXZ = new Vector3(toStart.x, 0f, toStart.z);
        float distToStartXZ = toStartXZ.magnitude;

        if (distToStartXZ > 0.75f)
        {
            Vector3 dir = toStartXZ.normalized;
            Vector3 desired = dir * moveSpeed;
            Vector3 newVel = Vector3.MoveTowards(_rb.velocity, desired, acceleration * Time.fixedDeltaTime);
            newVel.y = _rb.velocity.y;
            _rb.velocity = newVel;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
                Quaternion newRot = Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
                _rb.MoveRotation(newRot);
            }
            return false;
        }

        // Face landing direction and jump
        Vector3 faceDir = new Vector3(landing.x - start.x, 0f, landing.z - start.z).normalized;
        if (faceDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }

        Vector3 startPos = new Vector3(start.x, transform.position.y, start.z);
        transform.position = startPos;

        _lastJumpVelocity = v0;
        _rb.velocity = _lastJumpVelocity;
        _isJumping = true;
        _lastJumpTime = Time.time;

        if (drawRuntimeArc)
        {
            float g = Mathf.Abs(Physics.gravity.y);
            float tEst = EstimateFlightTime(startPos, _lastJumpVelocity, landing, g);
            int samples = 24;
            Vector3 prev = startPos;
            for (int i = 1; i <= samples; i++)
            {
                float t = (tEst * i) / samples;
                Vector3 p = startPos + _lastJumpVelocity * t + 0.5f * new Vector3(0f, -g, 0f) * (t * t);
                Debug.DrawLine(prev, p, Color.cyan, 0.25f);
                prev = p;
            }
        }

        return true;
    }

    // Fallback: heuristic when player is above some nearby platform
    bool TryJumpTowardsPlayerPlatform(Transform player)
    {
        if (Time.time - _lastJumpTime < jumpCooldown) return false;

        float verticalDelta = player.position.y - transform.position.y;
        if (verticalDelta <= 0.5f || verticalDelta > maxJumpRise) return false;

        Collider[] platformHits = Physics.OverlapSphere(player.position, platformSearchRadius, platformLayer, QueryTriggerInteraction.Ignore);
        if (platformHits == null || platformHits.Length == 0) return false;

        PlatformJumpPoints bestPjp = null;
        float bestScore = float.NegativeInfinity;
        Vector3 bestStart = Vector3.zero;
        Vector3 bestLanding = Vector3.zero;

        foreach (var ph in platformHits)
        {
            PlatformJumpPoints pjp = ph.GetComponentInParent<PlatformJumpPoints>();
            if (pjp == null) continue;

            var starts = pjp.GetJumpStartsWorld();
            var landings = pjp.GetLandingsWorld();
            if (starts.Count == 0 || landings.Count == 0) continue;

            int startIdx = FindClosestIndexXZ(transform.position, starts);
            Vector3 start = starts[startIdx];

            int landingIdx = FindClosestIndexXZ(player.position, landings);
            Vector3 landing = landings[landingIdx];

            Vector3 toLanding = landing - start;
            float horiz = new Vector3(toLanding.x, 0f, toLanding.z).magnitude;
            float rise = toLanding.y;
            if (horiz > maxJumpReach || rise > maxJumpRise) continue;

            if (pjp.TryComputeLaunchVelocity(start, landing, jumpInitialSpeed, out Vector3 v0))
            {
                float score = -horiz - Vector3.Distance(landing, player.position) * 0.5f;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPjp = pjp;
                    bestStart = start;
                    bestLanding = landing;
                    _lastJumpVelocity = v0;
                }
            }
        }

        if (bestPjp == null) return false;

        Vector3 toStart = bestStart - transform.position;
        Vector3 toStartXZ = new Vector3(toStart.x, 0f, toStart.z);
        float distToStartXZ = toStartXZ.magnitude;

        if (distToStartXZ > 0.75f)
        {
            Vector3 dir = toStartXZ.normalized;
            Vector3 desired = dir * moveSpeed;
            Vector3 newVel = Vector3.MoveTowards(_rb.velocity, desired, acceleration * Time.fixedDeltaTime);
            newVel.y = _rb.velocity.y;
            _rb.velocity = newVel;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
                Quaternion newRot = Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
                _rb.MoveRotation(newRot);
            }
            return false;
        }

        Vector3 faceDir = new Vector3(bestLanding.x - bestStart.x, 0f, bestLanding.z - bestStart.z).normalized;
        if (faceDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }

        Vector3 startPos = new Vector3(bestStart.x, transform.position.y, bestStart.z);
        transform.position = startPos;

        _rb.velocity = _lastJumpVelocity;
        _isJumping = true;
        _lastJumpTime = Time.time;

        if (drawRuntimeArc)
        {
            float g = Mathf.Abs(Physics.gravity.y);
            float tEst = EstimateFlightTime(startPos, _lastJumpVelocity, bestLanding, g);
            int samples = 24;
            Vector3 prev = startPos;
            for (int i = 1; i <= samples; i++)
            {
                float t = (tEst * i) / samples;
                Vector3 p = startPos + _lastJumpVelocity * t + 0.5f * new Vector3(0f, -g, 0f) * (t * t);
                Debug.DrawLine(prev, p, Color.yellow, 0.25f);
                prev = p;
            }
        }

        return true;
    }

    void MaintainAirborneFacing()
    {
        Vector3 vel = _rb.velocity;
        Vector3 dir = new Vector3(vel.x, 0f, vel.z);
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }
    }

    void DetectLandingAndReset()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundProbeDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (_rb.velocity.y <= 0f)
            {
                _isJumping = false;
            }
        }
    }

    static int FindClosestIndexXZ(Vector3 pos, IReadOnlyList<Vector3> points)
    {
        int best = 0;
        float bestSqr = float.PositiveInfinity;
        Vector3 pXZ = new Vector3(pos.x, 0f, pos.z);
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 q = points[i];
            float d = (new Vector3(q.x, 0f, q.z) - pXZ).sqrMagnitude;
            if (d < bestSqr) { bestSqr = d; best = i; }
        }
        return best;
    }

    static float EstimateFlightTime(Vector3 origin, Vector3 v0, Vector3 target, float g)
    {
        Vector3 to = target - origin;
        Vector3 v0XZ = new Vector3(v0.x, 0f, v0.z);
        float speedXZ = v0XZ.magnitude;
        float distXZ = new Vector3(to.x, 0f, to.z).magnitude;
        float tHoriz = speedXZ > 0.0001f ? distXZ / speedXZ : 0f;

        float a = -0.5f * g;
        float b = v0.y;
        float c = origin.y - target.y;

        float tVert = tHoriz;
        float disc = b * b - 4f * a * c;
        if (disc >= 0f)
        {
            float sqrt = Mathf.Sqrt(disc);
            float t1 = (-b + sqrt) / (2f * a);
            float t2 = (-b - sqrt) / (2f * a);
            float best = tHoriz;
            if (t1 > 0f && Mathf.Abs(t1 - tHoriz) < Mathf.Abs(best - tHoriz)) best = t1;
            if (t2 > 0f && Mathf.Abs(t2 - tHoriz) < Mathf.Abs(best - tHoriz)) best = t2;
            tVert = best;
        }

        return tHoriz > 0f ? Mathf.Lerp(tHoriz, tVert, 0.5f) : Mathf.Max(tVert, 0.1f);
    }
}