using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Camera _camera;
    private GameObject _player;
    private float _offsetY;
    private float _maxSpeed; // 카메라를 얼마나 느리게 이동할 것인가?

    void Start()
    {
        _camera = Camera.main;
        _player = GameObject.Find("Player").gameObject; // GameObject.Find("Player").gameObject;
        _offsetY = _camera.transform.position.y;
        _maxSpeed = 2.0f;
    }

    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y - _offsetY, 10.0f);

        float distance = GetDistance(_player.transform.position, pos);
        float angle    = GetAngleToPosition(_player.transform.position, pos);

        float x =  Mathf.Cos(angle * 3.141592653f * 0.005555555f) * distance * _maxSpeed * Time.deltaTime;
        float y = -Mathf.Sin(angle * 3.141592653f * 0.005555555f) * distance * _maxSpeed * Time.deltaTime;

        _camera.transform.position += new Vector3(x, y, 0.0f);
    }

    // 상대각으로 라디안 값을 구하고 도로 변환
    private float GetAngleToPosition(Vector3 p1, Vector3 p2)
    {
        return -Mathf.Atan2(p1.y - p2.y, p1.x - p2.x) * 180.0f * 0.318309886f;
    }

    // 거리 구하기
    private float GetDistance(Vector3 p1, Vector3 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }
}
