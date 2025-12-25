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
    public static Vector2 QuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        return (1 - t) * (1 - t) * start + 2f * (1 - t) * t * control + t * t * end;
    }
    public static Vector2 DiagonalFly(Vector2 start, Vector2 vel, Vector2 gravity, float t )
    {
        return start + vel*t + 0.5f *t*t*gravity ;
    }
}