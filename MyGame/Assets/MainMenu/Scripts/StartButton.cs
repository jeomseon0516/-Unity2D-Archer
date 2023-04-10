using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : ButtonBase
{
    protected override void PushButton()
    {
        GameManager.GetInstance().StartGame();
    }
}
