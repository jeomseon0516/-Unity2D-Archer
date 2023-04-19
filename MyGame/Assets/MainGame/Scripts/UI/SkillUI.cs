using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : UI
{
    protected override void Init()
    {
        //print(gameObject.transform.parent.childCount.ToString());
        _uiName = gameObject.name;
    }

    public string GetUIName() { return _uiName; }
}
