using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OBJECT;

/*
 * 옵저버 패턴으로 바꿔주자
 */
public class PrograssBar : MonoBehaviour
{
    Slider _slider;
    PlayerController _player;
    private void Awake()
    {
        _slider = transform.GetComponent<Slider>();
    }

    private void Start()
    {
        _player = GameObject.Find("Player").transform.Find("Player").GetComponent<PlayerController>();
        _slider.maxValue = _player.GetMaxHp();
        _slider.value    = _player.GetHp();
    }

    void Update()
    {
        _slider.value = _player.GetHp();
    }        
}
