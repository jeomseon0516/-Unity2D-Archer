using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float PI = 3.1415926f;
    public const float M_DEG = 0.3183098f;
    public const float GRAVITY = 9.8f;

    public static float GetDistance(Vector2 p1, Vector2 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }
    public static float GetPositionToRadian(Vector2 p1, Vector2 p2) { return Mathf.Atan2(p1.y - p2.y, p1.x - p2.x); }
    public static float ConvertFromRadianToAngle(float radian) { return radian * Mathf.Rad2Deg; }
    public static float ConvertFromAngleToRadian(float angle)  { return angle  * Mathf.Deg2Rad; }
}