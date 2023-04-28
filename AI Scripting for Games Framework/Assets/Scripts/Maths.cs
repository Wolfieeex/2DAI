using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {
        if (a.x == 0 && a.y == 0) return 0.0f;
        else return Mathf.Sqrt(Mathf.Pow(a.x, 2.0f) + Mathf.Pow(a.y, 2.0f));
    }

    public static Vector2 Normalise(Vector2 a)
    {
        Vector2 norVec = new Vector2(a.x / Magnitude(a), a.y / Magnitude(a));
        return norVec;
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        Vector2 norVeclhs = Normalise(lhs);
        Vector2 norVecrhs = Normalise(rhs);
        float dotProd = (norVeclhs.x * norVecrhs.x) + (norVeclhs.y * norVecrhs.y);
        return dotProd;
    }

    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        float deg = Mathf.Acos(Dot(lhs, rhs));
        return deg;
    }

    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        float degAng = angle * Mathf.Deg2Rad;
        float xPos = (vector.x * Mathf.Cos(degAng)) - (vector.y * Mathf.Sin(degAng));
        float yPos = (vector.x * Mathf.Sin(degAng)) + (vector.y * Mathf.Cos(degAng));
        return new Vector2(xPos, yPos);
    }
}
