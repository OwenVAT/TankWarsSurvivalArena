//using UnityEngine;

//public class TrajectoryRenderer : MonoBehaviour
//{
//    [SerializeField] private LineRenderer lineRenderer;

//    private void Awake()
//    {
//        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
//        Hide();
//    }

//    public void Hide()
//    {
//        lineRenderer.enabled = false;
//        lineRenderer.positionCount = 0;
//    }

//    public void DrawCurve(Vector2 start, Vector2 velocity, Vector2 acceleration, float totalTime, float step)
//    {
//        lineRenderer.enabled = true;
//        int count = Mathf.Max(2, Mathf.CeilToInt(totalTime / Mathf.Max(0.001f, step)));
//        lineRenderer.positionCount = count;

//        float t = 0f;
//        for (int i = 0; i < count; i++)
//        {
//            Vector2 p = Utilities.GetPosDiagonalShoot(start, velocity, acceleration, t);
//            lineRenderer.SetPosition(i, p);
//            t += step;
//        }
//    }
//}



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

    // ===== Existing DrawCurve (parabol) giữ nguyên nếu bạn còn dùng =====
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

    // ===== NEW: Draw Cubic Bezier =====
    public void DrawBezierCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int segments)
    {
        lineRenderer.enabled = true;

        int count = Mathf.Max(2, segments + 1);
        lineRenderer.positionCount = count;

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : (i / ((float)count - 1));
            Vector2 p = Utilities.BezierCubic(p0, p1, p2, p3, t);
            lineRenderer.SetPosition(i, p);
        }
    }
}

