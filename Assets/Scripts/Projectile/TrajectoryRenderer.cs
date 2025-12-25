using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int numPoint = 20;
    [SerializeField] private ProjectileConfig rocketConfig;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;

    }
    public void DrawStraight(Vector2 start, Vector2 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public void DrawCurve(Vector2 start, Vector2 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = numPoint;
        Vector2 control = (end + start) * 0.5f + rocketConfig.arcHeight * Utilities.GetPerpendicularUp(start, end); ;
        for (int i = 0; i < numPoint; i++)
        {
            float t = numPoint == 1 ? 1f : i / ((float)numPoint - 1);
            Vector2 point = Utilities.QuadraticBezier(start, control, end, t);
            lineRenderer.SetPosition(i, point);
        }

    }

}
