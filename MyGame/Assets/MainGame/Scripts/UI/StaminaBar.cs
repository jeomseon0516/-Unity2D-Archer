using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PUB_SUB;


public partial class StaminaBar : UI, IStaminaSubscriber
{
    Slider _slider;

    protected override void Init()
    {
        _uiName = "PlayerStaminaBar";
        TryGetComponent(out _slider);
        PlayerManager.GetInstance().GetPlayerPublisher().RegisterIStaminaSubscriber(this);
        gameObject.SetActive(false);
    }
}

public partial class StaminaBar : UI, IStaminaSubscriber
{
    public void OnUpdateStamina(int stamina) { _slider.value = stamina; }
    public void OnUpdateMaxStamina(int maxStamina) { _slider.maxValue = maxStamina; }
}
