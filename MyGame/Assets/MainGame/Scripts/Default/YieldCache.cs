using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 해당 클래스는 코루틴을 호출할때마다 매번 new 키워드를 사용시 생기는 가비지를 방지하기 위해 
 */
public static class YieldCache
{
    private static readonly Dictionary<float, WaitForSeconds> _time = new Dictionary<float, WaitForSeconds>();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    public static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    public static WaitForSeconds WaitForSeconds(float time)
    {
        WaitForSeconds wfs;
        if (!_time.TryGetValue(time, out wfs))
            _time.Add(time, wfs = new WaitForSeconds(time));
        return wfs;
    }
}
