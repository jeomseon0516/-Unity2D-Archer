using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Default
{
    public static int GetIntParseString(string number)
    {
        int num = 0;
        return int.TryParse(number, out num) ? num : 0; 
    }
    public static string GetRemoveSelectString(string str, string selectStr)
    {
        int index = str.IndexOf(selectStr);
        return index > 0 ? str.Substring(0, index) : str;
    }
    public static float GetDistance(Vector2 p1, Vector2 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }
    public static float GetFloatParseString(string number)
    {
        float num = 0.0f;
        return float.TryParse(number, out num) ? num : 0.0f;
    }
    public static float GetPositionToRadian(Vector2 p1, Vector2 p2) { return Mathf.Atan2(p1.y - p2.y, p1.x - p2.x); }
    public static float ConvertFromRadianToAngle(float radian) { return radian * Mathf.Rad2Deg; }
    public static float ConvertFromAngleToRadian(float angle) { return angle * Mathf.Deg2Rad; }
}
