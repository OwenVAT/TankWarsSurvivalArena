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
    


}