using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CM = ControllerManager;

public sealed class ControllerManager : SingletonTemplate<ControllerManager>
{ 
    protected override void Awake()
    {
        base.Awake();
    }

    public bool _dirLeft;
    public bool _dirRight;

    private ControllerManager() { }
}
