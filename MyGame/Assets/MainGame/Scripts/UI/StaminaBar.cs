using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PUB_SUB;


public partial class StaminaBar : ProgressBar, IStaminaSubscriber
{
    protected override void Init()
    {
        _uiName = "PlayerStaminaBar";
        TryGetComponent(out _slider);

        PlayerManager.GetInstance().GetPlayerPublisher().RegisterIStaminaSubscriber(this);
        gameObject.SetActive(false);
    }
}

public partial class StaminaBar : ProgressBar, IStaminaSubscriber
{
    public void OnUpdateStamina(int stamina) { _value = stamina; }
    public void OnUpdateMaxStamina(int maxStamina) 
    {
        _slider.maxValue = maxStamina;
        _slider.value = maxStamina;
    }
}
