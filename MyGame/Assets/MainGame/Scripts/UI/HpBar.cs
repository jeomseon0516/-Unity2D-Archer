using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PUB_SUB;

/*
 * 옵저버 패턴으로 바꿔주자
 */
public partial class HpBar : ProgressBar, IHpSubscriber
{
    protected override void Init()
    {
        _uiName = "PlayerHpBar";
        TryGetComponent(out _slider);

        PlayerManager.GetInstance().GetPlayerPublisher().RegisterIHpSubscriber(this);
        gameObject.SetActive(false);
    }
}
public partial class HpBar : ProgressBar, IHpSubscriber
{
    public void OnUpdateHp(int hp) { _value = hp; }
    public void OnUpdateMaxHp(int maxHp) 
    { 
        _slider.maxValue = maxHp;
        _slider.value = maxHp; 
    }
}