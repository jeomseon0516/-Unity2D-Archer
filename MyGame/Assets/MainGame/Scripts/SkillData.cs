using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillData : MonoBehaviour
{
    private Transform _skillList;
    private GameObject _uiPrefab;

    private void Awake()
    {
        _skillList = GameObject.Find("SkillList").transform;

        gameObject.SetActive(false);
        transform.SetParent(_skillList);
    }
    private IEnumerator ReadyCoroutine()
    {
        gameObject.SetActive(true);
        _uiPrefab.transform.localPosition = new Vector2(100.0f, 0.0f);

        for (float i = _uiPrefab.transform.localPosition.x; i >= -10; i -= GetXMovement())
            yield return YieldCache.WaitForFixedUpdate;

        _uiPrefab.transform.localPosition = Vector2.zero;
    }
    private float GetXMovement()
    {
        return _uiPrefab.transform.localPosition.x - (-10) * 90 * 0.01f * Time.fixedDeltaTime;
    }
    protected IEnumerator FadeOutObject()
    {
        TryGetComponent(out CanvasGroup canvasGroup);

        while (canvasGroup.alpha > float.Epsilon)
        {
            canvasGroup.alpha -= 0.05f;
            yield return YieldCache.WaitForFixedUpdate;
        }

        gameObject.SetActive(false);
        canvasGroup.alpha = 1.0f;
        transform.parent = _skillList;
    }
    public void Use()
    {
        StartCoroutine(FadeOutObject());
    }
    public void Ready() 
    {
        gameObject.SetActive(true);
        StartCoroutine(ReadyCoroutine()); 
    }
    public void SetUIPrefab(GameObject uiPrefab) 
    { 
        _uiPrefab = uiPrefab;
        name = _uiPrefab.name;
    }
    public void SetUIParent(Transform parent) { _uiPrefab.transform.SetParent(parent); }
}
