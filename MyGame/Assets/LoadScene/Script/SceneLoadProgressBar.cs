using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoadProgressBar : MonoBehaviour
{
    private AsyncOperation _asyncOperation;
    private Slider _slider;
    private Text _percent;
    private void Start()
    {
        StartCoroutine(LoadingScene());
    }

    private IEnumerator LoadingScene()
    {
        try
        {
            TryGetComponent(out _slider);
            transform.Find("Percent").TryGetComponent(out _percent);
        }
        catch (System.NullReferenceException ex)
        {
            print("Catch : " + ex.Message);
            yield break;
        }

        _slider.maxValue = 1.0f;
        _asyncOperation = SceneManager.LoadSceneAsync("MainGame");
        _asyncOperation.allowSceneActivation = false;

        while (!_asyncOperation.isDone)
        {
            // 0 ~ 0.9
            _slider.value = Mathf.Clamp01(_asyncOperation.progress / 0.9f);
            _percent.text = (_slider.value * 100f).ToString() + "%";

            yield return null;

            if (_asyncOperation.progress >= 0.9f)
            {
                yield return YieldCache.WaitForSeconds(1.2f);
                _asyncOperation.allowSceneActivation = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
