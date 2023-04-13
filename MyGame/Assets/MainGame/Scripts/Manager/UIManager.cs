using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonTemplate<UIManager>
{
    Dictionary<string, UI> _uiDic = new Dictionary<string, UI>(); 
    protected override void Init() {}

    public void RegistUI(string key, UI ui) 
    {
        if (_uiDic.ContainsKey(key)) return;
        _uiDic.Add(key, ui);
    }
    public UI GetFromKeyToUI(string key) 
    {
        try
        {
            return _uiDic[key];
        }
        catch (System.NullReferenceException ex)
        {
            print("Catch : " + ex.Message + "해당 밸류 값이 존재하지 않습니다.");
            return null;
        }
    }
    public void SetActiveUI(string key, bool isActive)
    {
        try
        {
            _uiDic[key].gameObject.SetActive(isActive);
        }
        catch (System.NullReferenceException ex)
        {
            print("Catch : " + ex.Message + "해당 밸류 값이 존재하지 않습니다.");
        }
    }

    private UIManager() {}
}
