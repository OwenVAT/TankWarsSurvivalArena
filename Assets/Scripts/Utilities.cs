using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static Vector2 GetPerpendicularUp(Vector2 start, Vector2 end)
    {
        Vector2 p = end - start;
        if (p.x > 0) { return new Vector2(-p.y, p.x).normalized; }
        else
        {
            return new Vector2(p.y, -p.x).normalized;
        }
    }
    public static float GetAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }
    public static Vector2 GetPosDiagonalShoot(Vector2 startPos, Vector2 velocity, Vector2 acceleration, float time)
    {
        return startPos + velocity * time + 0.5f * acceleration * time * time;
    }
    public static Vector2 GetVelocityDiagonalShoot(Vector2 velocity, Vector2 acceleration, float time)
    {
        return velocity + acceleration * time;
    }






    public static Vector2 BezierCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1f - t;
        return u * u * u * p0 + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * p3;
    }

    public static Vector2 BezierCubicDerivative(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1f - t;
        // B'(t)=3(1-t)^2(P1-P0)+6(1-t)t(P2-P1)+3t^2(P3-P2)
        return 3f * u * u * (p1 - p0) + 6f * u * t * (p2 - p1) + 3f * t * t * (p3 - p2);
    }




    public struct RocketBezierPath
    {
        public Vector2 p0, p1, p2, p3;
        public float flightTime;
    }

    /// <summary>
    /// 4 point of Bezier 
    /// - p0: start (firePoint)
    /// - p1: start direction by turretDirection
    /// - p3: endPosition ==>  ≤ maxRange
    /// - p2: tạo cong, nhưng cong giảm khi bắn gần thẳng lên/xuống
    /// - flightTime:  tính theo "speed" và độ dài xấp xỉ của curve
    /// </summary>
    public static RocketBezierPath BuildRocketBezier(
        Vector2 start,
        Vector2 firePointDirection,
        float maxRange,
        float speed,
        float curveFactor,
        float startStraightLen,
        float endStraightLen,
        float verticalCurvePow,
        int curveSide // +1 / -1 (PHẢI ổn định giữa preview và fire)
    )
    {
        RocketBezierPath path = new RocketBezierPath();

        Vector2 dir = firePointDirection.normalized;

        // p0
        path.p0 = start;

        // p3 = clamp range
        Vector2 delta = dir * maxRange;
        path.p3 = start + delta;

        // toTarget
        float dLen = delta.magnitude;
        

        // Normal theo hướng tới đích
        Vector2 n = new Vector2(-dir.y, dir.x);

        // Scale cong giảm khi bắn gần thẳng đứng (up/down theo world Y)
        // curveScale = (1 - |dir.y|)^k
        float curveScale = Mathf.Pow(1f - Mathf.Abs(dir.y), Mathf.Max(0.01f, verticalCurvePow));

        // p1: khóa hướng xuất phát theo NÒNG (turretDirection)
        float sLen = Mathf.Min(startStraightLen, dLen * 0.6f);
        path.p1 = path.p0 + dir * sLen;

        // p2: điều khiển cong về cuối
        float eLen = Mathf.Min(endStraightLen, dLen * 0.6f);
        float curvature = curveFactor * dLen * curveScale;

        // Nếu bắn gần thẳng đứng, curvature ~ 0 => đường gần thẳng (p2 nằm trên line)
        path.p2 = path.p3 - dir * eLen + n * curvature * Mathf.Sign(curveSide);

        // Flight time theo speed và độ dài xấp xỉ (đủ ổn cho game)
        float approxLen = (path.p0 - path.p1).magnitude + (path.p1 - path.p2).magnitude + (path.p2 - path.p3).magnitude;
        path.flightTime = approxLen / Mathf.Max(0.01f, speed);

        // Tránh quá ngắn
        path.flightTime = Mathf.Max(0.08f, path.flightTime);

        return path;
    }
}





