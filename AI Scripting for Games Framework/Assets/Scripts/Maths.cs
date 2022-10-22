using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {
        // Uses the pythagoras theorem to of the vector's x and y values to calculate the magnitude
        return Mathf.Sqrt((a.x * a.x) + (a.y * a.y));
    }

    public static Vector2 Normalise(Vector2 a)
    {
        // Calculates the unit vector
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
        // angle = inverse cos of the dot product
        return Mathf.Acos(Dot(lhs, rhs));
    }

    // Rotates a vector around the origin
    public static Vector2 RotateVector(Vector2 vector, float degree)
    {
        // converts the angle to radians
        float rad = degree * Mathf.Deg2Rad;
        // gets the rotation vector
        float x = (vector.x * Mathf.Cos(rad)) - (vector.y * Mathf.Sin(rad));
        float y = (vector.x * Mathf.Sin(rad)) + (vector.y * Mathf.Cos(rad));
        return new Vector2(x, y);
    }

    // Rotates a vector around a specific point
    public static Vector2 RotateVectorWithOffset(Vector2 offsetPoint, Vector2 rotationPoint, float degree)
    {               
        // converts the angle to radians
        float rad = degree * Mathf.Deg2Rad;
        // calculates the offsets amount
        Vector2 localOffset = offsetPoint - rotationPoint;
        Vector2 rotatedOffset = new Vector2();
        // gets the offset rotation vector
        rotatedOffset.x = localOffset.x * Mathf.Cos(rad) - localOffset.y * Mathf.Sin(rad);
        rotatedOffset.y = localOffset.x * Mathf.Sin(rad) + localOffset.y * Mathf.Cos(rad);

        return rotationPoint + rotatedOffset;
    }
}
