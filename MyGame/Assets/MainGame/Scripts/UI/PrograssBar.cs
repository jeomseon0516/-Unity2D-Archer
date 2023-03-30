using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OBJECT;
using OBSERVER;

/*
 * 옵저버 패턴으로 바꿔주자
 */
public partial class PrograssBar : MonoBehaviour, IObserver
{
    Slider _slider;

    private void Awake() { _slider = transform.GetComponent<Slider>(); }
}

public partial class PrograssBar : MonoBehaviour, IObserver
{
    public void UpdateData(int hp, int maxHp)
    {
        _slider.maxValue = maxHp;
        _slider.value = hp;
    }
    public void UpdateData(LivingObject obj)
    {
        _slider.maxValue = obj.GetMaxHp();
        _slider.value = obj.GetHp();
    }
}