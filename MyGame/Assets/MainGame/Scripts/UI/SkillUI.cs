using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : UI
{
    protected override void Init()
    {
        _uiName = gameObject.name + gameObject.transform.parent.childCount.ToString();
    }

    public string GetUIName() { return _uiName; }
}
