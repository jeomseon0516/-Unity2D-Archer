using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OBJECT;
using OBSERVER;

/*
 * 옵저버 패턴으로 바꿔주자
 */
public partial class PrograssBar : UI, IPlayerObserver
{
    Slider _slider;
    protected override void Init()
    {
        _uiName = "PlayerHpBar";
        PlayerManager.GetInstance().RegisterObserver(this);
        TryGetComponent(out _slider);
        gameObject.SetActive(false);
    }
}

public partial class PrograssBar : UI, IPlayerObserver
{
    public void UpdateData(int hp, int maxHp)
    {
        _slider.maxValue = maxHp;
        _slider.value = hp;
    }
    public void UpdateData(ISubject obj) {}
}