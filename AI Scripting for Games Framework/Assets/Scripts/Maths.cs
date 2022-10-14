using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {
        return Mathf.Sqrt((a.x * a.x) + (a.y * a.y));
    }

    public static Vector2 Normalise(Vector2 a)
    {
        float magnitude = Magnitude(a);
        return new Vector2(a.x/ magnitude, a.y / magnitude);
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        // Dot Product = Cos Theta
        Vector2 a = Normalise(lhs);
        Vector2 b = Normalise(rhs);
        return (a.x * b.x) + (a.y * b.y);
    }

    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        return Mathf.Acos(Dot(lhs, rhs));
    }

    public static Vector2 RotateVector(Vector2 vector, float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        float x = (vector.x * Mathf.Cos(rad)) - (vector.y * Mathf.Sin(rad));
        float y = (vector.x * Mathf.Sin(rad)) + (vector.y * Mathf.Cos(rad));
        return new Vector2(x, y);
    }

    public static Vector2 RotateVectorWithOffset(Vector2 offsetPoint, Vector2 rotationPoint, float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        Vector2 localOffset = offsetPoint - rotationPoint;
        Vector2 rotatedOffset = new Vector2();
        rotatedOffset.x = localOffset.x * Mathf.Cos(rad) - localOffset.y * Mathf.Sin(rad);
        rotatedOffset.y = localOffset.x * Mathf.Sin(rad) + localOffset.y * Mathf.Cos(rad);

        return rotationPoint + rotatedOffset;
    }
}
