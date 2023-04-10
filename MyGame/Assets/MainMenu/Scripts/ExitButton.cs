using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : ButtonBase
{
    protected override void PushButton()
    {
        Application.Quit();
    }
}
