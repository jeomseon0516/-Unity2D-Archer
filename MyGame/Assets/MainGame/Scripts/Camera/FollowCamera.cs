using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameObject _player;
    private float _offsetY;
    private float _maxSpeed;
    private float _floor;
    /*
     * 라디안값 = 육십분법 * PI / 180.0f
     * 육십분법 = 라디안 값 * 180 / 디그리 변환 값 
     */
    private void Start()
    {
        _player = GameObject.Find("Player").gameObject;
        _floor = transform.position.y + Camera.main.orthographicSize * 0.5f;
        _offsetY = transform.position.y;
        _maxSpeed = 2.0f; // 델타타임 보정하면 속도가 느리므로
    }
    private void Update()
    {
        FollowPlayer();
    }
    private void FollowPlayer()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - _offsetY);

        float distance  = Default.GetDistance(_player.transform.position, pos);
        float radian    = Default.GetPositionToRadian(_player.transform.position, pos);
        // Vector3 direction = (_player.transform.position - pos).normalized; 부자연스러움

        float x = Mathf.Cos(radian) /*direction.x*/ * distance * 75.0f * 0.01f * _maxSpeed; // 매 프레임당 카메라 앵커의 거리와 플레이어의 75퍼센트 비율만큼만 이동
        float y = Mathf.Sin(radian) /*direction.y*/ * distance * 75.0f * 0.01f * _maxSpeed;

        float rePosY = y + transform.position.y + Camera.main.orthographicSize * 0.5f;

        // 카메라가 바닥 뚫는 것 방지
        if (rePosY < _floor)
            y += _floor - rePosY;
        
        transform.position += new Vector3(x, y, 0.0f) * Time.deltaTime;
    }
}