using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배경 움직이기
public class BackGroundsControllor : MonoBehaviour
{
    public float _time;
    private GameObject _player;
    private Vector3 _movemane;
    private Vector3 _offset = new Vector3(0.0f, 7.5f, 0.0f);

    void Start()
    {
        _player = GameObject.Find("Player").gameObject;
    }

    void Update()
    {
        _movemane = new Vector3(
            _player.transform.position.x - transform.position.x,
            _player.transform.position.y - transform.position.y,
            0.0f);
        
        transform.position -= _movemane * Time.deltaTime * _time;
    }
}