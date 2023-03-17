using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ControllerManager : SingletonTemplate<ControllerManager>
{ 
    public bool _dirLeft;
    public bool _dirRight;

    protected override void Init() { }
    private ControllerManager() { }
}
