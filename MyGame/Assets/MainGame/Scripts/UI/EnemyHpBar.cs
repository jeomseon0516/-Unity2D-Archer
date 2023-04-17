using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OBJECT;

public partial class EnemyHpBar : MonoBehaviour, IEnemyStatsSubscriber
{
    private Slider _slider;

    private void Awake() 
    { 
        TryGetComponent(out _slider);
        gameObject.SetActive(false);
    }
}

public partial class EnemyHpBar : MonoBehaviour, IEnemyStatsSubscriber
{
    public void OnUpdateAngle(float angle)
    {
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = angle == 180 ? Vector3.zero : new Vector3(0.0f, 180.0f, 0.0f);

        transform.localRotation = rotation;
    }
    public void OnUpdateHp(int hp) 
    {
        if (!gameObject.activeSelf && hp < _slider.maxValue)
            gameObject.SetActive(true);

        _slider.value = hp; 
    }
    public void OnUpdateMaxHp(int maxHp) { _slider.maxValue = maxHp; }
}

