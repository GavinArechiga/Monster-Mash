using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to a platform. Use empty child GameObjects to mark jump start points (where enemies should jump from)
/// and landing points (where they should land on the platform). Draws gizmo spheres at those points and
/// an arc preview for the computed trajectory from a start to a landing point.
/// </summary>
[DisallowMultipleComponent]
public sealed class PlatformJumpPoints : MonoBehaviour
{
    [Header("Jump Points (Transforms)")]
    [Tooltip("Empty child objects that mark jump start positions (where enemies launch from).")]
    public Transform[] jumpStarts;

    [Tooltip("Empty child objects that mark landing positions (where enemies aim to land on this platform).")]
    public Transform[] landings;

    [Header("Jump Settings")]
    [Tooltip("Gravity magnitude used to compute the ballistic arc. Defaults to Physics.gravity.y if zero.")]
    public float gravityY = 0f;

    [Tooltip("Desired initial jump speed. If zero, a speed will be computed that minimally reaches the target.")]
    public float initialSpeed = 10f;

    [Tooltip("Max allowed initial speed. Prevents absurd velocities when auto-computing.")]
    public float maxInitialSpeed = 18f;

    [Tooltip("Number of samples used to draw the preview arc.")]
    [Range(8, 128)]
    public int arcSamples = 32;

    [Tooltip("Gizmo sphere radius for points.")]
    public float gizmoRadius = 0.2f;

    [Tooltip("Color for landing points gizmo spheres.")]
    public Color landingColor = new Color(0.2f, 0.8f, 1f);

    [Tooltip("Color for jump start points gizmo spheres.")]
    public Color startColor = new Color(1f, 0.6f, 0.2f);

    [Tooltip("Color for arc preview lines.")]
    public Color arcColor = new Color(0.9f, 0.9f, 0.2f);

    [Header("Targeting")]
    [Tooltip("If true, arcs will be previewed from each start to the closest landing point.")]
    public bool previewClosest = true;

    [Tooltip("If false, arcs are only drawn when the object is selected.")]
    public bool alwaysDrawGizmos = false;

    /// <summary>
    /// World positions for starts and landings.
    /// </summary>
    public IReadOnlyList<Vector3> GetJumpStartsWorld()
    {
        _worldStarts.Clear();
        if (jumpStarts != null)
        {
            for (int i = 0; i < jumpStarts.Length; i++)
            {
                var t = jumpStarts[i];
                if (t != null) _worldStarts.Add(t.position);
            }
        }
        return _worldStarts;
    }

    public IReadOnlyList<Vector3> GetLandingsWorld()
    {
        _worldLandings.Clear();
        if (landings != null)
        {
            for (int i = 0; i < landings.Length; i++)
            {
                var t = landings[i];
                if (t != null) _worldLandings.Add(t.position);
            }
        }
        return _worldLandings;
    }

    /// <summary>
    /// Computes a launch velocity to go from origin to target using ballistic motion.
    /// Returns true if a solution is found. Chooses the lower-angle solution when possible.
    /// </summary>
    public bool TryComputeLaunchVelocity(Vector3 origin, Vector3 target, float preferredSpeed, out Vector3 launchVelocity)
    {
        launchVelocity = Vector3.zero;

        float g = gravityY != 0f ? Mathf.Abs(gravityY) : Mathf.Abs(Physics.gravity.y);
        Vector3 to = target - origin;

        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float xzDist = toXZ.magnitude;
        float y = to.y;

        float v = preferredSpeed > 0f ? preferredSpeed
                                      : Mathf.Clamp(Mathf.Sqrt(g * Mathf.Max(2f * (xzDist + Mathf.Abs(y)), 0.01f)), 6f, maxInitialSpeed);

        float x = Mathf.Max(xzDist, 0.001f);
        float v2 = v * v;
        float a = (g * x * x) / (2f * v2);
        float b = -x;
        float c = y;

        float disc = b * b - 4f * a * c;
        if (disc < 0f)
        {
            float vTry = v;
            bool found = false;
            for (int i = 0; i < 5 && !found; i++)
            {
                vTry = Mathf.Min(vTry + 2f, maxInitialSpeed);
                v2 = vTry * vTry;
                a = (g * x * x) / (2f * v2);
                disc = b * b - 4f * a * c;
                if (disc >= 0f)
                {
                    v = vTry;
                    found = true;
                }
            }
            if (!found) return false;
        }

        float sqrtDisc = Mathf.Sqrt(Mathf.Max(disc, 0f));
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);

