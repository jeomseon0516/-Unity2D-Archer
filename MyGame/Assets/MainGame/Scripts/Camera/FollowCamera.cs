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
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - _offsetY, 10.0f);

        float distance  = GetDistance(_player.transform.position, pos);
        float angle     = GetAngleToPosition(_player.transform.position, pos);
        float direction = GetDirectionToAngle(angle);

        float x =  Mathf.Cos(direction) * distance * 75.0f * 0.01f * _maxSpeed; // 매 프레임당 카메라 앵커의 거리와 플레이어의 75퍼센트 비율만큼만 이동
        float y = -Mathf.Sin(direction) * distance * 75.0f * 0.01f * _maxSpeed;

        float rePosY = y + transform.position.y + Camera.main.orthographicSize * 0.5f;

        if (rePosY < _floor)
            y += _floor - rePosY;
        
        transform.position += new Vector3(x, y, 0.0f) * Time.deltaTime;
    }

    private float GetDistance(Vector3 p1, Vector3 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }

    private float GetAngleToPosition(Vector3 p1, Vector3 p2) { return -Mathf.Atan2(p1.y - p2.y, p1.x - p2.x) * 180.0f * Constants.M_DEG; }
    private float GetDirectionToAngle(float angle) { return angle * Constants.PI / 180.0f; }
}