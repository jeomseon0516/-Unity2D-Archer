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

        if (index > 0)
            str = str.Substring(0, index);

        return str;
    }
}
