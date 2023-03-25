using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float PI = 3.1415926f;
    public const float M_DEG = 0.3183098f;
    public const float GRAVITY = 0.1f;

    public static float GetDistance(Vector3 p1, Vector3 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }
    public static float GetPositionToRadian(Vector3 p1, Vector3 p2) { return Mathf.Atan2(p1.y - p2.y, p1.x - p2.x); }
}