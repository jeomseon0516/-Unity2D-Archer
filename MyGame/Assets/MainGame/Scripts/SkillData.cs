using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillData : MonoBehaviour
{
    SkillUI _uiPrefab;
    bool _isReady;

    private void Awake()
    {
        _isReady = false;
    }
    private IEnumerator DoReady()
    {
        for (float i = _uiPrefab.transform.localPosition.x; i >= -10; i -= GetXMovement())
            yield return YieldCache.WaitForFixedUpdate;

        _uiPrefab.transform.localPosition = Vector2.zero;
        _isReady = true;
    }
    private float GetXMovement()
    {
        return _uiPrefab.transform.localPosition.x - (-10) * 90 * 0.01f * Time.fixedDeltaTime;
    }
    public void Use()
    {
        _isReady = false;
        _uiPrefab.transform.localPosition = new Vector2(100.0f, 0.0f);
    }
    public void SetIsReady(bool isReady) { _isReady = isReady; }
}
