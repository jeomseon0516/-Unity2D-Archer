using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager
{
    private static ControllerManager _instance = null;
    public static ControllerManager GetInstance()
    { 
        return _instance = _instance == null ? new ControllerManager() : _instance; 
    }

    public bool _dirLeft;
    public bool _dirRight;
    private ControllerManager()
    {

    }
}