        float tanTheta = Mathf.Min(t1, t2);
        float theta = Mathf.Atan(tanTheta);

        Vector3 dirXZ = xzDist > 0.0001f ? toXZ / xzDist : Vector3.forward;

        float cos = Mathf.Cos(theta);
        float sin = Mathf.Sin(theta);
        float vx = v * cos;
        float vy = v * sin;

        launchVelocity = dirXZ * vx + Vector3.up * vy;
        return true;
    }

    /// <summary>
    /// Returns the platform bounds based on attached renderers or colliders.
    /// </summary>
    public Bounds GetPlatformBounds()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            var b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
            return b;
        }
        var colliders = GetComponentsInChildren<Collider>();
        if (colliders != null && colliders.Length > 0)
        {
            var b = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++) b.Encapsulate(colliders[i].bounds);
            return b;
        }
        return new Bounds(transform.position, Vector3.one);
    }

    // Gizmo drawing caches
    private readonly List<Vector3> _worldStarts = new List<Vector3>(16);
    private readonly List<Vector3> _worldLandings = new List<Vector3>(16);

    void OnDrawGizmos()
    {
        if (!alwaysDrawGizmos) return;
        DrawGizmosInternal();
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmosInternal();
    }

    private void DrawGizmosInternal()
    {
        var starts = GetJumpStartsWorld();
        var landingPts = GetLandingsWorld();

        // Draw start points
        Gizmos.color = startColor;
        for (int i = 0; i < starts.Count; i++)
            Gizmos.DrawSphere(starts[i], gizmoRadius);

        // Draw landing points
        Gizmos.color = landingColor;
        for (int i = 0; i < landingPts.Count; i++)
            Gizmos.DrawSphere(landingPts[i], gizmoRadius);

        // Draw arcs preview
        Gizmos.color = arcColor;
        if (starts.Count > 0 && landingPts.Count > 0)
        {
            for (int i = 0; i < starts.Count; i++)
            {
                Vector3 origin = starts[i];
                int targetIndex = previewClosest ? FindClosestIndex(origin, landingPts) : Mathf.Min(i, landingPts.Count - 1);
                Vector3 target = landingPts[targetIndex];

                if (TryComputeLaunchVelocity(origin, target, initialSpeed, out Vector3 v0))
                {
                    DrawBallisticArc(origin, v0, target, arcSamples);
                }
                else
                {
                    Gizmos.DrawLine(origin, target);
                }
            }
        }
    }

    private int FindClosestIndex(Vector3 pos, IReadOnlyList<Vector3> points)
    {
        int best = 0;
        float bestSqr = float.PositiveInfinity;
        for (int i = 0; i < points.Count; i++)
        {
            float d = (points[i] - pos).sqrMagnitude;
            if (d < bestSqr) { bestSqr = d; best = i; }
        }
        return best;
    }

    private void DrawBallisticArc(Vector3 origin, Vector3 v0, Vector3 target, int samples)
    {
        float g = gravityY != 0f ? Mathf.Abs(gravityY) : Mathf.Abs(Physics.gravity.y);
        float timeToTarget = EstimateFlightTime(origin, v0, target, g);

        Vector3 prev = origin;
        for (int i = 1; i <= samples; i++)
        {
            float t = (timeToTarget * i) / samples;
            Vector3 p = origin + v0 * t + 0.5f * new Vector3(0f, -g, 0f) * (t * t);
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }

    private float EstimateFlightTime(Vector3 origin, Vector3 v0, Vector3 target, float g)
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