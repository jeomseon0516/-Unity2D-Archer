using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OBJECT;
using OBSERVER;

public partial class EnemyHpBar : MonoBehaviour, IEnemyObserver
{
    Slider _slider;

    private void Awake() { _slider = transform.GetComponent<Slider>(); }
}

public partial class EnemyHpBar : MonoBehaviour, IEnemyObserver
{
    public void UpdateData(int hp, int maxHp)
    {
        _slider.maxValue = maxHp;
        _slider.value = hp;
    }
    public void UpdateData(ObjectBase obj)
    {
        if (!gameObject.activeSelf)
        {
            if (obj.GetMaxHp() <= obj.GetHp()) return;
            gameObject.SetActive(true);
        }

        _slider.maxValue = obj.GetMaxHp();
        _slider.value    = obj.GetHp();

        Quaternion rotation = obj.transform.rotation;
        rotation.eulerAngles = rotation.eulerAngles.y == 180 ? Vector3.zero : new Vector3(0.0f, 180.0f, 0.0f);

        transform.localRotation = rotation;
    }
    public void UpdateData(ISubject obj) {}
}

