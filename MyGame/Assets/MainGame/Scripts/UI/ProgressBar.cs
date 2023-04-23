using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : UI
{
    protected Slider _slider;
    protected float _value; // 목표값

    private void Update()
    {
        float average = (_slider.value - _value) * 90 * 0.01f * Time.deltaTime * 8;
        _slider.value -= average;
    }

    protected override void Init() {}
}
