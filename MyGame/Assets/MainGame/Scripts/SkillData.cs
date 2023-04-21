using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillData : MonoBehaviour
{
    private Transform _skillList;
    private GameObject _uiPrefab;
    private RectTransform _uiRect;

    private void Awake()
    {
        _skillList = GameObject.Find("SkillList").transform;

        transform.SetParent(_skillList, false);
        gameObject.SetActive(false);
    }
    private IEnumerator ReadyCoroutine()
    {
        _uiRect.localPosition = new Vector2(100.0f, 0.0f);

        for (float i = _uiRect.localPosition.x; i >= 0; i -= GetXMovement())
        {
            _uiRect.localPosition = new Vector2(i, 0.0f);
            yield return YieldCache.WaitForFixedUpdate;
        }

        _uiRect.localPosition = Vector2.zero;
    }
    private float GetXMovement()
    {
        return (_uiRect.localPosition.x + 10) * 90 * 0.01f * Time.deltaTime * 10;
    }
    protected IEnumerator FadeOutObject()
    {
        TryGetComponent(out CanvasGroup canvasGroup);

        while (canvasGroup.alpha > float.Epsilon)
        {
            canvasGroup.alpha -= 0.05f;
            yield return YieldCache.WaitForFixedUpdate;
        }

        canvasGroup.alpha = 1.0f;
        _uiRect.SetParent(_skillList, false);
        gameObject.SetActive(false);
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
        _uiPrefab.TryGetComponent(out _uiRect);
        name = _uiPrefab.name;
    }
    public void SetUIParent(Transform parent) { _uiPrefab.transform.SetParent(parent, false); }
}
