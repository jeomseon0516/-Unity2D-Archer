using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillData : MonoBehaviour
{
    private Transform _skillList;
    private GameObject _uiPrefab;
    private bool _isReady;

    private void Awake()
    {
        _isReady = false;

        _uiPrefab = Instantiate(ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.UI, "DogUI"));
        _uiPrefab.transform.parent = gameObject.transform;
        _skillList = GameObject.Find("SkillList").transform;

        transform.parent = _skillList;
        gameObject.SetActive(false);
    }
    private IEnumerator ReadyCoroutine()
    {
        gameObject.SetActive(true);

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
        gameObject.SetActive(false);

        _isReady = false;
        _uiPrefab.transform.localPosition = new Vector2(100.0f, 0.0f);
    }
    public void Ready() { StartCoroutine(ReadyCoroutine()); }
    public void SetUIParent(Transform parent) { _uiPrefab.transform.parent = parent; }
    public bool GetIsReady() { return _isReady; }
    public void SetIsReady(bool isReady) { _isReady = isReady; }
}
