using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI : MonoBehaviour
{
    protected string _uiName;
    protected abstract void Init();

    private void Awake()
    {
        Init();
        UIManager.GetInstance().RegistUI(_uiName, this);
    }
}
