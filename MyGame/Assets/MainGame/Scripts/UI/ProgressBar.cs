using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : UI
{
    protected Slider _slider;
    protected float _value; // 목표값
    protected float _average;

    private void Update()
    {
        _average = (_slider.maxValue - _value) * 90 * 0.01f * Time.deltaTime;
        _slider.value = _value;
    }

    protected override void Init() {}
}
