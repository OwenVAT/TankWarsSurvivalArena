using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        Hide();
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
    }

    public void DrawCurve(Vector2 start, Vector2 velocity, Vector2 acceleration, float totalTime, float step)
    {
        lineRenderer.enabled = true;
        int count = Mathf.Max(2, Mathf.CeilToInt(totalTime / Mathf.Max(0.001f, step)));
        lineRenderer.positionCount = count;

        float t = 0f;
        for (int i = 0; i < count; i++)
        {
            Vector2 p = Utilities.GetPosDiagonalShoot(start, velocity, acceleration, t);
            lineRenderer.SetPosition(i, p);
            t += step;
        }
    }
}
