using Pathfinding;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Path = Pathfinding.Path;

[RequireComponent(typeof(Seeker))]
public class AStarFollower2D : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform target;
    public float repathInterval = 0.35f;
    public float waypointReachDist = 0.25f;

    private Seeker seeker;
    private Path path;
    private int waypointIndex;
    private float repathTimer;

    public bool HasPath => path != null && path.vectorPath != null && path.vectorPath.Count > 0;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
    }

    private void Update()
    {
        repathTimer -= Time.deltaTime;
        if (target != null && repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            RequestPath();
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
        repathTimer = 0f;
    }

    private void RequestPath()
    {
        if (seeker == null || target == null) return;
        if (!seeker.IsDone()) return;

        seeker.StartPath(transform.position, target.position, OnPathComplete);
        // Seeker docs: handles path calls for a unit :contentReference[oaicite:4]{index=4}
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;
        path = p;
        waypointIndex = 0;
    }

    public Vector2 GetDesiredDirection()
    {
        if (!HasPath) return Vector2.zero;

        var vPath = path.vectorPath;
        Vector2 pos = transform.position;

        while (waypointIndex < vPath.Count && Vector2.Distance(pos, (Vector2)vPath[waypointIndex]) <= waypointReachDist)
            waypointIndex++;

        if (waypointIndex >= vPath.Count) return Vector2.zero;

        Vector2 next = vPath[waypointIndex];
        Vector2 dir = next - pos;
        return dir.sqrMagnitude < 0.0001f ? Vector2.zero : dir.normalized;
    }
}
