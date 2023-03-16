using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C = Constants;

static class Constants
{
    public const float PI    = 3.1415926f;
    public const float M_DEG = 0.3183098f;
}

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
        _maxSpeed = 2.0f;
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

        float x =  Mathf.Cos(direction) * distance * _maxSpeed;
        float y = -Mathf.Sin(direction) * distance * _maxSpeed;

        float rePosY = y + transform.position.y + Camera.main.orthographicSize * 0.5f;

        if (rePosY < _floor)
            y += _floor - rePosY;
        
        //Vector3 
        // if (y < _camera.ViewportToWorldPoint())
        transform.position += new Vector3(x, y, 0.0f) * Time.deltaTime;
    }

    private float GetDistance(Vector3 p1, Vector3 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }

    private float GetAngleToPosition(Vector3 p1, Vector3 p2) { return -Mathf.Atan2(p1.y - p2.y, p1.x - p2.x) * 180.0f * C.M_DEG; }
    private float GetDirectionToAngle(float angle) { return angle * C.PI / 180.0f; }
}